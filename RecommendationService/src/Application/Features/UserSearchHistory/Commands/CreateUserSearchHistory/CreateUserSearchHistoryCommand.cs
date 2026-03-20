using Domain.Common.Response;
using MediatR;
using RecommendationService.Application.DTOs.UserSearchHistory;

namespace RecommendationService.Application.Features.UserSearchHistory.Commands.CreateUserSearchHistory
{
    public class CreateUserSearchHistoryCommand: IRequest<Result<UserSearchHistoryDto>>
    {
        public Guid UserId { get; set; }
        public string RawQuery { get; set; } = null!;
        public string? Destination { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? adults { get; set; }
        public int? children { get; set; }
    }
}
