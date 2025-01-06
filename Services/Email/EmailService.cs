

using System.Net;
using System.Net.Mail;

namespace RealtyHub.Services.Email
{
    public class EmailService
    {
        private readonly SmtpClient _smtpClient;
        public EmailService(SmtpConfiguration smtpConfig)
        {   
            try{
                _smtpClient = new SmtpClient(smtpConfig.Host, smtpConfig.Port)
                {
                    Credentials = new NetworkCredential(smtpConfig.Username, smtpConfig.Password),
                    EnableSsl = smtpConfig.EnableSsl
                };
            }catch{
                throw new System.Exception("Error al configurar el cliente SMTP");
            }
            
        }

        public async Task<bool> SendEmailAsync(string from, string to, string subject, string body, bool isHtml = false)
        {
            var mailMessage = new MailMessage(from, to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            try
            {
                await _smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}