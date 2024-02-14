using Buratino.Entities.Abstractions;

using System.ComponentModel.DataAnnotations;

namespace Buratino.Entities
{
    public class InvestCharge : PersistentEntity
    {
        [Display(Name = "Инвест. поток")]
        public virtual InvestSource Source { get; set; }

        [Display(Name = "Размер пополнения")]
        public virtual decimal Increment { get; set; }

        [Display(Name = "Заметка к пополнению")]
        public virtual string Description { get; set; }
    }
}
