using Buratino.DI;
using Buratino.Entities;
using Buratino.Repositories.RepositoryStructure;
using Buratino.Services;

using LiteDB;

using System.Reflection;

public class MigrateData
{
    private IRepository<EntityKeyTransition> TransRep = Container.GetRepository<EntityKeyTransition>();
    public MigrateData()
    {
        MigrateNewData();
    }

    private void MigrateNewData()
    {
/*InvestSource
InvestCharge
InvestPoint
InvestBenifit
InvestDepositAutoPoint
CategoryOfCapital*/
        var oldDB = new LiteDatabase("D:\\source2\\Buratino_InvestTracker\\bin\\Debug\\net6.0\\DB1");
        oldDB.Mapper.EnumAsInteger = true;

        //source
        TransitSources(oldDB);
        TransitCharges(oldDB);
        TransitPoints(oldDB);
        TransitBenefits(oldDB);
    }

    private void TransitSources(LiteDatabase oldDB)
    {
        var source = oldDB.GetCollection("InvestSource", BsonAutoId.Int64);
        var entityRep = Container.GetRepository<InvestSource>();
        var items = source.FindAll();
        foreach (var item in items)
        {
            var trans = new EntityKeyTransition()
            {
                EntityType = nameof(InvestSource),
                OldId = item["_id"].AsInt64
            };
            var exist = TransRep.GetAll().Where(x => x.EntityType == trans.EntityType && x.OldId == trans.OldId).SingleOrDefault();
            if (exist == null)
            {
                TransRep.Insert(trans);
            }
            else
            {
                continue;
            }

            var newSource = new InvestSource();
            var entityType = typeof(InvestSource);
            foreach (var field in item.GetElements())
            {
                if (field.Key == "_id")
                    continue;
                entityType
                    .GetProperty(field.Key, BindingFlags.Public | BindingFlags.Instance)
                    .SetValue(newSource, field.Value.RawValue);
            }

            if (exist == null)
            {
                entityRep.Insert(newSource);
                trans.NewId = newSource.Id;
                TransRep.Update(trans);
            }
            else
            {
                newSource.Id = exist.NewId;
                entityRep.Update(newSource);
            }
        }
    }

    private void TransitBenefits(LiteDatabase oldDB)
    {
        var source = oldDB.GetCollection("InvestBenifit", BsonAutoId.Int64);
        var entityRep = Container.GetRepository<InvestBenifit>();
        var items = source.FindAll();
        var entityType = typeof(InvestBenifit);
        foreach (var item in items)
        {
            var trans = new EntityKeyTransition()
            {
                EntityType = entityType.Name,
                OldId = item["_id"].AsInt64
            };
            var exist = TransRep.GetAll()
                .Where(x => x.EntityType == trans.EntityType && x.OldId == trans.OldId)
                .SingleOrDefault();
            if (exist == null)
            {
                TransRep.Insert(trans);
            }
            else
            {
                continue;
            }

            var newBenifit = new InvestBenifit();
            foreach (var field in item.GetElements())
            {
                if (field.Key == "_id")
                    continue;

                if (field.Key == "Source")
                {
                    var oldRefId = field.Value["$id"].AsInt64;
                    newBenifit.Source = new InvestSource() { Id = GetNewId("InvestSource", oldRefId) };
                    continue;
                }
                entityType
                    .GetProperty(field.Key, BindingFlags.Public | BindingFlags.Instance)
                    .SetValue(newBenifit, field.Value.RawValue);
            }

            if (exist == null)
            {
                entityRep.Insert(newBenifit);
                trans.NewId = newBenifit.Id;
                TransRep.Update(trans);
            }
            else
            {
                newBenifit.Id = exist.NewId;
                entityRep.Update(newBenifit);
            }
        }
    }

    private void TransitPoints(LiteDatabase oldDB)
    {
        var source = oldDB.GetCollection("InvestPoint", BsonAutoId.Int64);
        var entityRep = Container.GetRepository<InvestPoint>();
        var items = source.FindAll();
        var entityType = typeof(InvestPoint);
        foreach (var item in items)
        {
            var trans = new EntityKeyTransition()
            {
                EntityType = entityType.Name,
                OldId = item["_id"].AsInt64
            };
            var exist = TransRep.GetAll()
                .Where(x => x.EntityType == trans.EntityType && x.OldId == trans.OldId)
                .SingleOrDefault();
            if (exist == null)
            {
                TransRep.Insert(trans);
            }
            else
            {
                continue;
            }

            var newCharge = new InvestPoint();
            foreach (var field in item.GetElements())
            {
                if (field.Key == "_id")
                    continue;

                if (field.Key == "Source")
                {
                    var oldRefId = field.Value["$id"].AsInt64;
                    newCharge.Source = new InvestSource() { Id = GetNewId("InvestSource", oldRefId) };
                    continue;
                }
                entityType
                    .GetProperty(field.Key, BindingFlags.Public | BindingFlags.Instance)
                    .SetValue(newCharge, field.Value.RawValue);
            }

            if (exist == null)
            {
                entityRep.Insert(newCharge);
                trans.NewId = newCharge.Id;
                TransRep.Update(trans);
            }
            else
            {
                newCharge.Id = exist.NewId;
                entityRep.Update(newCharge);
            }
        }
    }

    private void TransitCharges(LiteDatabase oldDB)
    {
        var source = oldDB.GetCollection("InvestCharge", BsonAutoId.Int64);
        var entityRep = Container.GetRepository<InvestCharge>();
        var items = source.FindAll();
        var entityType = typeof(InvestCharge);
        foreach (var item in items)
        {
            var trans = new EntityKeyTransition()
            {
                EntityType = entityType.Name,
                OldId = item["_id"].AsInt64
            };
            var exist = TransRep.GetAll()
                .Where(x => x.EntityType == trans.EntityType && x.OldId == trans.OldId)
                .SingleOrDefault();
            if (exist == null)
            {
                TransRep.Insert(trans);
            }
            else
            {
                continue;
            }

            var newCharge = new InvestCharge();
            foreach (var field in item.GetElements())
            {
                if (field.Key == "_id")
                    continue;

                if (field.Key == "Source")
                {
                    var oldRefId = field.Value["$id"].AsInt64;
                    newCharge.Source = new InvestSource() { Id = GetNewId("InvestSource", oldRefId) };
                    continue;
                }
                entityType
                    .GetProperty(field.Key, BindingFlags.Public | BindingFlags.Instance)
                    .SetValue(newCharge, field.Value.RawValue);
            }

            if (exist == null)
            {
                entityRep.Insert(newCharge);
                trans.NewId = newCharge.Id;
                TransRep.Update(trans);
            }
            else
            {
                newCharge.Id = exist.NewId;
                entityRep.Update(newCharge);
            }
        }
    }

    private Guid GetNewId(string entityType, long oldId)
    {
        return TransRep.GetAll()
            .Where(x => x.EntityType == entityType && x.OldId == oldId)
            .SingleOrDefault()?.NewId ?? throw new NullReferenceException("Эта сущность еще не перенесена в новую БД");
    }
}