using HotelCatalogService.Domain.Common;
using HotelCatalogService.Domain.Common.Helpers;
using HotelCatalogService.Domain.Enum;
using HotelCatalogService.Domain.Events.Hotel;
using HotelCatalogService.Domain.ValueObject;

namespace HotelCatalogService.Domain.Entities
{
    public class Hotel: BaseEntity, AggregateRoot
    {
        public Guid OwnerId { get; private set; }
        public string Name { get; private set; }
        public string Slug { get; private set; }
        public string Description { get; private set; }
        public Address Address { get; private set; }
        public HotelStatus Status { get; private set; }
        public decimal Rating { get; private set; }
        public Coordinates Location { get; private set; }
        public bool IsActive { get; private set; }


        private readonly List<Block> _blocks = new();
        private readonly List<RoomType> _roomTypes = new(); 
        private readonly List<HotelAmenity> _amenities = new();
        private readonly List<Promotion> _promotions = new();
        private readonly List<HotelImage> _images = new();

        public IReadOnlyCollection<HotelImage> Images => _images.AsReadOnly();
        public IReadOnlyCollection<Block> Blocks => _blocks.AsReadOnly();
        public IReadOnlyCollection<RoomType> RoomTypes => _roomTypes.AsReadOnly();
        public IReadOnlyCollection<HotelAmenity> Amenities => _amenities.AsReadOnly();
        public IReadOnlyCollection<Promotion> Promotions => _promotions.AsReadOnly();


        private Hotel() 
        {
            _images = new List<HotelImage>();
            _blocks = new List<Block>();
            _roomTypes = new List<RoomType>();
            _amenities = new List<HotelAmenity>();
            _promotions = new List<Promotion>();
        }

        private Hotel(Guid ownerId, string name, string description,
                 Address address, Coordinates location) : this()
        {
            OwnerId = ownerId;
            Name = name;
            Description = description;
            Address = address;
            Location = location;
            Status = HotelStatus.Pending;
            IsActive = false;
            Rating = 0;
            Slug = SlugHelper.GenerateSlug(name);
        }


        public static Hotel Create(Guid ownerId, string name, string description,
                                   Address address, Coordinates location)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (ownerId == Guid.Empty) throw new ArgumentException("OwnerId invalid");

            return new Hotel(ownerId, name, description, address, location);
        }

        public void DefineRoomType(string name, decimal basePrice, int capacity, decimal size)
        {
            if (_roomTypes.Any(x => x.Name == name)) 
                throw new ArgumentException("LThe room type already exists.");

            var roomType = new RoomType(this.Id, name, basePrice, capacity, size);
            _roomTypes.Add(roomType);
        }

        public void RemoveRoomType(Guid roomTypeId)
        {
            var item = _roomTypes.FirstOrDefault(x => x.Id == roomTypeId);
            if (item != null)
            {
                // Nếu loại phòng này đã có người đặt (Booking) thì không được xóa
                _roomTypes.Remove(item);
            }
        }

        public void AddPhysicalRoom(string blockName, int floorNumber, string roomName, Guid roomTypeId)
        {
            if (!_roomTypes.Any(rt => rt.Id == roomTypeId))
                throw new ArgumentException("Loại phòng không hợp lệ");

            var block = _blocks.FirstOrDefault(b => b.Name == blockName);
            if (block == null)
            {
                block = new Block(Id ,blockName);
                _blocks.Add(block);
            }

            // Tìm hoặc tạo Floor trong Block
            var floor = block.Floors.FirstOrDefault(f => f.FloorNumber == floorNumber);
            if (floor == null)
            {
               floor = block.AddFloor(block.Id, floorNumber); 
            }

            floor.AddRoom(floor.Id, roomName, roomTypeId); 
        }

