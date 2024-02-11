using FluentNHibernate.Mapping;

namespace Buratino.Models.Map.NHibMaps
{
    public abstract class NHClassMap<T> : ClassMap<T>, INHMap
    {
    }
}
