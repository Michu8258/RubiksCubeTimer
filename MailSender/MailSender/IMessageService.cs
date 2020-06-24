using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MailSender.MailSender
{
    public interface IMessageService
    {
        Task<bool> ResendVerificationCode(string emailAddress, string code, string userName);
        Task<bool> SendNewVerificationCode(string emailAddress, string code, string UserName);
        Task<bool> SendPasswordResetCode(string emailAddress, string code,
            string userName, DateTime requestTime, DateTime expireTime, int attempts);
        Task SendGlobalMessage(IDictionary<string, string> receivers, string message, string subject);
        Task<bool> SendCustomEmail(string username, string email, string subjest, string message);
    }
}
