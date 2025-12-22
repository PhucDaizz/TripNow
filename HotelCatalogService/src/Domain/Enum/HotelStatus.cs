namespace HotelCatalogService.Domain.Enum
{
    public enum HotelStatus
    {
        Pending = 0,   // Chờ duyệt KYB 
        Active = 1,
        Blocked = 2    // Bị khóa do vi phạm 
    }
}
