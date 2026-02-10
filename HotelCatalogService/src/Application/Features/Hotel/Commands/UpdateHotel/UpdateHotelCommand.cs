using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.UpdateHotel
{
    public class UpdateHotelCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid OwnerId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Rating { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
