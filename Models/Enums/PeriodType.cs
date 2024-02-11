using System.ComponentModel.DataAnnotations;

namespace Buratino.Enums
{
    public enum PeriodType
    {
        [Display(Name = "Ежедневно")]
        Daily = 0,

        [Display(Name = "Ежемесячно")]
        Monthly = 1,

        [Display(Name = "Ежеквартально")]
        Quater = 2,

        [Display(Name = "В конце срока")]
        AtTheEnd = 3
    }
}
