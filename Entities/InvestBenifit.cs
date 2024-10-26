using Buratino.Entities;
using Buratino.Entities.Abstractions;

namespace Buratino.Services
{
    public class InvestBenifit : EntityBase
    {
        public virtual InvestSource Source { get; set; }

        /// <summary>
        /// Сумма бенефита
        /// </summary>
        public virtual decimal Value { get; set; }

        /// <summary>
        /// Описание бенефита
        /// </summary>
        public virtual string Description { get; set; }

        public override string ToString()
        {
            return $"[{Id}]\t{Value}\t({Source})";
        }
    }
}