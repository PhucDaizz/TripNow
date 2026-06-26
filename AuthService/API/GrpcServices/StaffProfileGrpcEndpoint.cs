using Api.Protos;
using Application.Features.StaffProfile.Queries.GetStaffProfile;
using Domain.Common;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.GrpcServices
{
    [Authorize(Roles = $"{AppRoles.SysAdmin},{AppRoles.HotelOwner},{AppRoles.Housekeeping},{AppRoles.Receptionist}")]
    public class StaffProfileGrpcEndpoint : StaffProfileGrpc.StaffProfileGrpcBase
    {
        private readonly IMediator _mediator;

        public StaffProfileGrpcEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<GetStaffProfileResponse> GetStaffProfile(
            GetStaffProfileRequest request,
            ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();

            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid Token or UserId missing."));
            }

            var result = await _mediator.Send(new GetStaffProfileQuery { UserId = userId });

            if (result.IsFailure)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Employee information not found."));
            }

            return new GetStaffProfileResponse
            {
                Id = result.Value.Id.ToString(),
                UserId = result.Value.UserId.ToString(), 
                HotelId = result.Value.HotelId.ToString(),
                Position = result.Value.Position
            };
        }
    }
} 
