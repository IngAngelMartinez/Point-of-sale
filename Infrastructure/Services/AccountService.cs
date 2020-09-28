using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Cache;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Application.Interfaces;
using Application.Exceptions;
using Application.DTOs.Account;
using Application.Wrappers;
using Domain.Settings;
using Application.Enums;
using Application.DTOs.Email;
using System.Net;
using System.Net.Sockets;
using Infrastructure.Identity;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly JWTSettings _jwtSettings;
        public AccountService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JWTSettings> jwtSettings,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new ApiException($"No Accounts Registered with {request.Email}.");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new ApiException($"Invalid Credentials for '{request.Email}'.");
            }
            if (!user.EmailConfirmed)
            {
                throw new ApiException($"Account Not Confirmed for '{request.Email}'.");
            }

            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = user.Id;
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = user.Email;
            response.UserName = user.UserName;

            var rolesList = await _userManager.GetRolesAsync(user);

            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;


            if (user.RefreshTokens != null && user.RefreshTokens.Any(a => a.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.Where(a => a.IsActive == true).FirstOrDefault();
                response.RefreshToken = activeRefreshToken.Token;
                response.RefreshTokenExpiration = activeRefreshToken.Expires;
            }
            else
            {
                var refreshToken = GenerateRefreshToken(ipAddress);
                response.RefreshToken = refreshToken.Token;
                response.RefreshTokenExpiration = refreshToken.Expires;

                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }


            return new Response<AuthenticationResponse>(response, $"Authenticated {user.UserName}.");
        }

        public async Task<Response<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);

            if (userWithSameUserName != null)
            {
                throw new ApiException($"Username '{request.UserName}' is already taken.");
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);

            if (userWithSameEmail != null)
            {
                throw new ApiException($"Email {request.Email } is already registered.");
            }


            var user = new ApplicationUser
            {
                Email = request.Email,
                Nombre = request.Nombre,
                ApellidoPaterno = request.ApellidoPaterno,
                ApellidoMaterno = request.ApellidoMaterno,
                UserName = request.UserName,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.User.ToString());
                var verificationUri = await SendVerificationEmail(user, origin);

                var emailRequest = new EmailRequest()
                {
                    From = "Admin@gmail.com",
                    To = user.Email,
                    Body = $"Please confirm your account by visiting this URL {verificationUri}",
                    Subject = "Confirm Registration",

                };

                await _emailService.SendAsync(emailRequest);
                return new Response<string>(user.Id, message: $"User Registered. Please confirm your account by visiting this URL {verificationUri}.");
            }
            else
            {
                string errors = string.Join("\r\n", result.Errors.Select(T => T.Description));
                throw new ApiException(errors);
            }

        }

        public async Task<Response<string>> ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account == null)
            {
                return new Response<string>("Not Found.");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(account);
            code = code.Replace("/", "-");
            var route = "api/account/reset-password/";
            var enpointUri = new Uri(string.Concat($"{origin}/", route));
            var resetPasswordUrl = QueryHelpers.AddQueryString(enpointUri.AbsoluteUri, "token", code);

            var emailRequest = new EmailRequest()
            {
                From = "Admin@gmail.com",
                Body = $"Change your password her => {resetPasswordUrl}.",
                To = model.Email,
                Subject = "Reset Password",
            };
            bool wasSend = await _emailService.SendAsync(emailRequest);
            if (wasSend)
            {
                return new Response<string>(resetPasswordUrl, "Email sended.");
            }
            else
            {
                return new Response<string>("Failed to send.");
            }



        }

        public async Task<Response<string>> ResetPassword(string token, ResetPasswordRequest model)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account == null)
            {
                throw new ApiException($"No Accounts Registered with {model.Email}.");
            }

            token = token.Replace("-", "/");
            var result = await _userManager.ResetPasswordAsync(account, token, model.Password);
            if (result.Succeeded)
            {
                return new Response<string>(model.Email, message: $"Password Resetted.");
            }
            else
            {
                string errors = string.Join("\n\r", result.Errors.Select(T => T.Description));
                throw new ApiException(errors);
            }

        }

        public async Task<Response<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return new Response<string>(user.Id, message: $"Account Confirmed for {user.Email}. You can now use the /api/Account/authenticate endpoint.");
            }
            else
            {
                throw new ApiException($"An error occured while confirming {user.Email}.");
            }
        }

        public async Task<Response<AuthenticationResponse>> RefreshTokenAsync(string token, string ipAddress)
        {
            var response = new AuthenticationResponse();

            var user = _userManager.Users.FirstOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
            {
                throw new ApiException($"Token did not match any users.");
            }

            var refreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == token);
            if (!refreshToken.IsActive)
            {
                throw new ApiException($"Token Not Active.");
            }

            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);


            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = user.Email;
            response.UserName = user.UserName;
            var rolesList = await _userManager.GetRolesAsync(user);
            response.Roles = rolesList.ToList();
            response.RefreshToken = newRefreshToken.Token;
            response.RefreshTokenExpiration = newRefreshToken.Expires;
            return new Response<AuthenticationResponse>(response);
        }

        public async Task<Response<string>> RevokeToken(string token, string ipAddress)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null) 
            {
                throw new ApiException($"Token did not match any users.");
            } 

            var refreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == token);
            if (!refreshToken.IsActive) 
            {
                throw new ApiException($"Token Not Active.");
            }

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            await _userManager.UpdateAsync(user);
            
            return new Response<string>("Token revoked.");
        }

        public async Task<Response<string>> LogOut()
        {
            await _signInManager.SignOutAsync();
            return new Response<string>("User logged out.");
        }

        public async Task<Response<List<IRefreshToken>>> GetTokensUser(string id)
        {
            
            var user = await _userManager.FindByIdAsync(id);

            var refreshTokens = user?.RefreshTokens;
            
            var response = new List<IRefreshToken>();

            response.AddRange(refreshTokens);

            return new Response<List<IRefreshToken>>(response);

        }



        private async Task<string> SendVerificationEmail(ApplicationUser user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/account/confirm-email/";
            var _enpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
            verificationUri     = QueryHelpers.AddQueryString(verificationUri, "code", code);

            return verificationUri;
        }

        private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            string ipAddress = GetIpAddress();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("ip", ipAddress)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }



        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);

            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private static string GetIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return string.Empty;
        }

        
    }

}
