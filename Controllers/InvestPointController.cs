using Buratino.Entities;

using Buratino.ViewDto.Crud;
using Buratino.Xtensions;

namespace Buratino.Controllers
{
    public class InvestPointController : EntityController<InvestPoint>
    {
        protected override IEnumerable<ColumnSettings> GetColumnSettings()
        {
            return typeof(InvestPoint).GetPropertyList("Source", "Amount", "Description").Select(x => new ColumnSettings(x));
        }
    }
}
