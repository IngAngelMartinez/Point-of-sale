using Application.DTOs.Account;
using Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAccountService
    {
        Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress);
        Task<Response<string>> RegisterAsync(RegisterRequest request, string origin);
        Task<Response<string>> ConfirmEmailAsync(string userId, string code);
        Task <Response<string>>ForgotPassword(ForgotPasswordRequest model, string origin);
        Task<Response<string>> ResetPassword(string token, ResetPasswordRequest model);
        Task<Response<string>> LogOut();
        Task<Response<AuthenticationResponse>> RefreshTokenAsync(string token, string ipAddress);
        Task<Response<string>> RevokeToken(string token, string ipAddress);
        Task<Response<List<IRefreshToken>>> GetTokensUser(string id);
    }
}
