using BookingService.Application.Contracts;
using BookingService.Application.DTOs.HotelCatalog;
using BookingService.Infrastructure.Protos;
using Grpc.Core;

namespace BookingService.Infrastructure.Services
{
    public class HotelCatalogApiClient : IHotelCatalogService
    {
        private readonly CatalogGrpc.CatalogGrpcClient _grpcClient;

        public HotelCatalogApiClient(CatalogGrpc.CatalogGrpcClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public async Task<PromotionApplyResult> ApplyPromotionAsync(Guid hotelId, string code, decimal orderAmount, Guid userId, Guid bookingId, CancellationToken token = default)
        {
            try
            {
                var request = new ApplyPromotionRequest
                {
                    HotelId = hotelId.ToString(),
                    Code = code,
                    OrderAmount = (double)orderAmount, 
                    UserId = userId.ToString(),
                    BookingId = bookingId.ToString()
                };

                var response = await _grpcClient.ApplyPromotionAsync(request, cancellationToken: token);

                return new PromotionApplyResult
                {
                    IsSuccess = response.IsSuccess,
                    DiscountAmount = (decimal)response.DiscountAmount,
                    Message = response.Message
                };
            }
            catch (RpcException ex)
            {
                return new PromotionApplyResult { IsSuccess = false, Message = ex.Status.Detail ?? "Error applying discount code." };
            }
        }

        public async Task<List<CatalogBatchPriceDto>> GetBatchRoomPricesAsync(Guid hotelId, DateOnly fromDate, DateOnly toDate, CancellationToken token = default)
        {
            try
            {
                var request = new GetBatchRoomPricesRequest
                {
                    HotelId = hotelId.ToString(),
                    FromDate = fromDate.ToString("yyyy-MM-dd"),
                    ToDate = toDate.ToString("yyyy-MM-dd")
                };

                var response = await _grpcClient.GetBatchRoomPricesAsync(request, cancellationToken: token);

                var result = new List<CatalogBatchPriceDto>();
                foreach (var priceMsg in response.Prices)
                {
                    var dto = new CatalogBatchPriceDto
                    {
                        RoomTypeId = Guid.Parse(priceMsg.RoomTypeId),
                        RoomTypeName = priceMsg.RoomTypeName,
                        BasePrice = (decimal)priceMsg.BasePrice,
                        Calendar = priceMsg.Calendar.Select(c => new CatalogDailyPriceDto
                        {
                            Date = DateTime.Parse(c.Date),
                            Price = (decimal)c.Price,
                            IsSpecialPrice = c.IsSpecialPrice
                        }).ToList()
                    };

                    if (priceMsg.CancellationPolicy != null)
                    {
                        dto.CancellationPolicy = new CancellationPolicyDto
                        {
                            Id = Guid.Parse(priceMsg.CancellationPolicy.Id),
                            HotelId = Guid.Parse(priceMsg.CancellationPolicy.HotelId),
                            Name = priceMsg.CancellationPolicy.Name,
                            Type = priceMsg.CancellationPolicy.Type,
                            Description = priceMsg.CancellationPolicy.Description,
                            Rules = priceMsg.CancellationPolicy.Rules.Select(r => new CancellationRuleDto
                            {
                                Id = Guid.Parse(r.Id),
                                HoursBeforeCheckIn = r.HoursBeforeCheckIn,
                                RefundPercentage = (decimal)r.RefundPercentage
                            }).ToList()
                        };
                    }
                    result.Add(dto);
                }

                return result;
            }
            catch (RpcException)
            {
                return new List<CatalogBatchPriceDto>();
            }
        }

        public async Task<HotelSummaryDto?> GetHotelSummary(Guid hotelId, CancellationToken token = default)
        {
            try
            {
                var request = new GetHotelSummaryRequest { HotelId = hotelId.ToString() };
                var response = await _grpcClient.GetHotelSummaryAsync(request, cancellationToken: token);

                return new HotelSummaryDto
                {
                    HotelName = response.HotelName,
                    OwnerId = Guid.Parse(response.OwnerId),
                    Street = response.Street,
                    City = response.City,
                    Country = response.Country,
                    Status = response.Status
                };
            }
            catch (RpcException)
            {
                return null;
            }
        }

        public async Task<PromotionValidationResult> ValidatePromotionAsync(Guid hotelId, string code, decimal totalBaseAmount, Guid userId, CancellationToken token = default)
        {
            try
            {
                var request = new ValidatePromotionRequest
                {
                    HotelId = hotelId.ToString(),
                    Code = code,
                    TotalBaseAmount = (double)totalBaseAmount,
                    UserId = userId.ToString()
                };

                var response = await _grpcClient.ValidatePromotionAsync(request, cancellationToken: token);

                return new PromotionValidationResult
                {
                    IsValid = response.IsValid,
                    PromotionId = Guid.Parse(response.PromotionId),
                    DiscountAmount = (decimal)response.DiscountAmount,
                    Message = response.Message
                };
            }
            catch (RpcException ex)
            {
                return new PromotionValidationResult
                {
                    IsValid = false,
                    Message = ex.Status.Detail ?? "Catalog Service Connection Error."
                };
            }
        }

        public async Task<RoomResponse?> CheckInRoom(Guid hotelId, Guid roomId, Guid checkInBy, CancellationToken token = default) 
        {
            try
            {
                var request = new CheckInRoomRequest
                {
                    HotelId = hotelId.ToString(),
                    RoomId = roomId.ToString(),
                    CheckInBy = checkInBy.ToString()
                };

                var response = await _grpcClient.CheckInRoomAsync(request, cancellationToken: token);

                return new RoomResponse
                {
                    RoomId = Guid.Parse(response.RoomId),
                    RoomTypeId = Guid.Parse(response.RoomTypeId),
                    RoomName = response.RoomName,
                    Status = response.Status
                };
            }
            catch (RpcException)
            {
                return null;
            }
        }

        public async Task RollbackCheckInRoom(Guid hotelId, Guid roomId, CancellationToken token)
        {
            var request = new RollbackCheckInRoomRequest
            {
                HotelId = hotelId.ToString(),
                RoomId = roomId.ToString()
            };

            await _grpcClient.RollbackCheckInRoomAsync(request, cancellationToken: token);
        }

        public async Task<bool> VerifyHotelOwnershipAsync(Guid hotelId, CancellationToken cancellationToken)
        {
            try
            {
                var request = new VerifyHotelOwnershipRequest { HotelId = hotelId.ToString() };
                var response = await _grpcClient.VerifyHotelOwnershipAsync(request, cancellationToken: cancellationToken);

                return response.IsOwner;
            }
            catch (RpcException)
            {
                return false;
            }
        }
    }
}
