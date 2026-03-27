using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel;
using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Application.Features.Hotel.Commands;
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
using HotelCatalogService.Application.Features.Hotel.Queries.GetHotelDetail;
using HotelCatalogService.Application.Features.Hotel.Queries.GetHotelDetailBySlug;
using HotelCatalogService.Application.Features.Hotel.Queries.GetHotelsByIds;
using HotelCatalogService.Application.Features.Hotel.Queries.GetHotelStructure;
using HotelCatalogService.Application.Features.Hotel.Queries.GetHotelSummary;
using HotelCatalogService.Application.Features.Hotel.Queries.GetHotelsWithPagination;
using HotelCatalogService.Application.Features.Hotel.Queries.IsHotelExisting;
using HotelCatalogService.Application.Features.Hotel.Queries.IsHotelOwner;
using HotelCatalogService.Application.Features.Room.Commands.CheckInHotelRoom;
using HotelCatalogService.Application.Features.Room.Commands.RollbackCheckInRoom;
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

        /// <summary>
        /// Tạo khách sạn mới
        /// </summary>
        /// <remarks>
        /// - Chỉ HotelOwner được phép gọi
        /// - Mỗi HotelOwner có thể tạo nhiều khách sạn
        /// </remarks>
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
                Rating = request.Rating,
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

        /// <summary>
        /// Câp nhật thông tin khách sạn
        /// </summary>
        /// <remarks>
        /// - Chỉ HotelOwner được phép gọi
        /// </remarks>
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
                Rating = request.Rating,
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

        /// <summary>
        /// Xoá khách sạn
        /// </summary>
        /// <remarks>
        /// - Chỉ HotelOwner được phép gọi
        /// </remarks>
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

                return BadRequest(ApiResponse<string>.ErrorResponse("", new List<string> { result.Error.Message }));
            }

            return NoContent();
        }

        /// <summary>
        /// Admin phê duyệt khách sạn khi chủ khách sạn setup hoàn tất chuẩn bị vận hành
        /// </summary>
        /// <remarks>
        /// </remarks>
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

        /// <summary>
        /// Xem danh sách khách sạn
        /// </summary>
        /// <remarks>
        /// - Chỉ Admin và chủ khách sạn được phép gọi
        /// - Admin xem được tất cả, chủ khách sạn chỉ xem của họ
        /// 
        /// - status khách sạn để lọc
        /// Draft = 0,              - Nháp: Chủ khách sạn đang tạo, chưa xong, chưa muốn ai thấy.
        /// PendingApproval = 1,    - Chờ duyệt: Đã điền xong thông tin, gửi cho Admin duyệt (KYB).
        /// Active = 2,             - Hoạt động: Admin đã duyệt, khách có thể đặt phòng.
        /// Rejected = 3,           - Từ chối: Admin từ chối (do giấy tờ sai, v.v.).
        /// Suspended = 4,          - Bị đình chỉ: Admin khóa do vi phạm chính sách.
        /// TemporarilyClosed = 5   - Tạm đóng: Chủ khách sạn tự ẩn (sửa chữa, nghỉ đông, v.v.).
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.SysAdmin},{AppRoles.HotelOwner}")]
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

        /// <summary>
        /// Lấy toàn bộ cấu trúc Tòa nhà -> Tầng -> Phòng của Khách sạn
        /// </summary>
        /// <remarks>
        /// - Chỉ HotelOwner được phép gọi
        /// - fetch để có thể hiển thị giao diện tạo cấu trúc khách sạn nhanh (2.1), giam bớt nhầm lẫn khi tạo cấu trúc khách sạn
        /// </remarks>
        [HttpGet("{id:guid}/structure")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> GetHotelStructure(Guid id)
        {
            var query = new GetHotelStructureQuery(id);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(result.Error.Message));
            }

            return Ok(ApiResponse<List<BlockResponseDto>>.SuccessResponse(result.Value));
        }



        /// <summary>
        /// 2.1 Tạo cấu trúc khách sạn nhanh 
        /// </summary>
        /// <remarks>
        /// - Lưu cần tạo loại phòng ( 2.3 ) trước khi tạo cấu trúc khách sạn
        /// - Chỉ HotelOwner được phép gọi
        /// - Tạo nhanh dãy phòng, lầu, phòng
        /// </remarks>
        [HttpPost("create-structure")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> CreateHotelStructure([FromBody] AddHotelStructure request)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var command = new AddHotelStructureCommand
            {
                HotelId = request.HotelId,
                Blocks = request.Blocks,
                OwnerId = Guid.Parse(userId)
            };

            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("", new List<string> { result.Error.Message }));
            }
            return Ok(ApiResponse<object>.SuccessResponse(null, "Hotel structure created successfully."));
        }

        /// <summary>
        /// Tìm kiếm khách sạn (không cần đăng nhập)
        /// </summary>
        /// <remarks>
        /// </remarks>
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
        public async Task<IActionResult> CloseTemporarily(Guid id, [FromBody] HotelCloseTemporarily closeTemporarily)
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

        /// <summary>
        /// Lấy thông tin của khách sạn bằng ID (không cần map ra gateway)
        /// </summary>
        [HttpGet("{hotelId:Guid}/summary")]
        public async Task<IActionResult> GetHotelSummary(Guid hotelId)
        {
            var result = await _mediator.Send(new GetHotelSummaryQuery
            {
                HotelId = hotelId
            });

            if (result.IsFailure)
            {
                return NotFound(ApiResponse<HotelSummaryDto>.ErrorResponse(
                    message: result.Error.Message,
                    errors: new List<string> { result.Error.Code }
                ));
            }

            return Ok(ApiResponse<HotelSummaryDto>.SuccessResponse(result.Value));
        }


        /// <summary>
        /// Checkin phòng (chỉ lễ tân hoăc chủ khách sạn gọi)  không map ra gateway
        /// </summary>  
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.HotelOwner},{AppRoles.Receptionist}")]
        [Route("rooms/check-in")]
        public async Task<IActionResult> CheckInHotelRoom([FromBody] CheckInHotelRoomRequest request)
        {
            var userId = _currentUserService.UserId;
            var isReceptionist = User.IsInRole(AppRoles.Receptionist);

            Guid? userTokenHotelId = null;
            if (isReceptionist)
            {
                var hotelIdClaim = _currentUserService.HotelId;
                if (!string.IsNullOrEmpty(hotelIdClaim.ToString()))
                {
                    userTokenHotelId = hotelIdClaim;
                }
            }


            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var command = new CheckInHotelRoomCommand
            {
                HotelId = request.HotelId,
                RoomId = request.RoomId,
                CheckedInBy = Guid.Parse(userId),
                IsReceptionist = isReceptionist,
                UserTokenHotelId = userTokenHotelId
            };
            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "Hotel.Forbidden" => Forbid(),
                    "Hotel.NotFound" => NotFound(ApiResponse<object>.ErrorResponse(result.Error.Message)),
                    "Room.NotFound" => NotFound(ApiResponse<object>.ErrorResponse(result.Error.Message)),
                    _ => BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message))
                };
            }
            return Ok(ApiResponse<RoomResponse>.SuccessResponse(result.Value));

        }

        /// <summary>
        /// Trả lai phòng nếu checkin nhầm (chỉ lễ tân hoăc chủ khách sạn gọi)  không map ra gateway
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.HotelOwner},{AppRoles.Receptionist}")]
        [Route("rooms/rollback-check-in")]
        public async Task<IActionResult> RollbackCheckInRoom([FromBody] RollbackCheckInRoomRequest rollback, CancellationToken token)
        {
            var command = new RollbackCheckInRoomCommand
            {
                HotelId = rollback.HotelId,
                RoomId = rollback.RoomId
            };
            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "Hotel.Forbidden" => Forbid(),
                    "Hotel.NotFound" => NotFound(ApiResponse<object>.ErrorResponse(result.Error.Message)),
                    "Room.NotFound" => NotFound(ApiResponse<object>.ErrorResponse(result.Error.Message)),
                    _ => BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message))
                };
            }
            return Ok(ApiResponse<object>.SuccessResponse("Rollback room success"));
        }

        /// <summary>
        /// Kiểm tra khách sạn có tồn tại hay không (không map ra gateway)
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("hotel-existing")]
        public async Task<IActionResult> IsHotelExisting([FromQuery] Guid hotelId)
        {
            var request = new IsHotelExistingQuery
            {
                HotelId = hotelId
            };

            var result = await _mediator.Send(request);

            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// xem thông tin chi tiết của 1 khách sạn (không cần đăng nhập)
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("detail")]
        public async Task<IActionResult> GetHotelDetail([FromQuery] Guid hotelId)
        {
            var request = new GetHotelDetailQuery
            {
                HotelId = hotelId
            };

            var result = await _mediator.Send(request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<HotelDetailDto>.SuccessResponse(result.Value));
            }
            return NotFound(ApiResponse<HotelDetailDto>.ErrorResponse(result.Error.Message.ToString()));
        }

        /// <summary>
        /// Xem thông tin chi tiết của 1 khách sạn theo Slug (chuẩn SEO, không cần đăng nhập)
        /// </summary>
        [HttpGet("detail/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelDetailBySlug(string slug)
        {
            var request = new GetHotelDetailBySlugQuery
            {
                Slug = slug
            };

            var result = await _mediator.Send(request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<HotelDetailDto>.SuccessResponse(result.Value));
            }
            return NotFound(ApiResponse<HotelDetailDto>.ErrorResponse(result.Error.Message.ToString()));
        }

        /// <summary>
        /// Xem người dùng có phải là người sở hữu khách sạn hay không - yêu cầu đăng nhập (không map ra gateway)
        /// </summary>
        [HttpGet("is-hotel-owner")]
        [Authorize]
        public async Task<IActionResult> IsHotelOwner([FromQuery] Guid hotelId)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var request = new IsHotelOwnerQuery
            {
                HotelId = hotelId,
                UserId = Guid.Parse(userId)
            };
            var result = await _mediator.Send(request);
            return Ok(ApiResponse<bool>.SuccessResponse(result));
        }

        /// <summary>
        /// Lấy danh sách khách sạn theo id dùng cho viêc recommend, không cần đăng nhập
        /// </summary>
        [HttpPost("by-ids")]
        public async Task<IActionResult> GetHotelsByIds([FromBody] List<Guid> ids)
        {
            var query = new GetHotelsByIdsQuery(ids);
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<HotelSummaryDtoo>>.SuccessResponse(result.Value));
        }

        /*/// <summary>
        /// Đồng bộ lại dữ liệu các khách sạn đang Active sang Vector DB (Qdrant)
        /// Sử dụng khi đổi model embedding (ví dụ sang bge-m3)
        /// </summary>
        [HttpPost("sync-vector-db")]
        public async Task<IActionResult> SyncHotelsToVectorDb(CancellationToken cancellationToken)
        {
            var command = new SyncHotelsToVectorDbCommand();

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(new
            {
                Success = true,
                Message = "Đã trigger thành công event đồng bộ các khách sạn đang Active sang Qdrant!"
            });
        }*/
    }
}

