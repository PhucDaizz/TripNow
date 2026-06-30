using BookingService.Api.Protos;
using Grpc.Core;
using HotelCatalogService.Application.Features.Hotel.Queries.GetHotelDetail;
using HotelCatalogService.Application.Features.Hotel.Queries.GetHotelSummary;
using HotelCatalogService.Application.Features.Hotel.Queries.IsHotelExisting;
using HotelCatalogService.Application.Features.Hotel.Queries.IsHotelOwner;
using HotelCatalogService.Application.Features.Promotion.Commands.ApplyPromotion;
using HotelCatalogService.Application.Features.Promotion.Queries.CheckPromotion;
using HotelCatalogService.Application.Features.Room.Commands.CheckInHotelRoom;
using HotelCatalogService.Application.Features.Room.Commands.RollbackCheckInRoom;
using HotelCatalogService.Application.Features.RoomPrice.Queries.GetHotelBatchRoomPrices;
using HotelCatalogService.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HotelCatalogService.API.GrpcServices
{
    public class CatalogGrpcEndpoint : CatalogGrpc.CatalogGrpcBase
    {
        private readonly IMediator _mediator;

        public CatalogGrpcEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<ApplyPromotionResponse> ApplyPromotion(
            ApplyPromotionRequest request,
            ServerCallContext context)
        {
            var command = new ApplyPromotionCommand
            {
                HotelId = Guid.Parse(request.HotelId),
                Code = request.Code,
                UserId = Guid.Parse(request.UserId),
                BookingId = Guid.Parse(request.BookingId),
                OrderAmount = (decimal)request.OrderAmount
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return new ApplyPromotionResponse
                {
                    IsSuccess = true,
                    DiscountAmount = (double)result.Value,
                    Message = "Code applied successfully."
                };
            }

            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Error.Message));
        }

        public override async Task<GetBatchRoomPricesResponse> GetBatchRoomPrices(
            GetBatchRoomPricesRequest request,
            ServerCallContext context)
        {
            if (!DateTime.TryParse(request.FromDate, out DateTime fromDate) ||
                !DateTime.TryParse(request.ToDate, out DateTime toDate))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid date format. Please use yyyy-MM-dd."));
            }

            var query = new GetHotelBatchRoomPricesQuery
            {
                HotelId = Guid.Parse(request.HotelId),
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await _mediator.Send(query);

            if (result.IsFailure) 
            {
                throw new RpcException(new Status(StatusCode.NotFound, result.Error.Message ?? "Room prices not found."));
            }

            var response = new GetBatchRoomPricesResponse();

            foreach (var dto in result.Value) 
            {
                var batchPriceMsg = new CatalogBatchPriceMessage
                {
                    RoomTypeId = dto.RoomTypeId.ToString(),
                    RoomTypeName = dto.RoomTypeName,
                    BasePrice = (double)dto.BasePrice
                };

                if (dto.Calendar != null)
                {
                    foreach (var day in dto.Calendar)
                    {
                        batchPriceMsg.Calendar.Add(new CatalogDailyPriceMessage
                        {
                            Date = day.Date.ToString("yyyy-MM-dd"), 
                            Price = (double)day.Price,
                            IsSpecialPrice = day.IsSpecialPrice
                        });
                    }
                }

                if (dto.CancellationPolicy != null)
                {
                    var policyMsg = new CancellationPolicyMessage
                    {
                        Id = dto.CancellationPolicy.Id.ToString(),
                        HotelId = dto.CancellationPolicy.HotelId.ToString(),
                        Name = dto.CancellationPolicy.Name,
                        Type = dto.CancellationPolicy.Type,
                        Description = dto.CancellationPolicy.Description
                    };

                    if (dto.CancellationPolicy.Rules != null)
                    {
                        foreach (var rule in dto.CancellationPolicy.Rules)
                        {
                            policyMsg.Rules.Add(new CancellationRuleMessage
                            {
                                Id = rule.Id.ToString(),
                                HoursBeforeCheckIn = rule.HoursBeforeCheckIn,
                                RefundPercentage = (double)rule.RefundPercentage
                            });
                        }
                    }

                    batchPriceMsg.CancellationPolicy = policyMsg;
                }

                response.Prices.Add(batchPriceMsg);
            }

            return response;
        }

        public override async Task<GetHotelSummaryResponse> GetHotelSummary(
            GetHotelSummaryRequest request,
            ServerCallContext context)
        {
            var hotelId = Guid.Parse(request.HotelId);

            var result = await _mediator.Send(new GetHotelSummaryQuery
            {
                HotelId = hotelId
            });

            if (result.IsFailure)
            {
                throw new RpcException(new Status(StatusCode.NotFound, result.Error.Message ?? "Hotel summary not found."));
            }

            return new GetHotelSummaryResponse
            {
                HotelName = result.Value.HotelName,
                OwnerId = result.Value.OwnerId.ToString(), 
                Street = result.Value.Street,
                City = result.Value.City,
                Country = result.Value.Country,
                Status = result.Value.Status
            };
        }

        public override async Task<ValidatePromotionResponse> ValidatePromotion(
            ValidatePromotionRequest request,
            ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out Guid userId) || userId == Guid.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId is required."));
            }

            var query = new CheckPromotionQuery
            {
                HotelId = Guid.Parse(request.HotelId),
                Code = request.Code,
                UserId = userId,
                OrderAmount = (decimal)request.TotalBaseAmount
            };

            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                return new ValidatePromotionResponse
                {
                    IsValid = true,
                    DiscountAmount = (double)result.Value.FinalDiscountAmount,
                    PromotionId = result.Value.PromotionId.ToString(),
                    Message = "Success"
                };
            }

            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Error.Message ?? "Invalid promotion code."));
        }

        [Authorize(Roles = $"{AppRoles.HotelOwner},{AppRoles.Receptionist}")]
        public override async Task<CheckInRoomResponse> CheckInRoom(
            CheckInRoomRequest request,
            ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            var user = httpContext.User;

            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated."));
            }

            var isReceptionist = user.IsInRole(AppRoles.Receptionist); 

            Guid? userTokenHotelId = null;
            if (isReceptionist)
            {
                var hotelIdClaim = user.FindFirstValue("HotelId") ?? user.FindFirstValue("hotelId");
                if (!string.IsNullOrEmpty(hotelIdClaim) && Guid.TryParse(hotelIdClaim, out Guid parsedHotelId))
                {
                    userTokenHotelId = parsedHotelId;
                }
            }

            var command = new CheckInHotelRoomCommand
            {
                HotelId = Guid.Parse(request.HotelId),
                RoomId = Guid.Parse(request.RoomId),
                CheckedInBy = Guid.Parse(userIdStr), 
                IsReceptionist = isReceptionist,
                UserTokenHotelId = userTokenHotelId
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                var statusCode = result.Error.Code switch
                {
                    "Hotel.Forbidden" => StatusCode.PermissionDenied, 
                    "Hotel.NotFound" => StatusCode.NotFound,          
                    "Room.NotFound" => StatusCode.NotFound,
                    _ => StatusCode.InvalidArgument                     
                };

                throw new RpcException(new Status(statusCode, result.Error.Message));
            }

            return new CheckInRoomResponse
            {
                RoomId = result.Value.RoomId.ToString(),
                RoomTypeId = result.Value.RoomTypeId.ToString(),
                RoomName = result.Value.RoomName,
                Status = result.Value.Status
            };
        }

        [Authorize(Roles = $"{AppRoles.HotelOwner},{AppRoles.Receptionist}")]
        public override async Task<RollbackCheckInRoomResponse> RollbackCheckInRoom(
            RollbackCheckInRoomRequest request,
            ServerCallContext context)
        {
            var command = new RollbackCheckInRoomCommand
            {
                HotelId = Guid.Parse(request.HotelId),
                RoomId = Guid.Parse(request.RoomId)
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                var statusCode = result.Error.Code switch
                {
                    "Hotel.Forbidden" => StatusCode.PermissionDenied, 
                    "Hotel.NotFound" => StatusCode.NotFound,          
                    "Room.NotFound" => StatusCode.NotFound,           
                    _ => StatusCode.InvalidArgument                     
                };

                throw new RpcException(new Status(statusCode, result.Error.Message));
            }

            return new RollbackCheckInRoomResponse
            {
                IsSuccess = true
            };
        }

        [Authorize] 
        public override async Task<VerifyHotelOwnershipResponse> VerifyHotelOwnership(
            VerifyHotelOwnershipRequest request,
            ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr))
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated."));
            }

            var query = new IsHotelOwnerQuery
            {
                HotelId = Guid.Parse(request.HotelId),
                UserId = Guid.Parse(userIdStr) 
            };

            var result = await _mediator.Send(query);

            return new VerifyHotelOwnershipResponse
            {
                IsOwner = result
            };
        }

        public override async Task<IsHotelExistingResponse> IsHotelExisting(
            IsHotelExistingRequest request,
            ServerCallContext context)
        {
            var query = new IsHotelExistingQuery
            {
                HotelId = Guid.Parse(request.HotelId)
            };

            var result = await _mediator.Send(query);

            return new IsHotelExistingResponse
            {
                IsExisting = result.Value 
            };
        }

        public override async Task<GetHotelDetailResponse> GetHotelDetail(
            GetHotelDetailRequest request,
            ServerCallContext context)
        {
            var query = new GetHotelDetailQuery
            {
                HotelId = Guid.Parse(request.HotelId)
            };

            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                var dto = result.Value;

                var response = new GetHotelDetailResponse
                {
                    Id = dto.Id.ToString(),
                    OwnerId = dto.OwnerId.ToString(),
                    Name = dto.Name ?? string.Empty,
                    Follower = dto.Follower,
                    Slug = dto.Slug ?? string.Empty,
                    Description = dto.Description ?? string.Empty,
                    AddressStreet = dto.AddressStreet ?? string.Empty,
                    AddressCity = dto.AddressCity ?? string.Empty,
                    Status = dto.Status ?? string.Empty,
                    Rating = (double)dto.Rating, 
                    Thumbnail = dto.Thumbnail ?? string.Empty
                };

                if (dto.DistanceKm.HasValue)
                {
                    response.DistanceKm = dto.DistanceKm.Value;
                }

                if (dto.Location != null)
                {
                    response.Location = new CoordinatesMessage
                    {
                        Latitude = dto.Location.Latitude,
                        Longitude = dto.Location.Longitude
                    };
                }

                return response;
            }

            throw new RpcException(new Status(StatusCode.NotFound, result.Error.Message ?? "Hotel not found."));
        }
    }
}
