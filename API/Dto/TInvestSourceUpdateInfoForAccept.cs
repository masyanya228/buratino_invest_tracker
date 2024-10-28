using Buratino.Models.Services.Dto;

namespace Buratino.API.Dto
{
    public class TInvestSourceUpdateInfoForAccept
    {
        public Guid SourceId { get; set; }

        public HistoryOpsDiff HistoryOpsDiff{ get; set; }

        public decimal NewValue { get; set; }
    }
}
