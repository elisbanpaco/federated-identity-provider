using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace federated_identity_provider.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();
            
            // 1. Configura el remitente (Cambia esto por tu correo de Gmail real)
            var remitente = "keven58@ethereal.email"; // SOLO FUNCIONA PARA DESARROLLO
            emailMessage.From.Add(new MailboxAddress("Mi SSO de Identidad", remitente));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            // 2. Construye el cuerpo del mensaje
            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            // 3. Envía el correo usando MailKit
            using var client = new SmtpClient();
            try
            {
                // Conectamos al servidor SMTP de Gmail
                await client.ConnectAsync("smtp.ethereal.email", 587, SecureSocketOptions.StartTls); // SOLO FUNCIONA PARA DESARROLLO
                
                // IMPORTANTE: Leemos la contraseña desde un entorno seguro, o la pegas aquí temporalmente
                // Para tu .env o appsettings: _configuration["EmailPassword"]
                var password = "5NtzefdgZyzSZwJ8EN";  // SOLO FUNCIONA PARA DESARROLLO
                
                await client.AuthenticateAsync(remitente, password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // En producción aquí usarías un ILogger, por ahora consola está bien
                Console.WriteLine($"Error crítico enviando correo: {ex.Message}");
            }
        }
    }
}