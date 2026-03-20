namespace RecommendationService.Infrastructure.Settings
{
    public class QdrantSettings
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 6334; // gRPC default port
        public bool Https { get; set; } = false;
        public string? ApiKey { get; set; }
    }
}
