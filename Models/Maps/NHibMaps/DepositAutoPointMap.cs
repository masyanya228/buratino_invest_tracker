using Buratino.Entities;
using Buratino.Models.Map.NHibMaps;

public class DepositAutoPointMap : NHSubclassClassMap<DepositAutoPoint>
{
    public DepositAutoPointMap() : base()
    {
        Map(x => x.EndBalance);
        Map(x => x.Increment);
        Map(x => x.BVPS);
        Map(x => x.IsCapitalisation);
        Map(x => x.MidBalance);
    }
}
