﻿using Buratino.Entities.Abstractions;
using Buratino.Models.DomainService.DomainStructure;

namespace Buratino.Models.DomainService
{
    public class DefaultDomainService<T> : DomainServiceBase<T> where T : IEntityBase
    {
    }
}
