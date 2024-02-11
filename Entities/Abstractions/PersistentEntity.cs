using Buratino.Entities;
using Buratino.Models.Attributes;

using System.ComponentModel.DataAnnotations;

namespace Buratino.Entities.Abstractions
{
    public abstract class PersistentEntity : NamedEntity
    {
        [Display(Name = "Удален")]
        [HidedProperty]
        public virtual bool IsDeleted { get; set; }

        [Display(Name = "Дата создания")]
        [HidedProperty]
        public virtual DateTime TimeStamp { get; set; } = DateTime.Now;

        [Display(Name = "Автор")]
        [HidedProperty]
        public virtual Account Account { get; set; }
    }
}