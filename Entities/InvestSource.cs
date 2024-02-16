using Buratino.Entities.Abstractions;
using Buratino.Enums;
using Buratino.Models.Attributes;
using Buratino.Models.Enums;

using System.ComponentModel.DataAnnotations;

namespace Buratino.Entities
{
    public class InvestSource : NamedEntity
    {
        public virtual IList<InvestPoint> Points { get; set; }

        public virtual IList<InvestCharge> Charges { get; set; }

        public virtual IList<InvestComment> Comments { get; set; }

        [Display(Name = "Описание инвестиции")]
        public virtual string Description { get; set; }

        /// <summary>
        /// Эффективная годовая ставка
        /// </summary>
        [Display(Name = "Эффективная годовая ставка")]
        [HidenProperty]
        public virtual decimal EffectiveBase { get; set; }

        /// <summary>
        /// Дата расчета ставки
        /// </summary>
        [Display(Name = "Дата расчета ставки")]
        [HidenProperty]
        public virtual DateTime EffectiveBaseTimeStamp { get; set; }

        /// <summary>
        /// Последний баланс
        /// </summary>
        [Display(Name = "Последний баланс")]
        [HidenProperty]
        public virtual decimal LastBalance { get; set; }

        /// <summary>
        /// Всего инвестировано
        /// </summary>
        [Display(Name = "Всего инвестировано")]
        [HidenProperty]
        public virtual decimal TotalCharged { get; set; }

        /// <summary>
        /// Тип дохода
        /// </summary>
        [Display(Name = "Тип дохода")]
        public virtual ProfitType ProfitType { get; set; }

        /// <summary>
        /// Группа инвестиций
        /// </summary>
        [Display(Name = "Группа инвестиций")]
        public virtual SourceGroup SourceGroup { get; set; }

        /// <summary>
        /// Бансковский вклад
        /// </summary>
        [Display(Name = "Банковски вклад")]
        public virtual bool IsBankVklad { get; set; }

        /// <summary>
        /// Срок вклада
        /// </summary>
        [Display(Name = "Срок вклада")]
        public virtual DateTime BVEndStamp { get; set; }

        /// <summary>
        /// Процентная ставка (годовых)
        /// </summary>
        [Display(Name = "Процентная ставка")]
        public virtual decimal BVPS { get; set; }

        /// <summary>
        /// Капитализация процентов
        /// </summary>
        [Display(Name = "Капитализация процентов")]
        public virtual bool BVCapitalisation { get; set; }

        /// <summary>
        /// Период выплат процентов 
        /// </summary>
        [Display(Name = "Период выплат процентов")]
        public virtual PeriodType BVPeriodVyplat { get; set; } = PeriodType.Monthly;
    }
}
