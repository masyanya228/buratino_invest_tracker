using System.ComponentModel.DataAnnotations;

namespace Buratino.Entities.Abstractions
{
    public abstract class NamedEntity : PersistentEntity
    {
        [Display(Name = "Наименование")]
        public virtual string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}