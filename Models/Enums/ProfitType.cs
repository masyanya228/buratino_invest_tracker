using System.ComponentModel.DataAnnotations;

namespace Buratino.Models.Enums
{
    public enum ProfitType
    {
        [Display(Name = "С начислением прибыли")]
        WithIncome = 0,

        [Display(Name = "Без начисления прибыли")]
        WithoutIncome = 1
    }
}
