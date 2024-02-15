using Buratino.Entities;
using Buratino.Models.Xtensions;
using Buratino.ViewDto.Crud;

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
