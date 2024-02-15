using Buratino.Entities;
using Buratino.Models.Map.NHibMaps;

public class InvestCommentMap : NHSubclassClassMap<InvestComment>
{
    public InvestCommentMap() : base()
    {
        Map(x => x.Description);

        References(x => x.Source, "InvestSourceId");
    }
}
