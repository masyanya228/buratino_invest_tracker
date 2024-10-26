using Buratino.Entities;

using Buratino.ViewDto.Crud;
using Buratino.Xtensions;

namespace Buratino.Controllers
{
    public class InvestSourceController : EntityController<InvestSource>
    {

        protected override IEnumerable<ColumnSettings> GetColumnSettings()
        {
            return typeof(InvestSource).GetPropertyList("Name", "ProfitType", "SourceGroup").Select(x => new ColumnSettings(x));
        }
    }
}
