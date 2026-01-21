using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Entities
{
    public class CancellationRule: BaseEntity
    {
        public Guid CancellationPolicyId { get; private set; }

        /// <summary>
        /// Số giờ trước khi Check-in.
        /// VD: 48 nghĩa là "Trước 48 tiếng". 
        /// </summary>
        public int HoursBeforeCheckIn { get; private set; }

        /// <summary>
        /// Phần trăm hoàn tiền (0 - 100)
        /// </summary>
        public decimal RefundPercentage { get; private set; }

        private CancellationRule() { }

        internal CancellationRule(Guid policyId, int hours, decimal percentage)
        {
            CancellationPolicyId = policyId;
            HoursBeforeCheckIn = hours;
            RefundPercentage = percentage;
        }

        internal void Update(int hours, decimal percentage)
        {
            HoursBeforeCheckIn = hours;
            RefundPercentage = percentage;
        }
    }
}
