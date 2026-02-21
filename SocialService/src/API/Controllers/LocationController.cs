using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using SocialService.Application.DTOs.Locations;
using SocialService.Application.Features.Locations.Commands.CreateLocation;
using SocialService.Application.Features.Locations.Commands.DeleteLocation;
using SocialService.Application.Features.Locations.Commands.UpdateLocation;
using SocialService.Application.Features.Locations.Queries.GetLocationById;
using SocialService.Application.Features.Locations.Queries.GetNearbyLocations;
using SocialService.Application.Features.Locations.Queries.SearchLocations;
using SocialService.Domain.Common;
using SocialService.Domain.Enum;

namespace SocialService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LocationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tạo mới 1 địa điểm
        /// </summary>
        /// <remarks>
        /// - Chỉ người dùng hoặc admin tạo 
        /// -  người dùng đăng lên cần admin duyệt, admin đăng sẽ được duyệt trực tiếp
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.SysAdmin},{AppRoles.Customer}")]
        public async Task<IActionResult> Create([FromBody] CreateLocationCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(ApiResponse<string>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<Guid>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Xoá 1 địa điểm
        /// </summary>
        /// <remarks>
        /// - Chỉ người dùng hoặc admin xoá
        /// - Admin xoá được tất cả
        /// - Người dùng chỉ xoá đươc địa điểm mà họ đăng tải
        /// </remarks>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.SysAdmin},{AppRoles.Customer}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteLocationCommand { Id = id });
            if (result.IsFailure) return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Xem chi tiết 1 địa điểm
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetLocationByIdQuery { Id = id });
            if (result.IsFailure) return NotFound(ApiResponse<string>.ErrorResponse("Không tìm thấy địa điểm"));
            return Ok(ApiResponse<LocationDto>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Tìm kiếm địa điểm theo tên địa điểm hoăc địa chỉ có tìm theo loại hình địa điểm
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? keyword, [FromQuery] LocationType? type, [FromQuery] int pageIndex = 1)
        {
            var query = new SearchLocationsQuery { Keyword = keyword, Type = type, PageIndex = pageIndex };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<Domain.Common.Models.PagedResult<LocationDto>>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Tìm kiếm địa điểm xung quoanh
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearby([FromQuery] double lat, [FromQuery] double lon, [FromQuery] double radius = 5)
        {
            var query = new GetNearbyLocationsQuery
            {
                UserLatitude = lat,
                UserLongitude = lon,
                RadiusInKm = radius
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<LocationNearbyDto>>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Cập nhật lại thông tin địa điểm
        /// </summary>
        /// <remarks>
        /// - Chỉ người dùng hoặc admin được phép cập nhật
        /// - Admin sửa được tất cả
        /// - Người dùng chỉ sửa đươc địa điểm mà họ đăng tải
        /// </remarks>
        [HttpPut("update")]
        [Authorize(Roles = $"{AppRoles.SysAdmin},{AppRoles.Customer}")]
        public async Task<IActionResult> Update([FromBody] UpdateLocationCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
        }
    }
}
