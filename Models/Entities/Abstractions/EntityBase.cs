using Buratino.Models.Xtensions;

using LiteDB;

namespace Buratino.Models.Entities.Abstractions
{
    public class EntityBase : IEntityBase
    {
        [BsonId()]
        public virtual long Id { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is IEntityBase entity)
            {
                return Id == entity.Id;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (int)Id;
        }
    }
}
