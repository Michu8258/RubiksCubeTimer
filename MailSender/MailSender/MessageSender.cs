using ApplicationResources.UserInterface;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MailSender.MailSender
{
    public class MessageSender : IMessageService
    {
        private readonly IConfiguration _configuration;

        public MessageSender(IConfiguration config)
        {
            _configuration = config;
        }

        private EmailAddress GetMessageSender()
        {
            return new EmailAddress(_configuration["EmailClient:From"], _configuration["EmailClient:FromUserName"]);
        }

        private EmailAddress GetMessageReceiver(string emailAddress, string userName)
        {
            return new EmailAddress(emailAddress, userName);
        }

        private async Task<bool> SendMessage(string subject, string content, string emailAddress, string userName)
        {
            var client = new SendGridClient(_configuration["EmailClient:Key"]);
            var msg = MailHelper.CreateSingleEmail(GetMessageSender(), GetMessageReceiver(emailAddress, userName), subject, string.Empty, content);
            var result = await client.SendEmailAsync(msg);
            return await Task.FromResult(result.Headers != null);
        }

        public async Task<bool> ResendVerificationCode(string emailAddress, string code, string userName)
        {
            return await SendMessage(
                GenerateReVerificationCodeMailBody(code, userName, true),
                GenerateReVerificationCodeMailBody(code, userName),
                emailAddress,
                userName);
        }

        private string GenerateReVerificationCodeMailBody(string code, string userName, bool subject = false)
        {
            if (subject) return EmailResources.ResendVerificationKeySybject;
            else return string.Format(EmailResources.ResendVerificationCodeBody, userName, code);
        }

        public async Task<bool> SendNewVerificationCode(string emailAddress, string code, string userName)
        {
            return await SendMessage(
                GenerateVerificationCodeMailBody(code, userName, true),
                GenerateVerificationCodeMailBody(code, userName),
                emailAddress,
                userName);
        }

        private string GenerateVerificationCodeMailBody(string code, string userName, bool subject = false)
        {
            if (subject) return EmailResources.VerificationCodeSubject;
            else return string.Format(EmailResources.VerificationCodeBody, userName, code);
        }

        public async Task<bool> SendPasswordResetCode(string emailAddress, string code,
            string userName, DateTime requestTime, DateTime expireTime, int attempts)
        {
            return await SendMessage(
                GeneratePasswordResetCodeMailBody(code, userName, requestTime, expireTime, attempts, true),
                GeneratePasswordResetCodeMailBody(code, userName, requestTime, expireTime, attempts),
                emailAddress,
                userName);
        }

        private string GeneratePasswordResetCodeMailBody(string code, string userName,
            DateTime requestTime, DateTime expireTime, int attempts, bool subject = false)
        {
            if (subject) return EmailResources.PasswordResetSubject;
            else return string.Format(EmailResources.PasswordResetBody, userName, code,
                (string.Format("{0} {1}", requestTime.ToLongDateString(), requestTime.ToLongTimeString())),
                (string.Format("{0} {1}", expireTime.ToLongDateString(), expireTime.ToLongTimeString())), attempts);
        }

        public async Task SendGlobalMessage(IDictionary<string, string> receivers, string message, string subject)
        {
            foreach (var email in receivers)
            {
                await SendMessage(subject, message, email.Key, email.Value);
            }
        }

        public async Task<bool> SendCustomEmail(string username, string email, string subject, string message)
        {
            return await SendMessage(subject, message, email, username);
        }
    }
}
