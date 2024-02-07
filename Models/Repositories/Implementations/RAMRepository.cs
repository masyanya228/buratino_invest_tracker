using Buratino.Models.DomainService;
using System.Collections.Concurrent;
using Buratino.Models.Entities.Abstractions;

namespace Buratino.Models.Repositories.Implementations
{
    public class RAMRepository<T> : RepositoryBase<T> where T : IEntityBase
    {
        public ConcurrentDictionary<long, T> Collection;

        public RAMRepository()
        {
            Collection = new ConcurrentDictionary<long, T>();
        }

        public override bool Delete(T entity) => Collection.TryRemove(entity.Id, out _);

        public override T Get(long id) => Collection.TryGetValue(id, out T value)
            ? value
            : throw new Exception("Не получилось добавить элемент в репозиторий");

        public override IQueryable<T> GetAll() => Collection.Values.AsQueryable();

        public override T Insert(T entity)
        {
            long newId = GetNextKey();
            entity.Id = newId;
            return Collection.TryAdd(newId, entity)
                ? entity
                : throw new Exception("Не получилось добавить элемент в репозиторий");
        }

        public override T Update(T entity)
        {
            Collection[entity.Id] = entity;
            return entity;
        }
        private long GetNextKey() => Collection.Keys.Any()
            ? Collection.Keys.Max() + 1
            : 1;
    }
}
