namespace HotelCatalogService.Domain.Enum
{
    public enum DiscountType : byte
    {
        Percent = 1, // Giảm theo % (VD: 10%)
        Amount = 2   // Giảm tiền mặt (VD: 100k)
    }
}
