using HotelCatalogService.Application.DTOs.CancellationPolicy;

namespace HotelCatalogService.Application.DTOs.RoomType
{
    public class RoomTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public decimal BasePrice { get; set; }
        public decimal CurrentPrice { get; set; }   
        public bool IsDiscounted { get; set; }

        public int Capacity { get; set; }
        public decimal SizeM2 { get; set; }
        public string MainImage { get; set; }
        public CancellationPolicyDto? CancellationPolicy { get; set; }
    }
}
