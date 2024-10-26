using Buratino.Entities;
using Buratino.Models.Map.NHibMaps;

public class InvestSourceMap : NHSubclassClassMap<InvestSource>
{
    public InvestSourceMap()
    {
        Map(x => x.Name);
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
        Map(x => x.IsClosed);
        Map(x => x.CloseDate);
        Map(x => x.TotalRecharged);
        Map(x => x.TInvestAccountId);
        Map(x => x.Category);

        HasMany(x => x.Points)
            .Not.LazyLoad();

        HasMany(x => x.Charges)
            .Not.LazyLoad();

        HasMany(x => x.Benifits)
            .Not.LazyLoad();

        HasMany(x => x.Comments)
            .Not.LazyLoad();
    }
}