        public void UpdateRoomStatus(Guid roomId, RoomStatus newStatus)
        {
            // Tìm phòng trong tất cả các block/floor (Đây là sức mạnh của Aggregate Root)
            var room = _blocks.SelectMany(b => b.Floors).SelectMany(f => f.Rooms)
                              .FirstOrDefault(r => r.Id == roomId);

            if (room == null) throw new KeyNotFoundException("Không tìm thấy phòng.");

            if (newStatus == RoomStatus.Available && room.Status == RoomStatus.Dirty)
            {
                room.MarkAsClean();

                // Bắn Domain Event để thông báo cho Lễ tân (Real-time update)
                AddDomainEvent(new RoomCleanedEvent(Id, roomId, DateTime.UtcNow));
            }
            else if (newStatus == RoomStatus.Dirty)
            {
                room.MarkAsDirty();
            }
        }

        public void AddAmenity(Guid amenityId, string description, bool isFree)
        {
            if (_amenities.Any(a => a.AmenityId == amenityId)) return; 
            _amenities.Add(new HotelAmenity(Id, amenityId, description, isFree));
        }

        public void RemoveAmenity(Guid amenityId)
        {
            var item = _amenities.FirstOrDefault(x => x.AmenityId == amenityId);
            if (item != null) _amenities.Remove(item);
        }

        public void UpdateAmenity(Guid amenityId, string? description, bool isFree)
        {
            var item = _amenities.FirstOrDefault(x => x.AmenityId == amenityId);
            if (item == null) return; 

            item.UpdateInfo(description, isFree);
        }

        public void CreatePromotion(string code, byte type, decimal value, DateTime start, DateTime end, int qty)
        {
            if (_promotions.Any(p => p.Code == code && p.IsValid()))
                throw new InvalidOperationException("Mã giảm giá này đang tồn tại");

            _promotions.Add(new Promotion(code, type, value, start, end, qty));
        }

        public void SetRoomPrice(Guid roomTypeId, DateTime date, decimal price)
        {
            var roomType = _roomTypes.FirstOrDefault(rt => rt.Id == roomTypeId);
            if (roomType == null) throw new KeyNotFoundException("Loại phòng không tồn tại");

            roomType.SetSpecialPrice(date, price);
        }


        public void UpdateInfo(string name, string description, Address address, Coordinates location)
        {
            Name = name;
            Description = description;
            Address = address;
            Location = location;
            Slug = SlugHelper.GenerateSlug(name); 
            // HotelInfoUpdatedEvent
        }

        public void UpdateRating(decimal newRating)
        {
            if (newRating < 0 || newRating > 5)
                throw new ArgumentException("Rating phải từ 0 đến 5");
            Rating = newRating;
        }

        public void Approve()
        {
            if (Status == HotelStatus.Active) return;

            Status = HotelStatus.Active;
            IsActive = true;

            // Email thông báo cho chủ khách sạn
            AddDomainEvent(new HotelApprovedEvent(Id, Name, OwnerId));
        }

        public void Suspend(string reason)
        {
            Status = HotelStatus.Blocked;
            IsActive = false;
        }

        public void AddImage(string imageUrl, string publicId, bool isThumbnail, string? caption)
        {
            if (_images.Count == 0) isThumbnail = true;

            if (isThumbnail)
            {
                foreach (var img in _images)
                {
                    img.SetThumbnail(false);
                }
            }

            var newImage = new HotelImage(this.Id, imageUrl, publicId, isThumbnail, caption);
            _images.Add(newImage);
        }

        public void UpdateImageDetails(Guid imageId, bool isThumbnail, string? caption)
        {
            var img = _images.FirstOrDefault(x => x.Id == imageId);
            if (img == null) return;

            img.UpdateDetails(caption);

            if (isThumbnail)
            {
                foreach (var existingImg in _images)
                {
                    existingImg.SetThumbnail(false);
                }
                img.SetThumbnail(true);
            }
            else if (img.IsThumbnail && !isThumbnail)
            {
                img.SetThumbnail(false);
            }
        }

        public void RemoveImage(Guid imageId)
        {
            var img = _images.FirstOrDefault(x => x.Id == imageId);
            if (img == null) return;

            _images.Remove(img);

            if (img.IsThumbnail && _images.Count > 0)
            {
                _images.First().SetThumbnail(true);
            }
        }
    }
}
