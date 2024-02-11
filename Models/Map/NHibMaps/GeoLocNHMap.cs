using Buratino.Entities.Abstractions;
using Buratino.Models.Map.NHibMaps;

public class GeoLocNHMap : NHSubclassClassMap<GeoLoc>
{
    public GeoLocNHMap()
    {
        Abstract();
        Map(x => x.Latitude);
        Map(x => x.Longitude);
    }
}