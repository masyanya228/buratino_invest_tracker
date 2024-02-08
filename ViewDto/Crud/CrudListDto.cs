using Buratino.Models.Entities.Abstractions;

namespace Buratino.ViewDto.CRUD
{
    public class CrudListDto<T> where T : IEntityBase
    {
        public string ListName { get; set; }
        public IEnumerable<T> EntityList { get; set; }
    }
}
