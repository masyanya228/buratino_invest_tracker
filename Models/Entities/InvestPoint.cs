using Buratino.Models.Entities.Abstractions;

namespace Buratino.Models.Entities
{
    public class InvestPoint : PersistentEntity
    {
        public InvestSource Source { get; set; }

        /// <summary>
        /// Баланс
        /// </summary>
        public decimal Amount { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return $"[{Id}]\t{Amount}\t({Source})";
        }
    }
}
