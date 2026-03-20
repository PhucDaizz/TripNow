using Domain.Common.Response;
using MediatR;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Application.DTOs.UserSearchHistory;

namespace RecommendationService.Application.Features.UserSearchHistory.Commands.CreateUserSearchHistory
{
    public class CreateUserSearchHistoryCommandHandler : IRequestHandler<CreateUserSearchHistoryCommand, Result<UserSearchHistoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserSearchHistoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserSearchHistoryDto>> Handle(CreateUserSearchHistoryCommand request, CancellationToken cancellationToken)
        {
            var searchHistory = Domain.Entities.UserSearchHistory.Create(
                    request.UserId,
                    request.RawQuery,
                    request.Destination,
                    request.CheckInDate,
                    request.CheckOutDate,
                    request.adults,
                    request.children);

            await _unitOfWork.UserSearchHistories.AddAsync(searchHistory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success<UserSearchHistoryDto>(new UserSearchHistoryDto
            {
                UserId = request.UserId,
                RawQuery = request.RawQuery,
                Destination = request.Destination,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                Adults = request.adults,
                Children = request.children,
                SearchedAt = searchHistory.SearchedAt

            });
        }
    }
}
