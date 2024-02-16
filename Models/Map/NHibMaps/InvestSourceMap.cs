using Buratino.Entities;
using Buratino.Models.Map.NHibMaps;

public class InvestSourceMap : NHSubclassClassMap<InvestSource>
{
    public InvestSourceMap()
    {
        Map(x => x.Description);
        Map(x => x.EffectiveBase);
        Map(x => x.EffectiveBaseTimeStamp);
        Map(x => x.ProfitType);
        Map(x => x.TotalCharged);
        Map(x => x.LastBalance);
        Map(x => x.IsBankVklad);
        Map(x => x.BVPS);
        Map(x => x.BVCapitalisation);
        Map(x => x.BVEndStamp);
        Map(x => x.BVPeriodVyplat);

        References(x => x.SourceGroup, "SourceGroupId")
            .Not.LazyLoad();

        HasMany(x => x.Points)
            .Not.LazyLoad();

        HasMany(x => x.Charges)
            .Not.LazyLoad();

        HasMany(x => x.Comments)
            .Not.LazyLoad();

        Table("InvestSources");
    }
}