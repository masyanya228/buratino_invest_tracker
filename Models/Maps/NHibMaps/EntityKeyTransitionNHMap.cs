﻿using Buratino.Entities;
using Buratino.Models.Map.NHibMaps;

public class EntityKeyTransitionNHMap : NHSubclassClassMap<EntityKeyTransition>
{
    public EntityKeyTransitionNHMap() : base()
    {
        Map(x => x.EntityType);
        Map(x => x.OldId);
        Map(x => x.NewId);
    }
}