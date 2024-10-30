using Buratino.Entities;
using Buratino.Maps.NHibMaps;

public class InvestPointMap : NHSubclassClassMap<InvestPoint>
{
    public InvestPointMap() : base()
    {
        Map(x => x.Amount);
        Map(x => x.Description);

        References(x => x.Source, "InvestSourceId")
            .Not.LazyLoad();
    }
}
