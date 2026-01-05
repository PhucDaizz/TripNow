using Domain.Common.Response;
using HotelCatalogService.Domain.Common.Models;
using HotelCatalogService.Domain.Dto.Hotel;
using HotelCatalogService.Domain.Enum;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelsWithPagination
{
    public class GetHotelsWithPaginationQuery : IRequest<Result<PagedResult<HotelDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public string? City { get; set; }        // Tìm khách sạn ở Hà Nội?
        public decimal? MinPrice { get; set; }   // Giá thấp nhất
        public decimal? MaxPrice { get; set; }   // Giá cao nhất

        public string? SearchTerm { get; set; } // Tìm theo tên, địa chỉ
        public HotelStatus? Status { get; set; } // Lọc theo trạng thái (Pending, Active, Blocked,...)
        public Guid? OwnerId { get; set; }       // Lọc theo chủ sở hữu (Dùng cho trang My Hotels)
        public int? GuestCount { get; set; }     // Đi bao nhiêu người?
        public DateTime? CheckInDate { get; set; } // Ngày nhận phòng

        // Sắp xếp (Optional)
        public string? SortColumn { get; set; } // "CreatedAt", "Name", "Rating"
        public string? SortDirection { get; set; } // "ASC", "DESC"
    }
}
