using Buratino.Entities.Abstractions;
using Buratino.Enums;
using Buratino.Services;

using LiteDB;

namespace Buratino.Entities
{
    public class InvestSource : PersistentEntity
    {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        /// <summary>
        /// Эффективная годовая ставка
        /// </summary>
        public virtual decimal EffectiveBase { get; set; }

        /// <summary>
        /// Дата расчета ставки
        /// </summary>
        public virtual DateTime EffectiveBaseTimeStamp { get; set; }

        /// <summary>
        /// Последний баланс
        /// </summary>
        public virtual decimal LastBalance { get; set; }

        /// <summary>
        /// Всего инвестировано
        /// </summary>
        public virtual decimal TotalCharged { get; set; }

        /// <summary>
        /// Тип дохода
        /// </summary>
        public virtual ProfitType ProfitType { get; set; } = ProfitType.WithIncome;

        /// <summary>
        /// Бансковский вклад
        /// </summary>
        public virtual bool IsBankVklad { get; set; }
        
        /// <summary>
        /// Срок закрытия вклада
        /// </summary>
        public virtual DateTime BVEndStamp { get; set; }

        /// <summary>
        /// Процентная ставка (годовых)
        /// </summary>
        public virtual decimal BVPS { get; set; }

        /// <summary>
        /// Капитализация процентов
        /// </summary>
        public virtual bool BVCapitalisation { get; set; }

        /// <summary>
        /// Период выплат процентов 
        /// </summary>
        public virtual PeriodType BVPeriodVyplat { get; set; }

        /// <summary>
        /// Поток закрыт
        /// </summary>
        public virtual bool IsClosed { get; set; }

        /// <summary>
        /// Всего получено на выходе
        /// </summary>
        public virtual decimal TotalRecharged { get; set; }

        /// <summary>
        /// Дата закрытия вклада
        /// </summary>
        public virtual DateTime CloseDate { get; set; }

        /// <summary>
        /// Id аккаунта т инвестиций
        /// </summary>
        public virtual long TInvestAccountId { get; set; }

        /// <summary>
        /// Категория капитала
        /// </summary>
        public virtual CategoryEnum Category { get; set; }

        [BsonIgnore]
        public virtual IEnumerable<InvestPoint> Points { get; set; }

        [BsonIgnore]
        public virtual IEnumerable<InvestCharge> Charges { get; set; }
        
        [BsonIgnore]
        public virtual IEnumerable<InvestBenifit> Benifits { get; set; }

        [BsonIgnore]
        public virtual IEnumerable<InvestComment> Comments { get; set; }

        public override string ToString()
        {
            return $"[{Id}\t{Name}]";
        }

        public virtual string PrintList()
        {
            return this.IsBankVklad
                ? $"{Name} - {LastBalance:C} {Math.Round(EffectiveBase, 1)}% ({Math.Round(BVEndStamp.Subtract(DateTime.Now).TotalDays / 30.5, 1)})"
                : $"{Name} - {LastBalance:C} {Math.Round(EffectiveBase, 1)}%";
        }
    }
}
