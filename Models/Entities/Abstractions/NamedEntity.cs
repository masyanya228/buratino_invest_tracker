using System.ComponentModel.DataAnnotations;

namespace Buratino.Models.Entities.Abstractions
{
    public class NamedEntity : EntityBase
    {
        [Display(Name = "Наименование")]
        public virtual string Name { get; set; }
    }
}