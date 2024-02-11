using Buratino.Entities;
using Buratino.Models.Map.NHibMaps;

public class InvestPointMap : NHSubclassClassMap<InvestPoint>
{
    public InvestPointMap() : base()
    {
        Map(x => x.Amount);

        References(x => x.Source, "InvestSourceId");
    }
}
