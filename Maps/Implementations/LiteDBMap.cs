using Buratino.Entities.Abstractions;
using Buratino.Maps.MapStructure;
using LiteDB;

using System.Linq.Expressions;

namespace Buratino.Maps.Implementations
{
    public class LiteDBMap<T> : IMap<T> where T : IEntityBase
    {
        public BsonMapper Mapper { get; set; }

        public void Reference<Tprop>(Expression<Func<T, Tprop>> expression, string tableName = null)
        {
            Mapper.Entity<T>().DbRef(expression, tableName);
        }
    }
}
