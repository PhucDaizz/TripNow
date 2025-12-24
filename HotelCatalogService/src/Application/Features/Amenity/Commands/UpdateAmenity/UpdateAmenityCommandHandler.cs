using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Repositories;
using MediatR;

namespace HotelCatalogService.Application.Features.Amenity.Commands.UpdateAmenity
{
    public class UpdateAmenityCommandHandler : IRequestHandler<UpdateAmenityCommand, Result>
    {
        private readonly IAmenityRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAmenityCommandHandler(IAmenityRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateAmenityCommand request, CancellationToken token)
        {
            var amenity = await _repo.GetByIdAsync(request.Id, token);
            if (amenity == null) return Result.Failure(new Error("Amenity.NotFound", "Tiện ích không tồn tại"));

            amenity.Update(request.Name, request.Icon);

            await _repo.UpdateAsync(amenity, token);
            await _unitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
