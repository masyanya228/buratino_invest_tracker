using LiteDB;

namespace Buratino.Entities.Abstractions
{
    public abstract class EntityBase : IEquatable<EntityBase>, IEntityBase
    {
        [BsonId()]
        public virtual Guid Id { get; set; }

        public virtual DateTime TimeStamp { get; set; } = DateTime.Now;

        public virtual bool Equals(EntityBase other)
        {
            return other.GetType() == GetType()
                && other.Id == Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is EntityBase invest)
            {
                return Equals(invest);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}