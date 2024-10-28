using Buratino.Entities;

namespace Buratino.Models.Services.Dto
{
    public class HistoryOpsDiff
    {
        public IEnumerable<InvestCharge> Added{ get; set; }
        public IEnumerable<InvestCharge> Removed { get; set; }
        public IEnumerable<InvestCharge> Edited { get; set; }
    }
}