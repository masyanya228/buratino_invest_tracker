using Buratino.Entities.Abstractions;

namespace Buratino.ViewDto.Crud
{
    public class CrudDetailsDto
    {
        public string EntityName { get; set; }
        public IEntityBase Entity { get; set; }
    }
}
