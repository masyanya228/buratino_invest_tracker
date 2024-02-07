using Buratino.Models.Entities.Abstractions;

using FluentNHibernate.Mapping;

namespace Buratino.Models.Map.Implementations
{
    public abstract class NHibMapBase<T> : ClassMap<T> where T : IEntityBase
    {

    }
}
