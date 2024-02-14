using Buratino.DI;
using Buratino.Entities.Abstractions;
using Buratino.Models.DomainService;
using Buratino.Models.Repositories.Implementations.Postgres;

using NHibernate;

namespace Buratino.Models.Repositories.Implementations
{
    public class PGRepository<T> : RepositoryBase<T> where T : IEntityBase
    {
        public ISessionFactory SessionFactory { get; set; } = Container.Resolve<IPGSessionFactory>().SessionFactory;

        public PGRepository()
        {

        }

        public override IQueryable<T> GetAll()
        {
            var session = SessionFactory.OpenSession();
            return session.Query<T>();
        }

        public override bool Delete(T entity)
        {
            using (var session = SessionFactory.OpenSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    if (entity is PersistentEntity persistent)
                    {
                        persistent.IsDeleted = true;
                        session.Update(entity);
                    }
                    else
                    {
                        session.Delete(entity);
                    }
                    trans.Commit();
                    return true;
                }
            }
        }

        public override T Get(Guid id)
        {
            using (var session = SessionFactory.OpenSession())
            {
                return session.Get<T>(id);
            }
        }

        public override T Insert(T entity)
        {
            using (var session = SessionFactory.OpenSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    session.Save(entity);
                    trans.Commit();
                    return entity;
                }
            }
        }

        public override T Update(T entity)
        {
            using (var session = SessionFactory.OpenSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    session.Update(entity);
                    trans.Commit();
                    return entity;
                }
            }
        }
    }
}