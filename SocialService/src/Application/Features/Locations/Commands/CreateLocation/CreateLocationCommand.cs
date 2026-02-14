using Domain.Common.Response;
using MediatR;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.Locations.Commands.CreateLocation
{
    public class CreateLocationCommand : IRequest<Result<Guid>>
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }  
        public double Longitude { get; set; }
        public LocationType Type { get; set; }
    }
}
