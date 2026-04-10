using Application.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOtpAsync(string toEmail, string otp, CancellationToken cancellationToken = default)
        {
            var mailSection = _configuration.GetSection("Mail");
            var from = mailSection["Mail"];
            var displayName = mailSection["DisplayName"];
            var password = mailSection["Password"];
            var host = mailSection["Host"];
            var portRaw = mailSection["Port"];
            var port = int.TryParse(portRaw, out var p) ? p : 587;
            var skipCertificateValidation = bool.TryParse(mailSection["SkipCertificateValidation"], out var skipCert) && skipCert;
            var checkCertificateRevocation =
                !bool.TryParse(mailSection["CheckCertificateRevocation"], out var checkCrl) || checkCrl;

            if (string.IsNullOrWhiteSpace(from) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(host))
            {
                throw new InvalidOperationException("Thiếu cấu hình email SMTP. Vui lòng thiết lập các biến môi trường Mail__*.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(displayName ?? "Vua Thợ", from));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Mã OTP xác thực - Vua Thợ";

            message.Body = new TextPart("plain")
            {
                Text =
                    $"Mã OTP của bạn là: {otp}\n\n" +
                    "Mã OTP có hiệu lực trong 5 phút.\n" +
                    "Nếu bạn không yêu cầu OTP, vui lòng bỏ qua email này."
            };

            using var client = new SmtpClient();

            if (skipCertificateValidation)
            {
                client.ServerCertificateValidationCallback = (_, _, _, _) => true;
                client.CheckCertificateRevocation = false;
            }
            else
            {
                client.CheckCertificateRevocation = checkCertificateRevocation;
            }

            await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(from, password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}

