using Buratino.DI;
using Buratino.Entities.Abstractions;

namespace Buratino.Models.DomainService.DomainStructure
{
    public class PersistentDomainService<T> : DomainServiceBase<T> where T : PersistentEntity, IEntityBase
    {
        public PersistentDomainService()
        {
            Repository = Container.Resolve<IRepository<T>>("PersistentEntity");
            DInject();
        }

        public override IQueryable<T> GetAll()
        {
            return Repository.GetAll().Where(x => x.IsDeleted == false) as IQueryable<T>;
        }
    }
}
