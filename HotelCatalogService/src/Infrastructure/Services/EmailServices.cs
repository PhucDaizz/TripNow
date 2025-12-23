using HotelCatalogService.Application.Contracts;
using HotelCatalogService.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace HotelCatalogService.Infrastructure.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly ILogger<EmailServices> _logger;

        public EmailServices(IOptions<EmailSettings> emailSettings, ILogger<EmailServices> logger)
        {
            _emailSettings = emailSettings;
            _logger = logger;
        }

        public string CreateHotelApprovedEmailBody(string ownerName, string hotelName)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2>Xin chào {ownerName},</h2>
                    <p>Chúng tôi rất vui mừng thông báo rằng khách sạn <strong>{hotelName}</strong> của bạn đã vượt qua quy trình kiểm duyệt.</p>
                    <p>Hiện tại, khách sạn đã được kích hoạt và hiển thị trên hệ thống TripUp.</p>
                    <p>Bạn có thể đăng nhập vào trang quản trị để bắt đầu quản lý phòng và đơn đặt chỗ ngay bây giờ.</p>
                    <br>
                    <a href='https://tripup.com/dashboard' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Vào trang quản trị</a>
                    <br><br>
                    <p>Trân trọng,<br>Đội ngũ TripUp</p>
                </div>";
        }

        public string CreateHotelRejectedEmailBody(string ownerName, string hotelName, string reason)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML = true, CancellationToken cancellationToken = default)
        {
            try
            {
                var settings = _emailSettings.Value;

                using var smtpClient = new SmtpClient(settings.MailServer, settings.MailPort)
                {
                    Credentials = new NetworkCredential(settings.FromEmail, settings.Password),
                    EnableSsl = settings.UseSsl
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(settings.FromEmail, settings.DisplayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isBodyHTML
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage, cancellationToken);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                return true;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Email sending was cancelled for {Email}", toEmail);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return false;
            }
        }
    }
}
