using Buratino.Entities;
using Buratino.Maps.NHibMaps;

public class FilterNHMap : NHSubclassClassMap<Filter>
{
    public FilterNHMap() : base()
    {
        Map(x => x.Name);
        Map(x => x.IsIncludeCategories);
        HasMany(x => x.Categories)
            .Not.LazyLoad();
    }
}
