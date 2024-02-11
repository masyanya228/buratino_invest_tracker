using Buratino.Entities.Abstractions;

namespace Buratino.ViewDto.Crud
{
    public class CrudListDto
    {
        public string ListName { get; set; }
        public Type EntityType { get; set; }
        public IEnumerable<IEntityBase> EntityList { get; set; }
    }
}
