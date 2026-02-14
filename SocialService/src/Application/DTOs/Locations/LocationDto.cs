namespace SocialService.Application.DTOs.Locations
{
    public class LocationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }  
        public double Longitude { get; set; } 
        public string Type { get; set; }      
        public decimal AvgRating { get; set; }
        public bool IsVerify { get; set; }
    }
}
