using Domain.Common.Domain.Common;

namespace Domain.Entities
{
    public class StaffProfile : BaseEntity
    {
        public string UserId { get; set; }
        public Guid HotelId { get; set; }
        public string Position { get; set; } // "Receptionist", "Cleaner"
        public virtual ExtendedIdentityUser User { get; set; }
    }
}
