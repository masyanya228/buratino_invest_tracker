using Buratino.Entities.Abstractions;
using Buratino.Enums;

namespace Buratino.Entities
{
    public class FilterXrefCategory : EntityBase
    {
        public virtual Filter Filter { get; set; }

        public virtual CategoryEnum Category { get; set; }
    }
}
