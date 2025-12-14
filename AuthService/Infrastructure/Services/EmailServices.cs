using Application.Contracts;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly IOptions<FrontendSettings> _frontendSettings;
        private readonly ILogger<EmailServices> _logger;

        public EmailServices(
            IOptions<EmailSettings> emailSettings,
            IOptions<FrontendSettings> frontendSettings,
            ILogger<EmailServices> logger)
        {
            _emailSettings = emailSettings;
            _frontendSettings = frontendSettings;
            _logger = logger;
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

        public string CreateConfirmationEmailBody(string userName, string confirmationLink)
        {
            return $@"
                <!DOCTYPE html>
                <html lang=""vi"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Xác nhận tài khoản - Doris</title>
                </head>
                <body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; margin:0; padding:0;"">
                    <div style=""width:100%; max-width:600px; margin:0 auto; background-color:#ffffff; padding:20px; box-shadow:0 0 10px rgba(0,0,0,0.05);"">
        
                        <!-- Logo / Header -->
                        <div style=""text-align:center; padding:20px 0; border-bottom:1px solid #eeeeee;"">
                            <img src=""https://iili.io/KoLNvF2.jpg"" alt=""Doris Logo"" style=""max-width:150px; margin-bottom:10px;"">
                            <h1 style=""color:#333333; font-size:22px; margin:0;"">Xác nhận tài khoản của bạn</h1>
                        </div>

                        <!-- Nội dung -->
                        <div style=""padding:30px 20px; text-align:center; color:#555555;"">
                            <p style=""font-size:16px; margin:0 0 15px;"">Xin chào {userName},</p>
                            <p style=""font-size:16px; margin:0 0 20px; line-height:1.6;"">
                                Cảm ơn bạn đã đăng ký tài khoản tại <strong>Doris</strong> – nền tảng mua sắm thời trang trực tuyến.  
                                Vui lòng xác nhận địa chỉ email của bạn bằng cách nhấn vào nút bên dưới:
                            </p>
            
                            <a href=""{confirmationLink}"" 
                                style=""display:inline-block; padding:12px 28px; background-color:#111111; color:#ffffff; 
                                        text-decoration:none; border-radius:30px; font-size:16px; font-weight:bold;"">
                                Xác nhận Email
                            </a>

                            <p style=""font-size:14px; margin:25px 0 0; color:#888888; line-height:1.6;"">
                                Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.
                            </p>
                        </div>

                        <!-- Footer -->
                        <div style=""text-align:center; padding:20px; font-size:12px; color:#999999; border-top:1px solid #eeeeee;"">
                            <p style=""margin:0;"">Trân trọng, <br> Đội ngũ Doris</p>
                            <p style=""margin:8px 0 0;"">
                                Hỗ trợ khách hàng: 
                                <a href=""mailto:support@doris.com"" style=""color:#111111; text-decoration:none;"">support@doris.com</a>
                            </p>
                            <p style=""margin:8px 0 0; font-size:11px; color:#bbbbbb;"">
                                © 2025 Doris. Tất cả các quyền được bảo lưu.
                            </p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

        public string CreateResetPasswordEmailBody(string userName, string callback)
        {
            return $@"
                <!DOCTYPE html>
                    <html lang=""vi"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Đặt lại mật khẩu - Doris</title>
                    </head>
                    <body style=""font-family: Arial, sans-serif; background-color:#f4f4f4; margin:0; padding:0;"">
                        <div style=""width:100%; max-width:600px; margin:0 auto; background-color:#ffffff; padding:20px; box-shadow:0 0 10px rgba(0,0,0,0.05);"">
        
                            <!-- Logo / Header -->
                            <div style=""text-align:center; padding:20px 0; border-bottom:1px solid #eeeeee;"">
                                <img src=""https://iili.io/KoLNvF2.jpg"" alt=""Doris Logo"" style=""max-width:150px; margin-bottom:10px;"">
                                <h1 style=""color:#333333; font-size:22px; margin:0;"">Đặt lại mật khẩu của bạn</h1>
                            </div>

                            <!-- Nội dung -->
                            <div style=""padding:30px 20px; text-align:center; color:#555555;"">
                                <p style=""font-size:16px; margin:0 0 15px;"">Xin chào {userName},</p>
                                <p style=""font-size:16px; margin:0 0 20px; line-height:1.6;"">
                                    Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản tại <strong>Doris</strong>.  
                                    Vui lòng nhấn vào nút bên dưới để tạo mật khẩu mới:
                                </p>
            
                                <a href=""{callback}"" 
                                   style=""display:inline-block; padding:12px 28px; background-color:#111111; color:#ffffff; 
                                          text-decoration:none; border-radius:30px; font-size:16px; font-weight:bold;"">
                                    Đặt lại mật khẩu
                                </a>

                                <p style=""font-size:14px; margin:25px 0 0; color:#888888; line-height:1.6;"">
                                    Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.  
                                    Vì lý do bảo mật, liên kết sẽ hết hạn sau <strong>30 phút</strong>.
                                </p>
                            </div>

                            <!-- Footer -->
                            <div style=""text-align:center; padding:20px; font-size:12px; color:#999999; border-top:1px solid #eeeeee;"">
                                <p style=""margin:0;"">Trân trọng, <br> Đội ngũ Doris</p>
                                <p style=""margin:8px 0 0;"">
                                    Hỗ trợ khách hàng: 
                                    <a href=""mailto:support@doris.com"" style=""color:#111111; text-decoration:none;"">support@doris.com</a>
                                </p>
                                <p style=""margin:8px 0 0; font-size:11px; color:#bbbbbb;"">
                                    © 2025 Doris. Tất cả các quyền được bảo lưu.
                                </p>
                            </div>
                        </div>
                    </body>
                    </html>
            ";
        }

        public string GenerateConfirmationLink(string userId, string token)
        {
            var frontendUrl = _frontendSettings.Value.BaseUrl;

            if (string.IsNullOrEmpty(frontendUrl))
            {
                throw new InvalidOperationException("Frontend URL is not configured");
            }

            var encodedToken = System.Web.HttpUtility.UrlEncode(token);
            var encodedUserId = System.Web.HttpUtility.UrlEncode(userId);

            return $"{frontendUrl}/verify-email?userId={encodedUserId}&token={encodedToken}";
        }

        public string GenerateResetPasswordLink(string userId, string token)
        {
            var frontendUrl = _frontendSettings.Value.BaseUrl;

            if (string.IsNullOrEmpty(frontendUrl))
            {
                throw new InvalidOperationException("Frontend URL is not configured");
            }

            var encodedToken = System.Web.HttpUtility.UrlEncode(token);
            var encodedUserId = System.Web.HttpUtility.UrlEncode(userId);

            return $"{frontendUrl}/reset-password?userId={encodedUserId}&token={encodedToken}";
        }
    }
}