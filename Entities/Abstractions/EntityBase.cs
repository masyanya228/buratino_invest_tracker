using Buratino.Models.Attributes;

using LiteDB;

namespace Buratino.Entities.Abstractions
{
    public abstract class EntityBase : IEntityBase
    {
        [BsonId()]
        [HidenProperty]
        public virtual Guid Id { get; set; } = Guid.Empty;


        public static bool operator == (EntityBase f1, EntityBase f2)
        {
            if (f1 is null && f2 is null)
            {
                return true;
            }

            if (f1 is null)
            {
                return false;
            }

            return f1.Equals(f2);
        }

        public static bool operator != (EntityBase f1, EntityBase f2)
        {
            if (f1 is null && f2 is null)
            {
                return true;
            }

            if (f1 is null)
            {
                return false;
            }

            return !f1.Equals(f2);
        }

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
