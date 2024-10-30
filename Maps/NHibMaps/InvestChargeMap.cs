using Buratino.Entities;
using Buratino.Maps.NHibMaps;

public class InvestChargeMap : NHSubclassClassMap<InvestCharge>
{
    public InvestChargeMap() : base()
    {
        Map(x => x.Increment);
        Map(x => x.Description);

        References(x => x.Source, "InvestSourceId")
            .Not.LazyLoad();
    }
}
