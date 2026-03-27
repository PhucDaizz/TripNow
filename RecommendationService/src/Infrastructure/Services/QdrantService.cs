using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Application.DTOs.Recommendation;
using RecommendationService.Infrastructure.Settings;

namespace RecommendationService.Infrastructure.Services
{
    public class QdrantService : IQdrantService
    {
        private readonly QdrantClient _client;

        public QdrantService(IOptions<QdrantSettings> qdrantSettings)
        {
            var options = qdrantSettings.Value;
            _client = new QdrantClient(host: options.Host, port: options.Port, https: options.Https, apiKey: string.IsNullOrEmpty(options.ApiKey) ? null : options.ApiKey);
        }

        public async Task EnsureCollectionExistsAsync(string collectionName, ulong vectorSize)
        {
            var collections = await _client.ListCollectionsAsync();
            if (!collections.Contains(collectionName))
            {
                await _client.CreateCollectionAsync(collectionName, new VectorParams { Size = vectorSize, Distance = Distance.Cosine });
            }
        }

        public async Task UpsertVectorAsync(string collectionName, Guid id, float[] vector, Dictionary<string, object>? payload = null)
        {
            var point = new PointStruct
            {
                Id = id,
                Vectors = vector
            };

            if (payload != null)
            {
                foreach (var item in payload)
                {
                    point.Payload[item.Key] = MapToQdrantValue(item.Value);
                }
            }

            await _client.UpsertAsync(collectionName, new[] { point });
        }

        public async Task<IReadOnlyList<VectorSearchResult>> SearchSimilarAsync(string collectionName, float[] queryVector, ulong limit = 10)
        {
            var qdrantResults = await _client.SearchAsync(
                collectionName: collectionName,
                vector: queryVector,
                limit: limit);

            var mappedResults = qdrantResults.Select(r => new VectorSearchResult
            {
                HotelId = Guid.Parse(r.Id.Uuid),
                Score = r.Score
            }).ToList();

            return mappedResults;
        }

        public async Task<IReadOnlyList<VectorSearchResult>> RecommendAsync(
            string collectionName,
            IEnumerable<Guid> positiveHotelIds,
            List<string>? preferredCities = null, 
            ulong limit = 10)
        {
            var positivePoints = positiveHotelIds.Select(id => new PointId { Uuid = id.ToString() }).ToList();

            Filter? filter = null;

            if (preferredCities != null && preferredCities.Any())
            {
                filter = new Filter();
                foreach (var city in preferredCities.Distinct()) 
                {
                    filter.Should.Add(new Condition
                    {
                        Field = new FieldCondition
                        {
                            Key = "city",
                            Match = new Match { Keyword = city }
                        }
                    });
                }
            }

            var qdrantResults = await _client.RecommendAsync(
                collectionName: collectionName,
                positive: positivePoints,
                filter: filter,
                limit: limit);

            var mappedResults = qdrantResults.Select(r => new VectorSearchResult
            {
                HotelId = Guid.Parse(r.Id.Uuid),
                Score = r.Score
            }).ToList();

            return mappedResults;
        }

        public async Task<List<string>> GetCitiesByHotelIdsAsync(string collectionName, IEnumerable<Guid> hotelIds)
        {
            var pointIds = hotelIds.Select(id => new PointId { Uuid = id.ToString() }).ToList();

            var points = await _client.RetrieveAsync(collectionName, pointIds, withPayload: true);

            var cities = points
                .Where(p => p.Payload.ContainsKey("city"))
                .Select(p => p.Payload["city"].StringValue) 
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .ToList();

            return cities;
        }

        public async Task<IReadOnlyList<VectorSearchResult>> GetSimilarHotelsAdvancedAsync(
            string collectionName,
            Guid currentHotelId,
            string targetCity,
            ulong totalLimit = 5)
        {
            var positivePoint = new PointId { Uuid = currentHotelId.ToString() };

            ulong cityLimit = (ulong)Math.Ceiling(totalLimit * 0.8);
            ulong exploreLimit = totalLimit - cityLimit;

            var cityFilter = new Filter();
            cityFilter.Must.Add(new Condition
            {
                Field = new FieldCondition { Key = "city", Match = new Match { Keyword = targetCity } }
            });

            var cityTask = _client.RecommendAsync(
                collectionName: collectionName,
                positive: new[] { positivePoint },
                filter: cityFilter,
                limit: cityLimit);

            var exploreFilter = new Filter();
            exploreFilter.MustNot.Add(new Condition
            {
                Field = new FieldCondition { Key = "city", Match = new Match { Keyword = targetCity } }
            });

            var exploreTask = _client.RecommendAsync(
                collectionName: collectionName,
                positive: new[] { positivePoint },
                filter: exploreFilter,
                limit: exploreLimit);

            await Task.WhenAll(cityTask, exploreTask);

            var finalResults = new List<VectorSearchResult>();

            finalResults.AddRange(cityTask.Result.Select(r => new VectorSearchResult
            {
                HotelId = Guid.Parse(r.Id.Uuid),
                Score = r.Score,
            }));

            finalResults.AddRange(exploreTask.Result.Select(r => new VectorSearchResult
            {
                HotelId = Guid.Parse(r.Id.Uuid),
                Score = r.Score,
            }));

            return finalResults;
        }

        private Value MapToQdrantValue(object value)
        {
            if (value == null)
                return new Value { NullValue = Qdrant.Client.Grpc.NullValue.NullValue };

            return value switch
            {
                string s => new Value { StringValue = s },
                int i => new Value { IntegerValue = i },
                long l => new Value { IntegerValue = l },
                float f => new Value { DoubleValue = f },
                double d => new Value { DoubleValue = d },
                decimal dec => new Value { DoubleValue = (double)dec }, 
                bool b => new Value { BoolValue = b },
                Guid g => new Value { StringValue = g.ToString() },
                _ => new Value { StringValue = value.ToString() } 
            };
        }

        public async Task<IEnumerable<Guid>> GetRandomHotelsAsync(string collectionName, ulong limit)
        {
            int vectorSize = 1024;

            var random = new Random();
            var randomVector = new float[vectorSize];
            for (int i = 0; i < vectorSize; i++)
            {
                randomVector[i] = (float)((random.NextDouble() * 2.0) - 1.0);
            }

            var searchResults = await _client.SearchAsync(
                collectionName: collectionName,
                vector: randomVector,
                limit: limit
            );

            return searchResults.Select(r => Guid.Parse(r.Id.Uuid)).ToList();
        }
    }
}
