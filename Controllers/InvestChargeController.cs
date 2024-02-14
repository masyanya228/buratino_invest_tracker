using Buratino.Entities;
using Buratino.Models.Xtensions;
using Buratino.ViewDto.Crud;

namespace Buratino.Controllers
{
    public class InvestChargeController : EntityController<InvestCharge>
    {
        protected override IEnumerable<ColumnSettings> GetColumnSettings()
        {
            return typeof(InvestCharge).GetPropertyList("Source", "Increment", "Description").Select(x => new ColumnSettings(x));
        }
    }
}
