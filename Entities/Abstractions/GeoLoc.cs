namespace Buratino.Entities.Abstractions
{
    public abstract class GeoLoc : PersistentEntity
    {
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual double GetDistanceTo(double lat2, double lon2)
        {
            return DistanceBetween(Latitude, Longitude, lat2, lon2);
        }
        public virtual double GetDistanceTo(GeoLoc geoLoc)
        {
            return GetDistanceTo(geoLoc.Latitude, geoLoc.Longitude);
        }
        public static double DistanceBetween(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371000f;
            double dLat = (lat1 - lat2) * Math.PI / 180f;
            double dLon = (lon1 - lon2) * Math.PI / 180f;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180f) * Math.Cos(lat2 * Math.PI / 180f) *
                            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2f * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;
            return d;
        }
    }
}