using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Block.Commands.DeleteBlock
{
    public class DeleteBlockCommand: IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid BlockId { get; set; }
        public Guid OwnerId { get; set; }
    }
}
