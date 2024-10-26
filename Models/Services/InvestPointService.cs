using Buratino.DI;
using Buratino.Entities;
using Buratino.Models.DomainService.DomainStructure;

namespace Buratino.Models.Services
{
    public class InvestPointService : DomainServiceBase<InvestPoint>
    {
        public override InvestPoint Save(InvestPoint entity)
        {
            var res = base.Save(entity);
            Container.Get<InvestCalcService>().CalcCatiptalCategories(Container.GetRepository<InvestSource>().Get(entity.Source.Id));
            return res;
        }

    }
}
