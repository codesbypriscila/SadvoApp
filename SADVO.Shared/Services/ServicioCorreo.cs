using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SADVO.Shared.Interfaces;

namespace SADVO.Shared.Services
{
    public class ServicioCorreo : IServicioCorreo
    {
        public async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SADVO", "priscilaperezherrera@gmail.com")); 
            message.To.Add(MailboxAddress.Parse(destinatario));
            message.Subject = asunto;

            var builder = new BodyBuilder
            {
                HtmlBody = cuerpoHtml
            };

            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync("priscilaperezherrera@gmail.com", "qgclrpdosfcbpcxl");

            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}