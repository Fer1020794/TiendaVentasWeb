using System.Net;
using System.Net.Mail;

namespace TiendaVentas.Web.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarPedidoAsync(int idPedido, string rutaPdfFisica)
        {
            var host = _configuration["SmtpSettings:Host"];
            var port = int.Parse(_configuration["SmtpSettings:Port"] ?? "587");
            var enableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"] ?? "true");
            var user = _configuration["SmtpSettings:User"];
            var password = _configuration["SmtpSettings:Password"];
            var from = _configuration["SmtpSettings:From"];
            var toEmpresa = _configuration["SmtpSettings:ToEmpresa"];

            using var message = new MailMessage();
            message.From = new MailAddress(from!);
            message.To.Add(toEmpresa!);
            message.Subject = $"Nuevo pedido MC Nails - Pedido No. {idPedido}";
            message.Body = $"Se ha generado un nuevo pedido en MC Nails. Se adjunta el PDF del pedido No. {idPedido}.";
            message.Attachments.Add(new Attachment(rutaPdfFisica));

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, password),
                EnableSsl = enableSsl
            };

            await client.SendMailAsync(message);
        }
    }
}