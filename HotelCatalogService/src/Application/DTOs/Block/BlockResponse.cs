using HotelCatalogService.Application.DTOs.Floor;

namespace HotelCatalogService.Application.DTOs.Block
{
    public class BlockResponse
    {
        public Guid BlockId { get; set; }
        public string BlockName { get; set; } // "Khu A"
        public List<FloorResponse> Floors { get; set; } = new();
    }
}
