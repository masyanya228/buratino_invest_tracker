using Buratino.Entities.Abstractions;

namespace Buratino.Entities
{
    public class InvestCharge : EntityBase
    {
        public virtual InvestSource Source { get; set; }

        /// <summary>
        /// Сумма пополнения
        /// </summary>
        public virtual decimal Increment { get; set; }

        public virtual string Description { get; set; }

        public override string ToString()
        {
            return $"[{Id}]\t{Increment}\t({Source})";
        }
    }
}
