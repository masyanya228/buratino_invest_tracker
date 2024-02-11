using System;

namespace Buratino.Entities.Abstractions
{
    public interface IEntityBase
    {
        public Guid Id { get; set; }
    }
}