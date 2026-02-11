namespace SocialService.Domain.ValueObject
{
    public class Coordinates : Common.ValueObject
    {
        public double Latitude { get; }  // Vĩ độ
        public double Longitude { get; } // Kinh độ

        public Coordinates(double latitude, double longitude)
        {
            // Validate dữ liệu địa lý hợp lệ
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Vĩ độ phải từ -90 đến 90.");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Kinh độ phải từ -180 đến 180.");

            Latitude = latitude;
            Longitude = longitude;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
        }
    }
}
