using Buratino.Models.Entities.Abstractions;

namespace Buratino.Models.Entities
{
    public class InvestCharge : PersistentEntity
    {
        public InvestSource Source { get; set; }

        public decimal Increment { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return $"[{Id}]\t{Increment}\t({Source})";
        }
    }
}
