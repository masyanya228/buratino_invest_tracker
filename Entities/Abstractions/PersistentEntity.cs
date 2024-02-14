using Buratino.Models.Attributes;

using System.ComponentModel.DataAnnotations;

namespace Buratino.Entities.Abstractions
{
    public abstract class PersistentEntity : EntityBase
    {
        [Display(Name = "Удален")]
        [HidenProperty]
        public virtual bool IsDeleted { get; set; }

        [Display(Name = "Дата создания")]
        [HidenProperty]
        public virtual DateTime TimeStamp { get; set; } = DateTime.Now;

        [Display(Name = "Автор")]
        [HidenProperty]
        public virtual Account Account { get; set; }
    }
}