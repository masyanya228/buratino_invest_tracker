using Buratino.Models.Entities.Abstractions;

namespace Buratino.Models.Entities
{
    public class RoleAccountLink : EntityBase
    {
        public virtual Role Role { get; set; }
        public virtual Account Account { get; set; }
    }
}