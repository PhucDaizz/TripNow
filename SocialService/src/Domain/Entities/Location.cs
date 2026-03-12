using SocialService.Domain.Common;
using SocialService.Domain.Enum;
using SocialService.Domain.Events.Location;
using SocialService.Domain.Exceptions;
using SocialService.Domain.ValueObject;

namespace SocialService.Domain.Entities
{
    public class Location: BaseEntity, AggregateRoot
    {
        public string Name { get; private set; }
        public string Address { get; private set; }
        public Coordinates Coordinates { get; private set; }
        public LocationType Type { get; private set; }
        public decimal AvgRating { get; private set; }
        public bool IsVerify { get; private set; }
        public Guid? CreatedByUserId { get; private set; }

        private Location() { }

        public Location(string name, Coordinates coordinates, string address, LocationType type, Guid? createdByUserId)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Tên vị trí không thể trống.");
            if (string.IsNullOrWhiteSpace(address)) throw new DomainException("Địa chỉ không thể trống.");
            if (coordinates == null) throw new DomainException("Toạ độ không thể trống.");

            if (!System.Enum.IsDefined(typeof(LocationType), type))
            {
                throw new DomainException("Loại địa điểm không hợp lệ.");
            }

            Name = name;
            Address = address;
            Coordinates = coordinates;
            Type = type;
            AvgRating = 0;
            IsVerify = false;
            CreatedByUserId = createdByUserId;
        }

        public void Verify()
        {
            IsVerify = true;
            AddDomainEvent(new LocationVerifiedEvent{
                LocationId = Id,
                CreateBy = CreatedByUserId ?? Guid.Empty
            });
        }

        public void UpdateType(LocationType newType)
        {
            if (!System.Enum.IsDefined(typeof(LocationType), newType))
                throw new DomainException("Loại địa điểm không hợp lệ.");
            Type = newType;
        }

        public void UpdateDetails(string name, string address, Coordinates coordinates, LocationType type)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Tên vị trí không thể trống.");
            if (string.IsNullOrWhiteSpace(address)) throw new DomainException("Địa chỉ không thể trống.");
            if (coordinates == null) throw new DomainException("Toạ độ không thể trống.");

            if (!System.Enum.IsDefined(typeof(LocationType), type))
                throw new DomainException("Loại địa điểm không hợp lệ.");

            Name = name.Trim();
            Address = address.Trim();
            Coordinates = coordinates;
            Type = type;

            IsVerify = false; 
        }
    }
}
