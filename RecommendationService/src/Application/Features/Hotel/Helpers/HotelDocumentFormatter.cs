using RecommendationService.Application.Features.Hotel.EventHandlers.HotelIndexedIntegration;
using System.Text;

namespace RecommendationService.Application.Features.Hotel.Helpers
{
    /// <summary>
    /// Chịu trách nhiệm chuyển đổi thông tin khách sạn thành một chuỗi văn bản có cấu trúc
    /// để đưa vào embedding model. Format được thiết kế:
    ///   - Giàu ngữ nghĩa (semantic-rich) để vector hóa tốt.
    ///   - Dễ mở rộng khi thêm field mới (chỉ thêm dòng vào builder).
    ///   - Tái sử dụng được cho cả indexing lẫn RAG chatbot (prompt context).
    /// </summary>
    public static class HotelDocumentFormatter
    {
        /// <summary>
        /// Tạo document text từ integration event dùng khi indexing lần đầu.
        /// </summary>
        public static string Format(HotelIndexedIntegrationEvent hotel)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Hotel: {hotel.Name}");
            sb.AppendLine($"Location: {hotel.Street}, {hotel.City}, {hotel.Country}");
            sb.AppendLine($"Rating: {hotel.Rating:F1}/5");
            //sb.AppendLine($"Starting price: {hotel.StartingPrice:N0} VND per night");

            if (!string.IsNullOrWhiteSpace(hotel.Description))
                sb.AppendLine($"Description: {hotel.Description}");

            if (hotel.AmenityNames?.Count > 0)
                sb.AppendLine($"Amenities: {string.Join(", ", hotel.AmenityNames.Where(a => !string.IsNullOrWhiteSpace(a)))}");

            if (hotel.RoomTypes?.Count > 0)
            {
                sb.AppendLine("Room types & features:");
                foreach (var rt in hotel.RoomTypes)
                {
                    var roomInfo = $"- {rt.Name}";

                    if (!string.IsNullOrWhiteSpace(rt.Description))
                        roomInfo += $": {rt.Description}";

                    if (!string.IsNullOrWhiteSpace(rt.CancellationPolicyDescription))
                        roomInfo += $" (Policy: {rt.CancellationPolicyDescription})";

                    sb.AppendLine(roomInfo);
                }
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Tạo payload metadata lưu kèm vector trong Qdrant.
        /// Payload dùng để filter/display kết quả mà không cần gọi lại database.
        /// Đây cũng là context snippet dùng cho RAG chatbot sau này.
        /// </summary>
        public static Dictionary<string, object> BuildPayload(HotelIndexedIntegrationEvent hotel)
        {
            var maxCapacity = (hotel.RoomTypes != null && hotel.RoomTypes.Any())
                ? hotel.RoomTypes.Max(rt => rt.Capacity)
                : 0;

            var maxSizeM2 = (hotel.RoomTypes != null && hotel.RoomTypes.Any())
                ? hotel.RoomTypes.Max(rt => rt.SizeM2)
                : 0;

            return new Dictionary<string, object>
            {
                ["hotel_id"]       = hotel.HotelId.ToString(),
                ["name"]           = hotel.Name,
                ["city"]           = hotel.City,
                ["country"]        = hotel.Country,
                ["rating"]         = hotel.Rating,
                ["starting_price"] = hotel.StartingPrice,
                ["thumbnail_url"]  = hotel.ThumbnailUrl ?? string.Empty,
                ["max_capacity"] = maxCapacity,
                ["max_size_m2"] = (double)maxSizeM2,
                ["amenities"] = hotel.AmenityNames != null
                                     ? string.Join(", ", hotel.AmenityNames.Where(a => !string.IsNullOrWhiteSpace(a)))
                                     : string.Empty
            };
        }
    }
}
