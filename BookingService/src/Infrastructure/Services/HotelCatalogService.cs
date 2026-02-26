using BookingService.Application.Contracts;
using BookingService.Application.DTOs.HotelCatalog;
using Microsoft.AspNetCore.Http;
using Nexus.BuildingBlocks.Model;
using System.Net.Http.Json;

namespace BookingService.Infrastructure.Services
{
    public class HotelCatalogService : IHotelCatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HotelCatalogService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PromotionApplyResult> ApplyPromotionAsync(Guid hotelId, string code, decimal orderAmount, Guid userId, Guid bookingId, CancellationToken token = default)
        {
            var requestBody = new
            {
                Code = code,
                UserId = userId,
                BookingId = bookingId,
                OrderAmount = orderAmount
            };

            var response = await _httpClient.PostAsJsonAsync($"api/Hotel/{hotelId}/promotions/apply", requestBody, token);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<decimal>>(cancellationToken: token);
                if (apiResponse != null && apiResponse.Success)
                {
                    return new PromotionApplyResult
                    {
                        IsSuccess = true,
                        DiscountAmount = apiResponse.Data, 
                        Message = "Success"
                    };
                }
            }

            var errorMsg = "Error applying discount code.";
            try
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(cancellationToken: token);
                errorMsg = errorResponse?.Message ?? errorMsg;
            }
            catch { }

            return new PromotionApplyResult { IsSuccess = false, Message = errorMsg };
        }

        public async Task<List<CatalogBatchPriceDto>> GetBatchRoomPricesAsync(Guid hotelId, DateOnly fromDate, DateOnly toDate, CancellationToken token = default)
        {
            string fromStr = fromDate.ToString("yyyy-MM-dd");
            string toStr = toDate.ToString("yyyy-MM-dd");

            var response = await _httpClient.GetAsync($"/api/Hotel/{hotelId}/room-types/prices?from={fromStr}&to={toStr}", token);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<CatalogBatchPriceDto>>>(cancellationToken: token);
                if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                {
                    return apiResponse.Data; 
                }
            }

            return new List<CatalogBatchPriceDto>();
        }

        public async Task<HotelSummaryDto?> GetHotelSummary(Guid hotelId, CancellationToken token = default)
        {
            var response = await _httpClient.GetAsync($"/api/Hotel/{hotelId}/summary");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<HotelSummaryDto?>>(cancellationToken: token);
                if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                {
                    return apiResponse.Data;
                }
            }

            return null;
        }

        public async Task<PromotionValidationResult> ValidatePromotionAsync(Guid hotelId, string code, decimal totalBaseAmount, Guid userId, CancellationToken token = default)
        {
            var context = _httpContextAccessor.HttpContext;
            var bearerToken = context?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(bearerToken))
            {
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", bearerToken);
            }

            var requestBody = new CheckPromotionRequestDto
            {
                Code = code,
                OrderAmount = totalBaseAmount,
                UserId = userId
            };


            var response = await _httpClient.PostAsJsonAsync($"api/Hotel/{hotelId}/promotions", requestBody, token);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PromotionDiscountDto>>(cancellationToken: token);

                if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                {
                    return new PromotionValidationResult
                    {
                        IsValid = true,
                        PromotionId = apiResponse.Data.PromotionId,
                        DiscountAmount = apiResponse.Data.FinalDiscountAmount,
                        Message = "Success"
                    };
                }
            }
            else
            {
                try
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(cancellationToken: token);

                    string errorMsg = errorResponse?.Message;
                    if (string.IsNullOrEmpty(errorMsg) && errorResponse?.Errors != null)
                    {
                        errorMsg = string.Join(", ", errorResponse.Errors);
                    }

                    return new PromotionValidationResult
                    {
                        IsValid = false,
                        Message = string.IsNullOrEmpty(errorMsg) ? "Invalid promotion (Unknown error)" : errorMsg
                    };
                }
                catch
                {
                    return new PromotionValidationResult { IsValid = false, Message = "Catalog Service Connection Error." };
                }
            }

            return new PromotionValidationResult
            {
                IsValid = false,
                Message = "Discount codes cannot be checked at this time."
            };
        }

        public async Task<RoomResponse?> CheckInRoom(Guid HotelId, Guid RoomId, Guid CheckInBy, CancellationToken token = default) 
        {
            var context = _httpContextAccessor.HttpContext;
            var bearerToken = context?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(bearerToken))
            {
                return null;
            }

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", bearerToken);

            var requestBody = new CheckInRoomRequestDto
            {
                HotelId = HotelId,
                RoomId = RoomId,
                CheckedInBy = CheckInBy
            };

            var response = await _httpClient.PostAsJsonAsync($"/api/Hotel/rooms/check-in", requestBody, token);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<RoomResponse?>>(cancellationToken: token);
                if (apiResponse != null && apiResponse.Success)
                {
                    return apiResponse.Data;
                }
            }
            return null;
        }

        public async Task RollbackCheckInRoom(Guid hotelId, Guid roomId, CancellationToken token)
        {
            var context = _httpContextAccessor.HttpContext;
            var bearerToken = context?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(bearerToken))
            {
                throw new UnauthorizedAccessException("Cannot rollback without token.");
            }

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", bearerToken);

            var requestBody = new
            {
                HotelId = hotelId,
                RoomId = roomId
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Hotel/rooms/rollback-check-in", requestBody, token);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(token);
                throw new Exception($"Rollback failed with status {response.StatusCode}. Details: {errorContent}");
            }

        }

        public async Task<bool> VerifyHotelOwnershipAsync(Guid hotelId, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            var bearerToken = context?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(bearerToken))
            {
                return false;
            }

            _httpClient.DefaultRequestHeaders.Authorization = null;
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", bearerToken);

            var response = await _httpClient.GetAsync($"/api/Hotel/is-hotel-owner?hotelId={hotelId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken);
                if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                {
                    return apiResponse.Data;
                }
            }

            return false;

        }
    }
}
