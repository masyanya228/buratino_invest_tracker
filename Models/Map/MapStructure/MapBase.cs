﻿using Buratino.DI;
using Buratino.Entities.Abstractions;

using System.Linq.Expressions;

namespace Buratino.Models.Map.MapStructure
{
    public abstract class MapBase<T> where T : IEntityBase
    {
        public IMap<T> Map { get; set; } = Container.Resolve<IMap<T>>();

        public abstract void Setup();

        public void Reference<Tprop>(Expression<Func<T, Tprop>> expression, string tableName = null)
        {
            Map.Reference(expression, tableName);
        }
    }
}
