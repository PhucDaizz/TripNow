using MediatR;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Application.Features.Hotel.Helpers;

namespace RecommendationService.Application.Features.Hotel.EventHandlers.HotelIndexedIntegration
{
    public class HotelIndexedIntegrationEventHandler : INotificationHandler<HotelIndexedIntegrationEvent>
    {
        private const string CollectionName = "Hotels";

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
        }
    }
}
