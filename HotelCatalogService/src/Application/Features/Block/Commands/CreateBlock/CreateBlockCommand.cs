
using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Block.Commands.CreateBlock
{
    public class CreateBlockCommand : IRequest<Result<Guid>>
    {
        public Guid HotelId { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; }
    }
}
