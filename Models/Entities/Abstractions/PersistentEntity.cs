using System.ComponentModel.DataAnnotations;

namespace Buratino.Models.Entities.Abstractions
{
    public class PersistentEntity : NamedEntity
    {
        [Display(Name = "Удален")]
        public virtual bool IsDeleted { get; set; }

        public virtual DateTime TimeStamp { get; set; } = DateTime.Now;
    }
}