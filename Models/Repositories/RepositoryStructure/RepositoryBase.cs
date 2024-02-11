using Buratino.Entities.Abstractions;

namespace Buratino.Models.DomainService
{
    public abstract class RepositoryBase<T> : IRepository<T> 
        where T : IEntityBase
    {
        public abstract bool Delete(T entity);
        
        public abstract T Get(Guid id);
        
        public abstract IQueryable<T> GetAll();

        public abstract T Insert(T entity);

        public abstract T Update(T entity);
    }
}