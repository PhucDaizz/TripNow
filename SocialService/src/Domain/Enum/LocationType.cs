namespace SocialService.Domain.Enum
{
    public enum LocationType: int
    {
        Other = 0,
        Restaurant = 1,   // Nhà hàng, quán ăn
        Coffee = 2,       // Cafe, trà sữa
        Entertainment = 3, // Khu vui chơi, Bar, Pub, Karaoke
        Sightseeing = 4,  // Danh lam thắng cảnh, bảo tàng
        Shopping = 5,     // Trung tâm thương mại, chợ
        Service = 6       // Tiệm thuốc, tạp hóa, ATM (nếu cần)
    }
}
