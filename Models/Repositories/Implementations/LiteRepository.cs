using Buratino.Entities.Abstractions;
using Buratino.LiteDBContext;
using Buratino.Models.DomainService;

using LiteDB;
using LiteDB.Queryable;

namespace Buratino.Models.Repositories.Implementations
{
    public class LiteRepository<T> : RepositoryBase<T> where T : IEntityBase
    {
        public ILiteCollection<T> Collection;

        public LiteRepository()
        {
            Collection = DBContext.Database.GetCollection<T>();
            Collection = IncludeAll(Collection);
        }

        public override IQueryable<T> GetAll()
        {
            return Collection.AsQueryable();
        }

        public override T Get(Guid id)
        {
            var entity = GetAll().SingleOrDefault(x => x.Id == id);
            if (entity == null)
                throw new ArgumentException(nameof(id) + id);
            return entity;
        }

        public override T Insert(T entity)
        {
            Collection.Insert(entity);
            return entity;
        }

        public override T Update(T entity)
        {
            Collection.Update(entity);
            return entity;
        }

        public override bool Delete(T entity)
        {
            return Delete(entity.Id);
        }

        public override bool Delete(Guid id)
        {
            return Collection.Delete(id);
        }

        protected ILiteCollection<T> IncludeAll(ILiteCollection<T> collection)
        {
            IEnumerable<string> refMembers = GetDBRefs(collection);
            foreach (var item in refMembers)
            {
                BsonExpression keySelector = BsonExpression.Create($"{item}");
                collection = collection.Include(keySelector);
            }
            return collection;
        }

        [Obsolete]
        private ILiteQueryable<T> RemoveDeletedIncludes(ILiteQueryable<T> collection)
        {
            return collection;
            IEnumerable<string> refMembers = GetDBRefs(Collection);
            foreach (var item in refMembers)
            {
                var bexp = BsonExpression.Create($"{item}.IsDeleted=false");
                collection = collection.Where(bexp);
            }
            return collection;
        }

        protected IEnumerable<string> GetDBRefs(ILiteCollection<T> collection)
        {
            return collection.EntityMapper.Members.Where(x => x.IsDbRef).Select(x => x.FieldName);
        }
    }
}