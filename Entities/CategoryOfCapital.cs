using Buratino.Entities.Abstractions;
using Buratino.Enums;

namespace Buratino.Entities
{
    public class CategoryOfCapital : EntityBase
    {
        public virtual InvestSource Source { get; set; }

        public virtual CategoryEnum CategoryOfCapitalEnum { get; set; }

        /// <summary>
        /// Капитала в этом источнике дохода с этой категорией
        /// </summary>
        public virtual decimal Value { get; set; }
    }
}
