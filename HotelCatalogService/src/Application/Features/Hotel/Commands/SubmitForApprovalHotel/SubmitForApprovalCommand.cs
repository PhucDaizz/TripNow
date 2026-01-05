using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.SubmitForApprovalHotel
{
    public class SubmitForApprovalCommand: IRequest<Result>
    {
        public Guid HotelId { get; init; }
        public Guid OwerId { get; init; }
    }
}
