using Buratino.Entities.Abstractions;
using Buratino.Models.Map.NHibMaps;

public class NamedEntityMap : NHSubclassClassMap<NamedEntity>
{
    public NamedEntityMap()
    {
        Abstract();
        Map(x => x.Name);
    }
}