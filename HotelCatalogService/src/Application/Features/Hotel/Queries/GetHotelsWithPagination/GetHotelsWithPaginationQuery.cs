using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Hotel;
using HotelCatalogService.Domain.Common.Models;
using HotelCatalogService.Domain.Enum;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelsWithPagination
{
    public class GetHotelsWithPaginationQuery : IRequest<Result<PagedResult<HotelDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public string? SearchTerm { get; set; } // Tìm theo tên, địa chỉ
        public HotelStatus? Status { get; set; } // Lọc theo trạng thái (Pending, Active, Blocked)
        public Guid? OwnerId { get; set; }       // Lọc theo chủ sở hữu (Dùng cho trang My Hotels)
        public bool? IsActive { get; set; }      // Lọc ẩn/hiện

        // Sắp xếp (Optional)
        public string? SortColumn { get; set; } // "CreatedAt", "Name", "Rating"
        public string? SortDirection { get; set; } // "ASC", "DESC"
    }
}
