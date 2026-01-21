using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomType.Commands.SetPolicy
{
    public class SetRoomTypePolicyCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid PolicyId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class SetRoomTypePolicyCommandHandler : IRequestHandler<SetRoomTypePolicyCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SetRoomTypePolicyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(SetRoomTypePolicyCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdIncludeAsync(request.HotelId, token, h => h.RoomTypes);

            if (hotel == null) return Result.Failure(new Error("Hotel.NotFound", "Hotel not found"));
            if (hotel.OwnerId != request.OwnerId) return Result.Failure(new Error("Hotel.Forbidden", "No permission"));

            var roomType = hotel.RoomTypes.FirstOrDefault(x => x.Id == request.RoomTypeId);
            if (roomType == null) return Result.Failure(new Error("RoomType.NotFound", "Room type does not exist"));

            var policy = await _unitOfWork.CancellationPolicy.GetByIdAsync(request.PolicyId, token);
            if (policy == null) return Result.Failure(new Error("Policy.NotFound", "Cancellation policy not found"));
            if (policy.HotelId != request.HotelId) return Result.Failure(new Error("Policy.Invalid", "Policy does not belong to this hotel"));

            roomType.SetPolicy(request.PolicyId);

            await _unitOfWork.Hotel.UpdateAsync(hotel, token);
            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}
