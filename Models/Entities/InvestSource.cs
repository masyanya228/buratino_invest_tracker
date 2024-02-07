using Buratino.Models.Entities.Abstractions;
using Buratino.Models.Enums;

namespace Buratino.Models.Entities
{
    public class InvestSource : PersistentEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Эффективная годовая ставка
        /// </summary>
        public decimal EffectiveBase { get; set; }

        /// <summary>
        /// Дата расчета ставки
        /// </summary>
        public DateTime EffectiveBaseTimeStamp { get; set; }

        /// <summary>
        /// Последний баланс
        /// </summary>
        public decimal LastBalance { get; set; }

        /// <summary>
        /// Всего инвестировано
        /// </summary>
        public decimal TotalCharged { get; set; }

        /// <summary>
        /// Тип дохода
        /// </summary>
        public ProfitType ProfitType { get; set; }

        public override string ToString()
        {
            return $"[{Id}]\t{Name}";
        }
    }
}
