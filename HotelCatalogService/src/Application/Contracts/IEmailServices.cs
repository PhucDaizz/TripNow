namespace HotelCatalogService.Application.Contracts
{
    public interface IEmailServices
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML = true, CancellationToken cancellationToken = default);
        string CreateHotelApprovedEmailBody(string ownerName, string hotelName);
        string CreateHotelRejectedEmailBody(string ownerName, string hotelName, string reason);
    }
}
