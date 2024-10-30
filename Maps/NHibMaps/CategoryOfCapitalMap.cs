using Buratino.Entities;
using Buratino.Maps.NHibMaps;
using Buratino.Services;

public class CategoryOfCapitalMap : NHSubclassClassMap<CategoryOfCapital>
{
    public CategoryOfCapitalMap() : base()
    {
        Map(x => x.Value);
        Map(x => x.CategoryOfCapitalEnum);

        References(x => x.Source, "InvestSourceId")
            .Not.LazyLoad();
    }
}
