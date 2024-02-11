﻿using Buratino.Entities.Abstractions;
using Buratino.Models.Map.NHibMaps;

public class EntityBaseMap : NHClassMap<EntityBase>
{
    public EntityBaseMap()
    {
        UseUnionSubclassForInheritanceMapping();

        Id(item => item.Id)
            .Not.Nullable()
            .Default("gen_random_uuid()")
            .GeneratedBy.Guid();
    }
}