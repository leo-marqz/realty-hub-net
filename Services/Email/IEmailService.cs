using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealtyHub.Services.Email
{
    public interface IEmailService
    {
        Task SendAsync(string from, string to, string subject, string body, bool isHtml = false);  
        Task SendPasswordResetAsync(string to, string callbackUrl);
        Task SendEmailConfirmationAsync(string to, string callbackUrl);
    }
}