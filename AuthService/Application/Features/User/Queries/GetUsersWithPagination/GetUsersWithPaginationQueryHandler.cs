using Application.Common.Interfaces;
using Application.DTOs.User;
using Domain.Common.Models;
using Domain.Common.Response;
using MediatR;

namespace Application.Features.User.Queries.GetUsersWithPagination
{
    public class GetUsersWithPaginationQueryHandler : IRequestHandler<GetUsersWithPaginationQuery, Result<PagedResult<UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUsersWithPaginationQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<UserDto>>> Handle(GetUsersWithPaginationQuery request, CancellationToken cancellationToken)
        {
            IList<string>? userIdsInRole = null;
            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                var roleId = await _unitOfWork.Auth.GetUserRoleId(request.Role, cancellationToken);
                if (roleId != null)
                {
                    userIdsInRole = await _unitOfWork.Auth.GetUserIdsByRoleIdAsync(roleId);
                }
                else
                {
                    return Result.Success(new PagedResult<UserDto>(new List<UserDto>(), 0, request.PageNumber, request.PageSize));
                }
            }

            var (pagedUsers, totalCount) = await _unitOfWork.Auth.GetFilteredUsersWithPaginationAsync(request, userIdsInRole, cancellationToken);

            if (totalCount == 0)
            {
                return Result.Success(new PagedResult<UserDto>(new List<UserDto>(), 0, request.PageNumber, request.PageSize));
            }


            var userIds = pagedUsers.Select(u => u.Id).ToList();
            var userRolesDict = await _unitOfWork.Auth.GetRolesByUserIdsAsync(userIds, cancellationToken);

            var items = pagedUsers.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                AvatarUrl = u.AvatarUrl,
                IsActive = u.IsActive,
                Gender = u.Gender,
                HotelId = u.StaffProfile?.HotelId,
                Position = u.StaffProfile?.Position,
                Role = userRolesDict.TryGetValue(u.Id, out var role) ? role : "User"
            }).ToList();

            return Result.Success(new PagedResult<UserDto>(items, totalCount, request.PageNumber, request.PageSize));
        }
    }
}
