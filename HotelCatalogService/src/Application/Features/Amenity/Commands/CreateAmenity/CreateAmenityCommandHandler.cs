using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Repositories;
using MediatR;

namespace HotelCatalogService.Application.Features.Amenity.Commands.CreateAmenity
{
    public class CreateAmenityCommandHandler : IRequestHandler<CreateAmenityCommand, Result<Guid>>
    {
        private readonly IAmenityRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAmenityCommandHandler(IAmenityRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateAmenityCommand request, CancellationToken token)
        {
            var amenity = Domain.Entities.Amenity.Create(request.Name, request.Icon);
            await _repo.AddAsync(amenity, token);
            await _unitOfWork.SaveChangesAsync(token);
            return Result.Success(amenity.Id);
        }
    }
}
