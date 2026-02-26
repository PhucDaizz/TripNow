namespace SocialService.Application.DTOs.Locations
{
    public class ContributedLocationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal AvgRating { get; set; }
        public bool IsVerify { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
