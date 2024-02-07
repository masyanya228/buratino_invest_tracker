using Buratino.Models.DomainService.DomainStructure;
using Buratino.Models.Entities.Abstractions;

namespace Buratino.Models.DomainService
{
    public class DefaultDomainService<T> : DomainServiceBase<T> where T : IEntityBase
    {
    }
}
