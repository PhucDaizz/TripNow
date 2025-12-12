namespace Infrastructure.Common
{
    public static class AppRoles
    {
        public const string SysAdmin = "SysAdmin";
        public const string HotelOwner = "HotelOwner";
        public const string Receptionist = "Receptionist";
        public const string Housekeeping = "Housekeeping";
        public const string Customer = "Customer";

        public static IEnumerable<string> AllRoles => new[]
        {
            SysAdmin, HotelOwner, Receptionist, Housekeeping, Customer
        };
    }
}
