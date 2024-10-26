using Buratino.Entities.Abstractions;

namespace Buratino.Entities
{
    public class EntityKeyTransition : EntityBase
    {
        public string EntityType { get; set; }

        public long OldId { get; set; }

        public Guid NewId { get; set; }
    }
}
