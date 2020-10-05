using Application.DTOs.Email;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Infraestructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings mailSettings;
        private readonly ILogger<EmailService> logger;

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            this.mailSettings = mailSettings.Value;
            this.logger = logger;
        }

        public async Task<bool> SendAsync(EmailRequest request)
        {
            bool response;

            try
            {
             
                var email = new MimeMessage();
                //email.Sender = MailboxAddress.Parse(request.From);
                email.From.Add(new MailboxAddress(mailSettings.DisplayName, request.From));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = request.Body,
                };
                email.Body = builder.ToMessageBody();


                using var smtp = new SmtpClient();
                smtp.Connect(mailSettings.SmtpHost, mailSettings.SmtpPort);
                smtp.Authenticate(mailSettings.SmtpUser, mailSettings.SmtpPass);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                response = true;

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                throw new ApiException(ex.Message);
            }

            return response;
        }
    }
}
