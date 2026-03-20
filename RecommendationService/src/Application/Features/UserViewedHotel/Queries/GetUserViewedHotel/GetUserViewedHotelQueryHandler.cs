using Domain.Common.Response;
using MediatR;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Application.DTOs.UserViewedHotel;

namespace RecommendationService.Application.Features.UserViewedHotel.Queries.GetUserViewedHotel
{
    public class GetUserViewedHotelQueryHandler : IRequestHandler<GetUserViewedHotelQuery, Result<IEnumerable<UserViewedHotelDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserViewedHotelQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<UserViewedHotelDto>>> Handle(GetUserViewedHotelQuery request, CancellationToken cancellationToken)
        {
            var viewedHotels = await _unitOfWork.UserViewedHotels.GetByUserIdAsync(request.UserId);

            var result = viewedHotels.Select(v => new UserViewedHotelDto
            {
                UserId = v.UserId,
                HotelId = v.HotelId,
                ViewedAt = v.ViewedAt
            });

            return Result.Success(result);
        }
    }
}
