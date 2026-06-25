using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Enum;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands
{
    public record SyncHotelsToVectorDbCommand() : IRequest<Result>;

    public class SyncHotelsToVectorDbCommandHandler : IRequestHandler<SyncHotelsToVectorDbCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SyncHotelsToVectorDbCommandHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(SyncHotelsToVectorDbCommand request, CancellationToken cancellationToken)
        {
            var hotels = await _unitOfWork.Hotel.GetHotelsWithAggregateByStatusAsync(HotelStatus.Active, cancellationToken);

            foreach (var hotel in hotels)
            {
                hotel.RepublishToVectorDb();
                await _unitOfWork.Hotel.UpdateAsync(hotel, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
