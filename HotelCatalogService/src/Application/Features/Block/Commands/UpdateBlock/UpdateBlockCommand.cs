using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Block.Commands.UpdateBlock
{
    public class UpdateBlockCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid BlockId { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; }
    }
}
