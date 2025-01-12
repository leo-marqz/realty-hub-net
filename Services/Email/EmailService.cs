
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace RealtyHub.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private SmtpConfiguration _stmtpConfig;
        private SmtpClient _smtpClient;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {   
            _configuration = configuration;
            _logger = logger;
            _stmtpConfig = _configuration.GetSection("SmtpConfiguration").Get<SmtpConfiguration>();
        }

        public async Task SendAsync(string from, string to, string subject, string body, bool isHtml = false)
        {
            if(from.IsNullOrEmpty() || to.IsNullOrEmpty() || subject.IsNullOrEmpty() || body.IsNullOrEmpty())
            {
                throw new ArgumentException("Invalid email parameters");
            }
            
            var mailMessage = new MailMessage( from: new MailAddress(from), to: new MailAddress(to) )
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            try
            {
                _smtpClient = new SmtpClient(_stmtpConfig.Host, _stmtpConfig.Port){
                    Credentials = new NetworkCredential(_stmtpConfig.Username, _stmtpConfig.Password),
                    EnableSsl = _stmtpConfig.EnableSsl
                };

            } catch (System.Exception)
            {
                throw new Exception("Failed to create SMTP client");
            }

            try
            {
                await _smtpClient.SendMailAsync(mailMessage);
            } catch
            {
                throw new Exception("Failed to send email");
            }

        }

        public async Task SendPasswordResetAsync(string to, string callbackUrl)
        {
            StringBuilder body = new StringBuilder();
            body.Append("<!DOCTYPE html>");
            body.Append("<html lang='en'>");
            body.Append(@"<head>
                            <meta charset='UTF-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            <title>Reset Your Password</title>
                            <style>
                                body {
                                    font-family: Arial, sans-serif; background-color: #f4f4f9;
                                    margin: 0; padding: 0; color: #333;
                                }
                                .email-container {
                                    max-width: 600px; margin: 20px auto;
                                    background-color: #ffffff; border-radius: 10px;
                                    overflow: hidden; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                    border: 1px solid #eaeaea;
                                }
                                .header { background-color: #2d3748; color: #ffffff; text-align: center; padding: 20px; }
                                .header h1 { margin: 0; font-size: 28px; font-weight: 700; }
                                .header span { font-weight: 400; color: #a0aec0; }
                                .body { padding: 20px; text-align: center; }
                                .body p { line-height: 1.8; font-size: 16px; margin-bottom: 20px; }
                                .button {
                                    display: inline-block; background-color: #4caf50;
                                    color: #ffffff; text-decoration: none; padding: 10px 20px;
                                    border-radius: 5px; font-weight: bold; font-size: 16px;
                                }
                                .button:hover { background-color: #3e8e41; }
                                .footer {
                                    background-color: #edf2f7;
                                    text-align: center;
                                    padding: 15px;
                                    font-size: 12px;
                                    color: #718096;
                                }
                                .footer a { color: #4caf50; text-decoration: none; }
                                .footer a:hover { text-decoration: underline; }
                            </style>
                        </head>
            ");

            body.Append(@"<body>
                            <div class='email-container'>
                                <div class='header'>
                                    <h1>Realty <span>Hub</span></h1>
                                </div>
                                <div class='body'>
                                    <p>Hola,</p>
                                    <p>Recibimos una solicitud para restablecer tu contraseña. Haz clic en el botón de abajo para continuar:</p>
                ");
            
            body.Append($"<a href='{callbackUrl}' class='button'>Restablecer Contraseña</a>");
            body.Append("<p>Si no solicitaste este cambio, puedes ignorar este correo y tu contraseña seguirá siendo la misma.</p>");
            body.Append("<p>Gracias,<br>El equipo de Realty Hub</p>");
            body.Append("</div>");
            body.Append("<div class='footer'>");
            body.Append("<p>¿Problemas con el botón? Copia y pega el siguiente enlace en tu navegador:</p>");
            body.Append($"<p><a href='{callbackUrl}'>{callbackUrl}</a></p>");
            body.Append("</div>");
            body.Append("</div>");
            body.Append("</body>");
            body.Append("</html>");


            await SendAsync(
                from: "no-repply@realtyhub.com",
                to: to,
                subject: "Reset Password - Realty Hub",
                body: body.ToString(),
                isHtml: true
            );
        }
    }
}