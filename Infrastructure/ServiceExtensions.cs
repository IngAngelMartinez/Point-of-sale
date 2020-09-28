using Application.Interfaces;
using Application.Wrappers;
using Domain.Settings;
using Infraestructure.Services;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => 
                                    options.UseLazyLoadingProxies()
                                           .UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                                                         b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name)
                                            ));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                //// Password settings.
                //options.Password.RequireDigit = true;
                //options.Password.RequireLowercase = true;
                //options.Password.RequireNonAlphanumeric = true;
                //options.Password.RequireUppercase = true;
                //options.Password.RequiredLength = 6;
                //options.Password.RequiredUniqueChars = 1;

                //// Lockout settings.
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                //options.Lockout.MaxFailedAccessAttempts = 5;
                //options.Lockout.AllowedForNewUsers = true;

                //// User settings.
                //options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                //options.User.RequireUniqueEmail = true;
            });


            #region Services
            services.AddTransient<IAccountService, AccountService>();
            #endregion

            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                };
                o.Events = new JwtBearerEvents()
                {
                    //OnAuthenticationFailed = context =>
                    //{
                    //    context.NoResult();
                    //    context.Response.StatusCode = 401;
                    //    context.Response.ContentType = "application/json";
                    //    string message = TokenHandler(context.Exception.Message);
                    //    var result = JsonConvert.SerializeObject(new Response<string>(message));
                    //    return context.Response.WriteAsync(result);
                    //},
                    OnChallenge = context => 
                    {

                        // Skip the default logic.
                        context.HandleResponse();

                        if (context.Response.ContentType != null)
                        {
                            return Task.CompletedTask;
                        }
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new Response<string>(context.ErrorDescription));
                        return context.Response.WriteAsync(result);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new Response<string>("You are not authorized to access this resource"));
                        return context.Response.WriteAsync(result);
                    },

                };
            });

            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            //services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IEmailService, EmailService>();

        }

        private static string TokenHandler(string exception)
        {
            if (exception.Contains("IDX12729"))
            {
                return "Not a valid token.";
            }
            else if (exception.Contains("IDX10223"))
            {
                return "Token was expired.";
            }
            else
            {
                return "Error in the authentication.";
            }


            //o.Events = new JwtBearerEvents()
            //{
            //    OnAuthenticationFailed = c =>
            //    {
            //        c.NoResult();
            //        c.Response.StatusCode = 500;
            //        c.Response.ContentType = "text/plain";
            //        string message = TokenHandler(context.Exception.Message);
            //        var result = JsonConvert.SerializeObject(new Response<string>(message));
            //        return context.Response.WriteAsync(result);
            //    },
            //    OnChallenge = c =>
            //    {
            //        c.HandleResponse();
            //        return Task.CompletedTask;
            //    }
            //};



        }
    }
}
