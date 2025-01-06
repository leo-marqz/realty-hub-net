

namespace RealtyHub.Services.Email
{
    public class SmtpConfiguration
    {
        /// <summary>
        /// The host of the SMTP server
        /// </summary>
        public string? Host { get; set; }

        /// <summary>
        /// The port of the SMTP server
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The username to authenticate with the SMTP server
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// The password to authenticate with the SMTP server
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Whether to use SSL to connect to the SMTP server
        /// </summary>
        public bool EnableSsl { get; set; }
    }
}