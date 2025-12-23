using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel;
using HotelCatalogService.Application.Features.Hotel.Commands.ApproveHotel;
using HotelCatalogService.Application.Features.Hotel.Commands.CreateHotel;
using HotelCatalogService.Application.Features.Hotel.Commands.DeleteHotel;
using HotelCatalogService.Application.Features.Hotel.Commands.UpdateHotel;
using HotelCatalogService.Application.Features.Hotel.Queries.GetHotelsWithPagination;
using HotelCatalogService.Domain.Common;
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


        [HttpPost("{id}/approve")]
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

            /*if (currentRole == null)
            {
                query.IsActive = true;
                query.Status = HotelStatus.Active;
            }*/

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
