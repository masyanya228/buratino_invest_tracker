using Buratino.Models.Entities.Abstractions;

namespace Buratino.Models.DomainService.DomainStructure
{
    public interface IDomainService<T> where T : IEntityBase
    {
        bool Delete(T entity);

        T Get(long id);

        IEnumerable<T> GetAll();

        T Save(T entity);
    }
}