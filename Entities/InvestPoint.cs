using Buratino.Entities.Abstractions;

namespace Buratino.Entities
{
    public class InvestPoint : EntityBase
    {
        public virtual InvestSource Source { get; set; }

        /// <summary>
        /// Баланс
        /// </summary>
        public virtual decimal Amount { get; set; }

        public virtual string Description { get; set; }

        public override string ToString()
        {
            return $"[{Id}]\t{Amount}\t({Source})";
        }
    }
}
