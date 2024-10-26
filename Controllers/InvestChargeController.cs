using Buratino.Entities;
using Buratino.ViewDto.Crud;
using Buratino.Xtensions;

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
