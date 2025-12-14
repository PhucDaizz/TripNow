using Application.Common.Interfaces;
using Application.DTOs.User;
using AutoMapper;
using Domain.Common.Response;
using MediatR;

namespace Application.Features.User.Queries.GetInfoDetail
{
    public class GetInfoDetailQueryHandler : IRequestHandler<GetInfoDetailQuery, Result<InforDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetInfoDetailQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<InforDto>> Handle(GetInfoDetailQuery request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Auth.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                return Result.Failure<InforDto>(new Error("NotFound","User not found."));
            }
            var inforDto = _mapper.Map<InforDto>(user);
            return Result.Success(inforDto);
        }
    }
}
