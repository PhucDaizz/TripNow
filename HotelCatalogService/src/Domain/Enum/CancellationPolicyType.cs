namespace HotelCatalogService.Domain.Enum
{
    public enum CancellationPolicyType : byte
    {
        Flexible = 0,       // Linh hoạt
        Moderate = 1,       // Trung bình
        Strict = 2,         // Nghiêm ngặt
        NonRefundable = 3   // Không hoàn hủy
    }
}
