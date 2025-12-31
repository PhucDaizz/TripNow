using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.RoomPrice;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomPrice.Queries.GetRoomTypeCalendar
{
    public class GetRoomTypeCalendarQuery : IRequest<Result<List<CalendarDayDto>>>
    {
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
