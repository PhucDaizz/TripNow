using HotelCatalogService.Application.DTOs.Amenity;
using HotelCatalogService.Application.Features.Amenity.Commands.CreateAmenity;
using HotelCatalogService.Application.Features.Amenity.Commands.DeleteAmenity;
using HotelCatalogService.Application.Features.Amenity.Commands.UpdateAmenity;
using HotelCatalogService.Application.Features.Amenity.Queries.GetAllAmenities;
using HotelCatalogService.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmenityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AmenityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllAmenitiesQuery());
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<List<AmenityDto>>.SuccessResponse(result.Value));
            }
            return BadRequest(ApiResponse<List<AmenityDto>>.ErrorResponse(result.Error.Message));
        }

        [HttpPost]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> Create([FromBody] CreateAmenityCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
            }

            var response = ApiResponse<Guid>.SuccessResponse(result.Value, "Successfully created the Amenity");
            return CreatedAtAction(nameof(GetAll), new { id = result.Value }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAmenityCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("The ID in the URL and the body do not match."));
            }

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
            }
            return Ok(ApiResponse<object>.SuccessResponse(null, "Update successful"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteAmenityCommand(id));

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Deleted successfully"));
        }
    }
}
