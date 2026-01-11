using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.RoomType;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomType.Commands.CreateRoomType
{
    public class CreateRoomTypeCommandHandler : IRequestHandler<CreateRoomTypeCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventService _integrationEventService;

        public CreateRoomTypeCommandHandler(IUnitOfWork unitOfWork, IIntegrationEventService integrationEventService)
        {
            _unitOfWork = unitOfWork;
            _integrationEventService = integrationEventService;
        }

        public async Task<Result<Guid>> Handle(CreateRoomTypeCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdIncludeAsync(request.HotelId, token, h => h.RoomTypes);

            if (hotel == null) return Result.Failure<Guid>(new Error("Hotel.NotFound", "Not found hotel"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure<Guid>(new Error("Hotel.Forbidden", "No permission"));

            hotel.DefineRoomType(request.Name, request.BasePrice, request.Capacity, request.SizeM2);

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);

            var newRoomTypeEvent = new RoomTypeCreatedEvent {
                RoomTypeId = hotel.RoomTypes.Last().Id,
                HotelId = hotel.Id,
                InitialStock = 0
            };

            await _integrationEventService.PublishAsync<RoomTypeCreatedEvent>(newRoomTypeEvent, "hotel-catalog.events", "topic", "roomtype.create", token);

            var newRoomType = hotel.RoomTypes.Last();

            return Result.Success(newRoomType.Id);
        }
    }
}
