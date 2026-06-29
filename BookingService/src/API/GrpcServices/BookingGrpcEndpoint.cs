using BookingService.API.Protos;
using BookingService.Application.Features.Booking.Queries.IsBookingExisting;
using BookingService.Application.Features.Inventory.Queries.CheckRoomUsage;
using Grpc.Core;
using MediatR;

namespace BookingService.API.GrpcServices
{
    public class BookingGrpcEndpoint : BookingGrpc.BookingGrpcBase
    {
        private readonly IMediator _mediator;

        public BookingGrpcEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<CheckRoomUsageResponse> CheckIsHaveAnyBookInFurniture(
            CheckRoomUsageRequest request,
            ServerCallContext context)
        {
            var roomId = Guid.Parse(request.RoomTypeId);

            var result = await _mediator.Send(new CheckRoomUsageQuery { RoomTypeId = roomId });

            return new CheckRoomUsageResponse { IsUsed = result };
        }

        public override async Task<IsBookingExistingResponse> IsBookingExisting(
            IsBookingExistingRequest request,
            ServerCallContext context)
        {
            var query = new IsBookingExistingQuery
            {
                BookingId = Guid.Parse(request.BookingId)
            };

            var result = await _mediator.Send(query);

            return new IsBookingExistingResponse
            {
                IsExisting = result.Value 
            };
        }
    }
}
