using Buratino.Entities.Abstractions;
using System.Linq.Expressions;

namespace Buratino.Models.Map.MapStructure
{
    public interface IMap<T> where T : IEntityBase
    {
        void Reference<Tprop>(Expression<Func<T, Tprop>> expression, string tableName = null);
    }
}