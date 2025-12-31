using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomPrice.Commands.BulkSetRoomPrice
{
    public class BulkSetRoomPriceCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid OwnerId { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal Price { get; set; }
        public List<DayOfWeek>? SpecificDays { get; set; }
    }
}
