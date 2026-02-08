using Application.Contracts;
using Infrastructure.Settings;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

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

        public string CreateResetPasswordEmailBody(string userName, string resetLink, int expiryMinutes = 30)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Reset Your Password</title>
            </head>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2>Reset Your Password</h2>
                    <p>Hello {userName},</p>
                    <p>We received a request to reset your password for your TravelNow account.</p>
                
                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{resetLink}' 
                           style='background-color: #4CAF50; color: white; padding: 12px 24px; 
                                  text-decoration: none; border-radius: 4px; font-weight: bold;'>
                           Reset Password
                        </a>
                    </p>
                
                    <p><strong>This link will expire in {expiryMinutes} minutes.</strong></p>
                
                    <p>If you didn't request a password reset, please ignore this email or 
                       contact our support team if you have concerns.</p>
                
                    <p>Best regards,<br>The TravelNow Team</p>
                
                    <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                
                    <p style='font-size: 12px; color: #666;'>
                        If you're having trouble clicking the button, copy and paste this URL into your browser:<br>
                        <a href='{resetLink}' style='color: #666;'>{resetLink}</a>
                    </p>
                </div>
            </body>
            </html>";
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

        public string GenerateResetPasswordLink(string email, string token, string clientUrl = null)
        {
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            if (string.IsNullOrEmpty(clientUrl))
            {
                clientUrl = _frontendSettings.Value.ResetPasswordUrl
                    ?? "http://localhost:5173/reset-password";
            }

            var queryParams = new Dictionary<string, string?>
            {
                ["token"] = encodedToken,
                ["email"] = email,
                ["expires"] = DateTime.UtcNow.AddMinutes(30).ToString("o") 
            };

            return QueryHelpers.AddQueryString(clientUrl, queryParams);
        }

        public string CreateHotelApprovedEmailBody(string ownerName, string hotelName)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2>Xin chào {ownerName},</h2>
                    <p>Chúng tôi rất vui mừng thông báo rằng khách sạn <strong>{hotelName}</strong> của bạn đã vượt qua quy trình kiểm duyệt.</p>
                    <p>Hiện tại, khách sạn đã được kích hoạt và hiển thị trên hệ thống TripNow.</p>
                    <p>Bạn có thể đăng nhập vào trang quản trị để bắt đầu quản lý phòng và đơn đặt chỗ ngay bây giờ.</p>
                    <br>
                    <a href='https://tripnow.com/dashboard' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Vào trang quản trị</a>
                    <br><br>
                    <p>Trân trọng,<br>Đội ngũ TripNow</p>
                </div>";
        }

        public string CreateHotelRejectedEmailBody(string ownerName, string hotelName, string reason = null)
        {
            return $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                <h2>Xin chào {ownerName},</h2>
                <p>Chúng tôi gửi email này để thông báo về kết quả phê duyệt khách sạn <strong>{hotelName}</strong> của bạn trên hệ thống TripNow.</p>
            
                <p>Rất tiếc, hồ sơ khách sạn của bạn <strong>chưa đủ điều kiện</strong> để được phê duyệt vào lúc này.</p>
            
                <div style='background-color: #fff3cd; border: 1px solid #ffeeba; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <strong>Lý do từ chối:</strong><br>
                    <span style='color: #856404;'>{reason}</span>
                </div>

                <p>Đừng lo lắng, bạn hoàn toàn có thể cập nhật lại thông tin theo yêu cầu trên và gửi lại hồ sơ để chúng tôi xem xét.</p>
            
                <br>
                <a href='https://tripnow.com/dashboard/hotels/edit' style='background-color: #ff9800; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Cập nhật hồ sơ ngay</a>
                <br><br>
            
                <p>Nếu bạn có thắc mắc, vui lòng liên hệ với bộ phận hỗ trợ đối tác.</p>
                <p>Trân trọng,<br>Đội ngũ TripNow</p>
            </div>";
        }

        public string CreateHotelSuspendedEmailBody(string ownerName, string hotelName, string reason)
        {
            return $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                <h2>Xin chào {ownerName},</h2>
                <p>Đây là thông báo quan trọng liên quan đến khách sạn <strong>{hotelName}</strong> của bạn.</p>
            
                <p style='color: #d32f2f;'><strong>Khách sạn của bạn đã bị TẠM KHÓA (SUSPENDED) trên hệ thống TripNow.</strong></p>
                <p>Trong thời gian này, khách sạn sẽ không hiển thị trên kết quả tìm kiếm và khách hàng không thể thực hiện đặt phòng mới.</p>

                <div style='background-color: #f8d7da; border: 1px solid #f5c6cb; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <strong>Lý do đình chỉ:</strong><br>
                    <span style='color: #721c24;'>{reason}</span>
                </div>

                <p>Để khôi phục hoạt động, vui lòng xem xét và khắc phục vấn đề nêu trên, hoặc liên hệ ngay với chúng tôi để giải quyết khiếu nại.</p>
            
                <br>
                <a href='https://tripnow.com/help-center/contact' style='background-color: #d32f2f; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Liên hệ hỗ trợ</a>
                <br><br>
            
                <p>Trân trọng,<br>Bộ phận Pháp chế & Kiểm soát chất lượng TripNow</p>
            </div>";
        }

        public string CreateRefundSuccessEmailBody(string userName, string email, decimal refundAmount, string refundTransactionCode)
        {
            // Định dạng tiền tệ (Ví dụ: 500,000 VND)
            string formattedAmount = refundAmount.ToString("N0") + " VND";

            return $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; color: #333; line-height: 1.6;'>
                <h2 style='color: #4CAF50;'>Thông báo hoàn tiền thành công</h2>
            
                <p>Xin chào <strong>{userName}</strong>,</p>
            
                <p>Chúng tôi viết email này để xác nhận rằng yêu cầu hoàn tiền của bạn trên hệ thống <strong>TripNow</strong> đã được xử lý thành công.</p>
            
                <div style='background-color: #f9f9f9; padding: 15px; border-radius: 5px; border: 1px solid #e0e0e0; margin: 20px 0;'>
                    <h3 style='margin-top: 0; color: #555;'>Chi tiết giao dịch:</h3>
                    <ul style='list-style-type: none; padding: 0;'>
                        <li style='margin-bottom: 10px;'>
                            <strong>Tài khoản nhận:</strong> {email}
                        </li>
                        <li style='margin-bottom: 10px;'>
                            <strong>Số tiền hoàn:</strong> <span style='color: #e53935; font-weight: bold; font-size: 1.1em;'>{formattedAmount}</span>
                        </li>
                        <li style='margin-bottom: 10px;'>
                            <strong>Mã tham chiếu hoàn tiền:</strong> <span style='font-family: monospace; background: #eee; padding: 2px 5px; border-radius: 3px;'>{refundTransactionCode}</span>
                        </li>
                    </ul>
                </div>

                <p><strong>Lưu ý quan trọng:</strong></p>
                <p style='font-size: 0.9em; color: #666;'>
                    Mặc dù chúng tôi đã thực hiện lệnh chuyển tiền ngay lập tức, thời gian tiền về tài khoản ngân hàng của bạn có thể mất từ <strong>3 đến 14 ngày làm việc</strong> tùy thuộc vào quy trình xử lý của ngân hàng phát hành thẻ.
                </p>

                <br>
                <div style='text-align: center;'>
                    <a href='https://tripnow.com/my-bookings' style='background-color: #008CBA; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Kiểm tra lịch sử giao dịch</a>
                </div>
                <br><br>

                <p>Nếu quá thời gian trên mà bạn vẫn chưa nhận được tiền, vui lòng liên hệ với ngân hàng của bạn và cung cấp mã tham chiếu <strong>{refundTransactionCode}</strong> để được hỗ trợ tra soát.</p>

                <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
            
                <p style='font-size: 0.9em;'>
                    Trân trọng,<br>
                    <strong>Đội ngũ TripNow</strong>
                </p>
            </div>";
        }
    }
}