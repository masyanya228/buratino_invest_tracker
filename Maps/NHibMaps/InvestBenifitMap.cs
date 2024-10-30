using Buratino.Maps.NHibMaps;
using Buratino.Services;

public class InvestBenifitMap : NHSubclassClassMap<InvestBenifit>
{
    public InvestBenifitMap() : base()
    {
        Map(x => x.Value);
        Map(x => x.Description);

        References(x => x.Source, "InvestSourceId")
            .Not.LazyLoad();
    }
}
