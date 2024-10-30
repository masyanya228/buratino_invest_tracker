using Buratino.Entities.Abstractions;

namespace Buratino.Entities
{
    public class Filter : PersistentEntity
    {
        public virtual string Name { get; set; }
        
        public virtual IEnumerable<FilterXrefCategory> Categories{ get; set; }

        public virtual bool IsIncludeCategories { get; set; }
    }
}
