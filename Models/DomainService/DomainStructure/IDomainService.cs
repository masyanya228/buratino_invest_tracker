using Buratino.Entities.Abstractions;

namespace Buratino.Models.DomainService.DomainStructure
{
    public interface IDomainService<T> where T : IEntityBase
    {
        bool Delete(T entity);

        T Get(Guid id);

        IQueryable<T> GetAll();

        T Save(T entity);
    }
}