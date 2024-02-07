namespace Buratino.Models.DomainService
{
    public interface IRepository<T>
    {
        T Get(long id);
        IQueryable<T> GetAll();
        T Insert(T entity);
        T Update(T entity);
        bool Delete(T entity);
    }
}