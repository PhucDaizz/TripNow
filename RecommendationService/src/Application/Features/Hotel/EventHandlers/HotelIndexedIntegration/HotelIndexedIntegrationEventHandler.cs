using MediatR;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Application.Features.Hotel.Helpers;

namespace RecommendationService.Application.Features.Hotel.EventHandlers.HotelIndexedIntegration
{
    public class HotelIndexedIntegrationEventHandler : INotificationHandler<HotelIndexedIntegrationEvent>
    {
        private const string CollectionName = "Hotels";
        private const string RagCollection = "HotelKnowledgeBase";

        private readonly IEmbeddingService _embeddingService;
        private readonly IQdrantService    _qdrantService;
        private readonly ILogger<HotelIndexedIntegrationEventHandler> _logger;

        public HotelIndexedIntegrationEventHandler(
            IEmbeddingService embeddingService,
            IQdrantService    qdrantService,
            ILogger<HotelIndexedIntegrationEventHandler> logger)
        {
            _embeddingService = embeddingService;
            _qdrantService    = qdrantService;
            _logger           = logger;
        }

        public async Task Handle(HotelIndexedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Indexing hotel {HotelId} - {HotelName} into Qdrant", notification.HotelId, notification.Name);

            // 1. Đảm bảo collection tồn tại
            await _qdrantService.EnsureCollectionExistsAsync(
                collectionName: CollectionName,
                vectorSize    : (ulong)_embeddingService.VectorSize);

            // 2. Format hotel data thành document text
            var documentText = HotelDocumentFormatter.Format(notification);

            // 3. Generate embedding vector
            var vector = await _embeddingService.GenerateEmbeddingAsync(documentText, cancellationToken);

            if (vector.Length == 0)
            {
                _logger.LogWarning("Embedding returned empty vector for hotel {HotelId}. Skipping upsert.", notification.HotelId);
                return;
            }

            // 4. Build payload metadata (dùng cho filter & RAG context sau này)
            var payload = HotelDocumentFormatter.BuildPayload(notification);

            // 5. Upsert vào Qdrant
            await _qdrantService.UpsertVectorAsync(
                collectionName: CollectionName,
                id            : notification.HotelId,
                vector        : vector,
                payload       : payload);

            _logger.LogInformation("Successfully indexed hotel {HotelId} into Qdrant collection '{Collection}'",
                notification.HotelId, CollectionName);



            await _qdrantService.EnsureCollectionExistsAsync(RagCollection, (ulong)_embeddingService.VectorSize);

            var ragChunks = new List<(Guid Id, float[] Vector, Dictionary<string, object> Payload)>();

            string generalInfo = $@"
                Thông tin tổng quan khách sạn {notification.Name}:
                - Mô tả: {notification.Description}
                - Vị trí: {notification.Street}, {notification.City}, {notification.Country}
                - Tiện ích chung: {string.Join(", ", notification.AmenityNames)}
                ";

            var generalVector = await _embeddingService.GenerateEmbeddingAsync(generalInfo, cancellationToken);
            ragChunks.Add((
                Guid.NewGuid(), 
                generalVector,
                CreateRagPayload(notification.HotelId, "Tổng quan", generalInfo)
            ));

            if (notification.RoomTypes != null)
            {
                foreach (var room in notification.RoomTypes)
                {
                    string roomInfo = $@"
                        Thông tin phòng: {room.Name}
                        - Giá cơ bản: {room.BasePrice} VND
                        - Sức chứa: {room.Capacity} người
                        - Diện tích: {room.SizeM2} m2
                        - Mô tả phòng: {room.Description}
                        - Chính sách hủy phòng này: {room.CancellationPolicyDescription ?? "Không có"}
                        ";

                    var roomVector = await _embeddingService.GenerateEmbeddingAsync(roomInfo, cancellationToken);

                    ragChunks.Add((
                        Guid.NewGuid(),
                        roomVector,
                        CreateRagPayload(notification.HotelId, $"Phòng {room.Name}", roomInfo)
                    ));
                }
            }

            foreach (var chunk in ragChunks)
            {
                await _qdrantService.UpsertVectorAsync(
                    collectionName: RagCollection,
                    id: chunk.Id, 
                    vector: chunk.Vector,
                    payload: chunk.Payload);
            }


        }

        private Dictionary<string, object> CreateRagPayload(Guid hotelId, string category, string content)
        {
            return new Dictionary<string, object>
            {
                ["hotelId"] = hotelId.ToString(), 
                ["category"] = category,
                ["content"] = content
            };
        }
    }
}
