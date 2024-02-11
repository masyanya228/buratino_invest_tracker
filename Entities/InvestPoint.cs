using Buratino.Entities.Abstractions;

namespace Buratino.Entities
{
    public class InvestPoint : PersistentEntity
    {
        public virtual InvestSource Source { get; set; }

        /// <summary>
        /// Отметка о балансе
        /// </summary>
        public virtual decimal Amount { get; set; }

        public override string Name { get => base.Name; set => base.Name = value; }
    }
}
