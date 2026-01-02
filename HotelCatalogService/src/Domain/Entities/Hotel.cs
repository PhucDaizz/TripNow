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

        public void AddBlock(string name)
        {
            if (_blocks.Any(x => x.Name.ToLower() == name.ToLower()))
                throw new InvalidOperationException($"Khu vực '{name}' đã tồn tại");

            _blocks.Add(new Block(this.Id, name));
        }

        public void UpdateBlock(Guid blockId, string newName)
        {
            var block = _blocks.FirstOrDefault(x => x.Id == blockId);
            if (block == null) return;

            if (_blocks.Any(x => x.Id != blockId && x.Name.ToLower() == newName.ToLower()))
                throw new InvalidOperationException($"Khu vực '{newName}' đã tồn tại");

            block.UpdateDetails(newName);
        }

        public void RemoveBlock(Guid blockId)
        {
            var block = _blocks.FirstOrDefault(x => x.Id == blockId);
            if (block == null) return;

            if (block.Floors.Any())
                throw new InvalidOperationException("Không thể xóa khu vực đã có tầng");

            _blocks.Remove(block);
        }

        public void UpdateRoomStatus(Guid roomId, RoomStatus newStatus)
        {
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

        public void AddRoom(Guid blockId, Guid floorId, string roomName, Guid roomTypeId)
        {
            var block = _blocks.FirstOrDefault(b => b.Id == blockId);
            if (block == null)
                throw new InvalidOperationException("Khu vực (Block) không tồn tại");

            var floor = block.Floors.FirstOrDefault(f => f.Id == floorId);
            if (floor == null)
                throw new InvalidOperationException("Tầng không tồn tại trong khu vực này");

            floor.AddRoom(roomName, roomTypeId);
        }

        public void RemoveRoom(Guid blockId, Guid floorId, Guid roomId)
        {
            var block = _blocks.FirstOrDefault(b => b.Id == blockId);
            if (block == null) return;

            var floor = block.Floors.FirstOrDefault(f => f.Id == floorId);
            if (floor == null) return;

            floor.RemoveRoom(roomId); 
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

        public void AddPromotion(string code, DiscountType type, decimal value, DateTime start, DateTime end, int qty, decimal minBookingAmount)
        {
            if (_promotions.Any(p => p.Code == code.ToUpper()))
                throw new InvalidOperationException($"Promotion code '{code}' already exists.");

            _promotions.Add(new Promotion(this.Id, code, type, value, start, end, qty, minBookingAmount));
        }

        public void UpdatePromotion(Guid promotionId, string code, DiscountType type, decimal value, DateTime start, DateTime end, int newTotalQty, decimal minBookingAmount)
        {
            var promo = _promotions.FirstOrDefault(p => p.Id == promotionId);
            if (promo == null) throw new KeyNotFoundException("Promotion not found");

            if (_promotions.Any(p => p.Id != promotionId && p.Code == code.ToUpper()))
                throw new InvalidOperationException($"Promotion code '{code}' already exists.");

            promo.UpdateDetails(code, type, value, start, end, newTotalQty, minBookingAmount);
        }

        public void ChangePromotionStatus(Guid promoId, bool isActive)
        {
            var promo = _promotions.FirstOrDefault(p => p.Id == promoId);
            if (promo == null) throw new KeyNotFoundException("Promotion not found");

            if (isActive) promo.Activate();
            else promo.Deactivate();
        }

        public void DeletePromotion(Guid promoId)
        {

            var promo = _promotions.FirstOrDefault(p => p.Id == promoId);
            if (promo == null) throw new KeyNotFoundException("Promotion not found");

            if (promo.IsUsed)
            {
                throw new InvalidOperationException("It is not possible to delete an encrypted user. Please select 'Deactivate' instead.");
            }

            _promotions.Remove(promo);
        }

        public decimal ApplyPromotion(string code, decimal orderAmount, Guid bookingId, Guid hotelId, Guid userId)
        {
            var promo = _promotions.FirstOrDefault(p => p.Code == code.ToUpper());

            if (promo == null) throw new InvalidOperationException("Discount codes do not exist for this hotel.");

            if (orderAmount < promo.MinBookingAmount)
            {
                throw new InvalidOperationException($"Orders must be {promo.MinBookingAmount:N0}đ or higher to use this code.");
            }

            try
            {
                promo.UsePromotion(bookingId, hotelId, userId, orderAmount); 
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"The code cannot be applied: {ex.Message}");
            }

            decimal discountAmount = 0;
            if (promo.DiscountType == DiscountType.Amount)
            {
                discountAmount = promo.DiscountValue;
            }
            else
            {
                discountAmount = orderAmount * (promo.DiscountValue / 100);
            }

            return discountAmount > orderAmount ? orderAmount : discountAmount;
        }

        public void RefundPromotion(Guid bookingId)
        {
            var targetPromo = _promotions.FirstOrDefault(p => p.PromotionUsages.Any(u => u.BookingId == bookingId));

            if (targetPromo != null)
            {
                targetPromo.RestorePromotion(bookingId);
            }
        }
    }
}
