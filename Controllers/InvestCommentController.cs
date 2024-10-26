using Buratino.Entities;

using Buratino.ViewDto.Crud;
using Buratino.Xtensions;

namespace Buratino.Controllers
{
    public class InvestCommentController : EntityController<InvestComment>
    {
        protected override IEnumerable<ColumnSettings> GetColumnSettings()
        {
            return typeof(InvestComment).GetPropertyList("Source", "TimeStamp", "Description").Select(x => new ColumnSettings(x));
        }
    }
}
