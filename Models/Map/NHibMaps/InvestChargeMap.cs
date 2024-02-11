using Buratino.Entities;
using Buratino.Models.Map.NHibMaps;

public class InvestChargeMap : NHSubclassClassMap<InvestCharge>
{
    public InvestChargeMap() : base()
    {
        Map(x => x.Increment);

        References(x => x.Source, "InvestSourceId");
    }
}
