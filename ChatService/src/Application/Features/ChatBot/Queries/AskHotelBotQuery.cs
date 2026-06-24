using Domain.Common.Response;
using MediatR;

namespace ChatService.Application.Features.ChatBot.Queries
{
    public record AskHotelBotQuery(Guid HotelId, string Message) : IRequest<Result<string>>;
}
