using Domain.Common.Response;
using MediatR;
using RecommendationService.Application.DTOs.UserViewedHotel;
using System.Collections.Generic;

namespace RecommendationService.Application.Features.UserViewedHotel.Queries.GetUserViewedHotel
{
    public class GetUserViewedHotelQuery : IRequest<Result<IEnumerable<UserViewedHotelDto>>>
    {
        public Guid UserId { get; set; }
    }
}
