using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace RiccioneDisco.Models
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            // Ignora la validazione del certificato SSL
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };

            var emailSettings = _configuration.GetSection("EmailSettings");
            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(emailSettings["FromEmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(new MailAddress(toEmail));

            SmtpClient smtp = new SmtpClient()
            {
                Host = emailSettings["PrimaryDomain"],
                Port = int.Parse(emailSettings["PrimaryPort"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(emailSettings["UsernameEmail"], emailSettings["UsernamePassword"])
            };

            smtp.Send(mail);
        }
    }

}
