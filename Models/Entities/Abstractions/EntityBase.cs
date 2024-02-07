using LiteDB;

namespace Buratino.Models.Entities.Abstractions
{
    public class EntityBase : IEntityBase
    {
        [BsonId()]
        public virtual long Id { get; set; }
    }
}