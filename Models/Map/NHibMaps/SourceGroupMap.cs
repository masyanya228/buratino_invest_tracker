using Buratino.Entities;
using Buratino.Models.Map.NHibMaps;

public class SourceGroupMap : NHSubclassClassMap<SourceGroup>
{
    public SourceGroupMap()
    {
        Table("SourceGroups");
    }
}