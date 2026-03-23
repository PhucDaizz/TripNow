using HotelCatalogService.Domain.Common;
using HotelCatalogService.Domain.Enum;

namespace HotelCatalogService.Domain.Entities
{
    public class CancellationPolicy: BaseEntity, AggregateRoot
    {
        public Guid HotelId { get; private set; }
        public string Name { get; private set; }        // VD: "Chính sách Mùa Hè"
        public CancellationPolicyType Type { get; private set; }
        public string Description { get; private set; } // VD: "Free cancellation until 24h before check-in"

        private readonly List<CancellationRule> _rules = new();
        public IReadOnlyCollection<CancellationRule> Rules => _rules.AsReadOnly();

        private CancellationPolicy() { }

        public CancellationPolicy(Guid hotelId, string name, string description, CancellationPolicyType type)
        {
            HotelId = hotelId;
            Name = name;
            Description = description;
            Type = type;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddRule(int hoursBeforeCheckIn, decimal refundPercentage)
        {
            // Validate: Phần trăm phải từ 0 đến 100 (hoặc 0 - 1.0 tùy quy ước)
            if (refundPercentage < 0 || refundPercentage > 100)
                throw new ArgumentException("Refund percentage must be between 0 and 100.");

            var rule = new CancellationRule(Id, hoursBeforeCheckIn, refundPercentage);
            _rules.Add(rule);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveRule(Guid ruleId)
        {
            var rule = _rules.FirstOrDefault(r => r.Id == ruleId);
            if (rule != null)
            {
                _rules.Remove(rule);
            }
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateRule(Guid ruleId, int hoursBeforeCheckIn, decimal refundPercentage)
        {
            var rule = _rules.FirstOrDefault(r => r.Id == ruleId);
            if (rule == null)
            {
                throw new KeyNotFoundException($"Cancellation rule with id {ruleId} not found.");
            }
            
             if (refundPercentage < 0 || refundPercentage > 100)
                throw new ArgumentException("Refund percentage must be between 0 and 100.");

            UpdatedAt = DateTime.UtcNow;

            rule.Update(hoursBeforeCheckIn, refundPercentage);
        }

        public void Update(string name, string description, CancellationPolicyType type)
        {
            Name = name;
            Description = description;
            Type = type;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
