using Buratino.DI;
using Buratino.Models.DomainService.DomainStructure;
using Buratino.Models.Entities;
using LiteDB;

namespace Buratino
{
    public class DBContext
    {
        public static LiteDatabase db = new LiteDatabase("DB1");

        public static ILiteCollection<InvestSource> InvestSources;
        public static ILiteCollection<InvestCharge> InvestCharges;
        public static ILiteCollection<InvestPoint> InvestPoints;

        public static void Init()
        {
            BsonMapper.Global.EnumAsInteger = true;
            BsonMapper.Global.Entity<InvestCharge>().DbRef(x => x.Source, "InvestSources");
            BsonMapper.Global.Entity<InvestPoint>().DbRef(x => x.Source, "InvestSources");


            InvestSources = db.GetCollection<InvestSource>();
            InvestCharges = db.GetCollection<InvestCharge>();
            InvestPoints = db.GetCollection<InvestPoint>();

        }

        internal static Account GetAccById(long id)
        {
            return Container.Resolve<IDomainService<Account>>().Get(id);
        }
    }
}
