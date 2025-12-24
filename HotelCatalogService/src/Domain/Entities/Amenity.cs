using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Entities
{
    public class Amenity : BaseEntity, AggregateRoot
    {
        public string Name { get; private set; }
        public string Icon { get; private set; }

        private Amenity() { }

        private Amenity(string name, string icon)
        {

            Name = name;
            Icon = icon;
        }

        public static Amenity Create(string name, string icon)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            return new Amenity(name, icon);
        }

        public void Update(string name, string icon)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            Name = name;
            Icon = icon;
        }

    }
}

