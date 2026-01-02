using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Entities
{
    public class PromotionUsage: BaseEntity
    {
        public Guid PromotionId { get; private set; }
        public Guid BookingId { get; private set; }
        public Guid UserId { get; private set; }

        private PromotionUsage() { } 

        internal PromotionUsage(Guid promotionId, Guid bookingId, Guid userId)
        {
            PromotionId = promotionId;
            BookingId = bookingId;
            UserId = userId;
        }
    }
}
