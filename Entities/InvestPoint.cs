using Buratino.Entities.Abstractions;

using System.ComponentModel.DataAnnotations;

namespace Buratino.Entities
{
    public class InvestPoint : PersistentEntity
    {
        [Display(Name = "Инвест. поток")]
        public virtual InvestSource Source { get; set; }

        [Display(Name = "Баланс")]
        public virtual decimal Amount { get; set; }

        [Display(Name = "Заметка")]
        public virtual string Description { get; set; }
    }
}
