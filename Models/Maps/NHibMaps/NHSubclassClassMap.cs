using FluentNHibernate.Mapping;

namespace Buratino.Models.Map.NHibMaps
{
    public abstract class NHSubclassClassMap<T> : SubclassMap<T>, INHMap
    {
        public NHSubclassClassMap()
        {
            Table($"{typeof(T).Name}s");
        }
    }
}
