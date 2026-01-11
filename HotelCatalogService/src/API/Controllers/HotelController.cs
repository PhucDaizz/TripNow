using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel;
using HotelCatalogService.Application.Features.Hotel.Commands.AddHotelStructure;
using HotelCatalogService.Application.Features.Hotel.Commands.ApproveHotel;
using HotelCatalogService.Application.Features.Hotel.Commands.CloseTemporarilyHotel;
using HotelCatalogService.Application.Features.Hotel.Commands.CreateHotel;
using HotelCatalogService.Application.Features.Hotel.Commands.DeleteHotel;
using HotelCatalogService.Application.Features.Hotel.Commands.RejectHotel;
using HotelCatalogService.Application.Features.Hotel.Commands.ReopenHotel;
using HotelCatalogService.Application.Features.Hotel.Commands.SubmitForApprovalHotel;
using HotelCatalogService.Application.Features.Hotel.Commands.SuspendHotel;
using HotelCatalogService.Application.Features.Hotel.Commands.UpdateHotel;
using HotelCatalogService.Application.Features.Hotel.Queries.GetHotelsWithPagination;
using HotelCatalogService.Domain.Common;
using HotelCatalogService.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using System.Security.Claims;

namespace HotelCatalogService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public HotelController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Create([FromBody] CreateHotelRequest request)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var command = new CreateHotelCommand
            {
                OwnerId = Guid.Parse(userId),
                Name = request.Name,
                Description = request.Description,
                Street = request.Street,
                City = request.City,
                Country = request.Country,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("", new List<string> { result.Error.Message }));
            }

            return Ok(ApiResponse<object>.SuccessResponse(new { id = result.Value }));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHotelRequest request)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var command = new UpdateHotelCommand
            {
                HotelId = id,
                OwnerId = Guid.Parse(userId),
                Name = request.Name,
                Description = request.Description,
                Street = request.Street,
                City = request.City,
                Country = request.Country,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                if (result.Error.Code == "Hotel.Forbidden")
                    return Forbid();

                return BadRequest(ApiResponse<string>.ErrorResponse("", new List<string> { result.Error.Message }));
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var command = new DeleteHotelCommand
            {
                HotelId = id,
                OwnerId = Guid.Parse(userId) 
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                if (result.Error.Code == "Hotel.Forbidden") return Forbid();
                if (result.Error.Code == "Hotel.NotFound") return NotFound();

                return BadRequest(ApiResponse<string>.ErrorResponse("", new List<string> { result.Error.Message}));
            }

            return NoContent();
        }


        [HttpPost("{id}/status/approve")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")] 
        public async Task<IActionResult> Approve(Guid id)
        {
            var command = new ApproveHotelCommand
            {
                HotelId = id
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(ApiResponse<string>.SuccessResponse(null, "Hotel approved successfully and notification sent."));
        }

        [HttpGet]
        [Authorize(Roles =$"{AppRoles.SysAdmin},{AppRoles.HotelOwner}")]
        public async Task<IActionResult> GetHotels([FromQuery] GetHotelsWithPaginationQuery query)
        {
            var currentRole = User.FindFirstValue(ClaimTypes.Role);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentRole == AppRoles.HotelOwner)
            {
                query.OwnerId = Guid.Parse(currentUserId); 
            }
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpPost("create-structure")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> CreateHotelStructure([FromBody]AddHotelStructure request)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var command = new AddHotelStructureCommand
            {
                HotelId = request.HotelId,
                Blocks = request.Blocks,
                OwnerId =  Guid.Parse(userId)
            };

            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("", new List<string> { result.Error.Message }));
            }
            return Ok(ApiResponse<object>.SuccessResponse(null, "Hotel structure created successfully."));
        }


        [HttpGet("search")]
        [AllowAnonymous] 
        public async Task<IActionResult> SearchHotels([FromQuery] GetHotelsWithPaginationQuery query)
        {
            query.Status = HotelStatus.Active;

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Chủ khách sạn gửi yêu cầu duyệt (Status: Draft -> Pending)
        /// </summary>
        [HttpPost("{id}/status/submit-for-approval")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> SubmitForApproval(Guid id)
        {
            var command = new SubmitForApprovalCommand
            {
                HotelId = id,
                OwerId = Guid.Parse(_currentUserService.UserId)
            };

            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    message: result.Error.Message,
                    errors: new List<string> { result.Error.Code }
                ));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Your hotel is awaiting approval."));
        }

        /// <summary>
        /// Chủ khách sạn tạm đóng cửa (Status: Active -> TemporarilyClosed)
        /// </summary>
        [HttpPost("{id}/status/temporarily-close")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> CloseTemporarily(Guid id, [FromBody]HotelCloseTemporarily closeTemporarily)
        {
            var command = new CloseTemporarilyHotelCommand
            {
                HotelId = id,
                OwerId = Guid.Parse(_currentUserService.UserId),
                FromDate = closeTemporarily.FromDate,
                ToDate = closeTemporarily.ToDate
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    message: result.Error.Message,
                    errors: new List<string> { result.Error.Code }
                ));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Hotel closed temporarily successfully."));
        }

        /// <summary>
        /// Chủ khách sạn mở lại (Status: TemporarilyClosed -> Active)
        /// </summary>
        [HttpPost("{id}/status/reopen")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Reopen(Guid id)
        {
            var command = new ReopenHotelCommand
            {
                HotelId = id,
                OwerId = Guid.Parse(_currentUserService.UserId)
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    message: result.Error.Message,
                    errors: new List<string> { result.Error.Code }
                ));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Hotel reopened successfully."));
        }

        /// <summary>
        /// Admin từ chối duyệt (Status: Pending -> Rejected)
        /// </summary>
        [HttpPost("{id}/status/reject")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] ReasonRequest request)
        {
            var command = new RejectHotelCommand
            {
                HotelId = id,
                AdminId = Guid.Parse(_currentUserService.UserId),
                Reason = request.Reason
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    message: result.Error.Message,
                    errors: new List<string> { result.Error.Code }
                ));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Hotel rejected successfully."));
        }

        /// <summary>
        /// Admin khóa khách sạn vi phạm (Status: Any -> Suspended)
        /// </summary>
        [HttpPost("{id}/status/suspend")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> Suspend(Guid id, [FromBody] ReasonRequest request)
        {
            var command = new SuspendHotelCommand
            {
                HotelId = id,
                AdminId = Guid.Parse(_currentUserService.UserId),
                Reason = request.Reason
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    message: result.Error.Message,
                    errors: new List<string> { result.Error.Code }
                ));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Hotel suspended successfully."));
        }

    }
}
