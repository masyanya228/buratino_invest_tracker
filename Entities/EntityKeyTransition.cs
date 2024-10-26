using Buratino.Entities.Abstractions;

namespace Buratino.Entities
{
    public class EntityKeyTransition : EntityBase
    {
        public virtual string EntityType { get; set; }

        public virtual long OldId { get; set; }

        public virtual Guid NewId { get; set; }
    }
}
