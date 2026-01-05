namespace HotelCatalogService.Domain.Enum
{
    public enum HotelStatus
    {
        Draft = 0,              // 1. Nháp: Chủ khách sạn đang tạo, chưa xong, chưa muốn ai thấy.
        PendingApproval = 1,    // 2. Chờ duyệt: Đã điền xong thông tin, gửi cho Admin duyệt (KYB).
        Active = 2,             // 3. Hoạt động: Admin đã duyệt, khách có thể đặt phòng.
        Rejected = 3,           // 4. Từ chối: Admin từ chối (do giấy tờ sai, v.v.).
        Suspended = 4,          // 5. Bị đình chỉ: Admin khóa do vi phạm chính sách.
        TemporarilyClosed = 5   // 6. Tạm đóng: Chủ khách sạn tự ẩn (sửa chữa, nghỉ đông, v.v.).
    }
}
