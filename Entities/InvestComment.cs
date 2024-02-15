using Buratino.Entities.Abstractions;

using System.ComponentModel.DataAnnotations;

namespace Buratino.Entities
{
    public class InvestComment : PersistentEntity
    {
        [Display(Name = "Инвест. поток")]
        public virtual InvestSource Source { get; set; }

        [Display(Name = "Заметка")]
        public virtual string Description { get; set; }
    }
}
