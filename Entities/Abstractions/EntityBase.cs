using Buratino.Models.Attributes;

using LiteDB;

namespace Buratino.Entities.Abstractions
{
    public abstract class EntityBase : IEntityBase
    {
        [BsonId()]
        [HidenProperty]
        public virtual Guid Id { get; set; } = Guid.Empty;

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
            return -1;
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
