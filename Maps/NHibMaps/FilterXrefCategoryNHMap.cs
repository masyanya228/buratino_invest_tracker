using Buratino.Entities;
using Buratino.Maps.NHibMaps;

public class FilterXrefCategoryNHMap : NHSubclassClassMap<FilterXrefCategory>
{
    public FilterXrefCategoryNHMap() : base()
    {
        Map(x => x.Category);
        References(x => x.Filter);
    }
}
