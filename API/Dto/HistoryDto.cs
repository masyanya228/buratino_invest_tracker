using Buratino.Enums;

namespace Buratino.API.Dto
{
    public class HistoryDto
    {
        public Guid Id { get; set; }

        public HistoryItemType ItemType { get; set; }

        public string Title {  get; set; }
    }
}
