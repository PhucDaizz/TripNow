using MediatR;

namespace PaymentService.Application.DTOs.OwnerWallet.Event
{
    public class RegisterHotelOwer: INotification
    {
        public string UserId { get; set; }
    }
}
