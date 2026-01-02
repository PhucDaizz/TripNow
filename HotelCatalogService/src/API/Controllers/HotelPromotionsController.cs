using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Promotion;
using HotelCatalogService.Application.Features.Promotion.Commands.ApplyPromotion;
using HotelCatalogService.Application.Features.Promotion.Commands.ChangePromotionStatus;
using HotelCatalogService.Application.Features.Promotion.Commands.CreatePromotion;
using HotelCatalogService.Application.Features.Promotion.Commands.DeletePromotion;
using HotelCatalogService.Application.Features.Promotion.Commands.UpdatePromotion;
using HotelCatalogService.Application.Features.Promotion.Queries.CheckPromotion;
using HotelCatalogService.Application.Features.Promotion.Queries.GetAvailablePromotions;
using HotelCatalogService.Application.Features.Promotion.Queries.GetPromotionsByHotel;
using HotelCatalogService.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.API.Controllers
{
    [Route("api/Hotel/{hotelId}/promotions")]
    [ApiController]
    public class HotelPromotionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser; 

        public HotelPromotionsController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        [HttpGet("manage")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> GetAll(
            Guid hotelId,
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = new GetPromotionsByHotelQuery
            {
                HotelId = hotelId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Ok(ApiResponse<PagedResult<PromotionDto>>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Dánh cho người dùng kiểm tra các mã khuyến mãi khả dụng
        /// </summary>
        [HttpGet("available")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetAvailablePromotions(Guid hotelId)
        {
            var result = await _mediator.Send(new GetAvailablePromotionsQuery { HotelId = hotelId });

            return Ok(ApiResponse<List<UserPromotionDto>>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Người dùng kiểm tra mã giảm giá có hợp lệ không
        /// </summary>
        [HttpPost("check")]
        [Authorize]
        public async Task<IActionResult> CheckPromotion(Guid hotelId, [FromBody] CheckPromotionRequest request)
        {
            var query = new CheckPromotionQuery
            {
                HotelId = hotelId,
                Code = request.Code,
                UserId = Guid.Parse(_currentUser.UserId), 
                OrderAmount = request.OrderAmount
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess
                ? Ok(ApiResponse<PromotionDiscountDto>.SuccessResponse(result.Value))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// API dành cho Booking Service gọi để áp dụng mã giảm giá và trừ kho. (Không map ra api gateway)
        /// </summary>
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyPromotion(Guid hotelId, [FromBody] ApplyPromotionRequest request)
        {
            var command = new ApplyPromotionCommand
            {
                HotelId = hotelId,
                Code = request.Code,
                UserId = request.UserId,
                BookingId = request.BookingId,
                OrderAmount = request.OrderAmount
            };
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<decimal>.SuccessResponse(result.Value, "Code applied successfully."));
            }
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        [HttpPost]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Create(Guid hotelId, [FromBody] CreatePromotionRequest req)
        {
            var command = new CreatePromotionCommand
            {
                HotelId = hotelId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                Code = req.Code,
                DiscountType = req.DiscountType,
                DiscountValue = req.DiscountValue,
                StartDate = req.StartDate,
                EndDate = req.EndDate,
                Quantity = req.Quantity,
                MinBookingAmount = req.MinBookingAmount
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<Guid>.SuccessResponse(result.Value))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        [HttpPut("{promoId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Update(Guid hotelId, Guid promoId, [FromBody] UpdatePromotionRequest req)
        {
            var command = new UpdatePromotionCommand
            {
                HotelId = hotelId,
                PromotionId = promoId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                Code = req.Code,
                DiscountType = req.DiscountType,
                DiscountValue = req.DiscountValue,
                StartDate = req.StartDate,
                EndDate = req.EndDate,
                Quantity = req.NewQuantity,
                MinBookingAmount = req.MinBookingAmount
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Update successful"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        [HttpPatch("{promoId}/status")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> ChangeStatus(Guid hotelId, Guid promoId, [FromBody] ChangeStatusRequest req)
        {
            var command = new ChangePromotionStatusCommand
            {
                HotelId = hotelId,
                PromotionId = promoId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                IsActive = req.IsActive
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, req.IsActive ? "On" : "Off"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        [HttpDelete("{promoId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Delete(Guid hotelId, Guid promoId)
        {
            var command = new DeletePromotionCommand
            {
                HotelId = hotelId,
                PromotionId = promoId,
                OwnerId = Guid.Parse(_currentUser.UserId)
            };

            var result = await _mediator.Send(command);

            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "The promotional code has been removed."))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }



    }
}
