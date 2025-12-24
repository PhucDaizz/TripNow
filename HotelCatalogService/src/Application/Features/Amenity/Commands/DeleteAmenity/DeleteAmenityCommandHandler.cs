using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Repositories;
using MediatR;

namespace HotelCatalogService.Application.Features.Amenity.Commands.DeleteAmenity
{
    public class DeleteAmenityCommandHandler : IRequestHandler<DeleteAmenityCommand, Result>
    {
        private readonly IAmenityRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAmenityCommandHandler(IAmenityRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteAmenityCommand request, CancellationToken token)
        {
            var amenity = await _repo.GetByIdAsync(request.Id, token);
            if (amenity == null) return Result.Failure(new Error("Amenity.NotFound", "Tiện ích không tồn tại"));

            await _repo.DeleteAsync(amenity, token);
            await _unitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
