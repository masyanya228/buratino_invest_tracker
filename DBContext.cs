using Buratino.Entities;
using Buratino.Services;

using LiteDB;

namespace Buratino.LiteDBContext
{
    public class DBContext
    {
        public static LiteDatabase Database = new LiteDatabase("DB1");

        public static ILiteCollection<InvestSource> InvestSources;
        public static ILiteCollection<InvestCharge> InvestCharges;
        public static ILiteCollection<InvestPoint> InvestPoints;
        public static ILiteCollection<InvestBenifit> InvestBenifits;
        public static ILiteCollection<CategoryOfCapital> CategoryOfCapitals;
        public static ILiteCollection<DepositAutoPoint> InvestDepositAutoPoints;

        public static void Init()
        {
            BsonMapper.Global.EnumAsInteger = true;
            BsonMapper.Global.Entity<InvestCharge>().DbRef(x => x.Source, "InvestSource");
            BsonMapper.Global.Entity<InvestPoint>().DbRef(x => x.Source, "InvestSource");
            BsonMapper.Global.Entity<InvestBenifit>().DbRef(x => x.Source, "InvestSource");
            BsonMapper.Global.Entity<CategoryOfCapital>().DbRef(x => x.Source, "InvestSource");
            BsonMapper.Global.Entity<DepositAutoPoint>().DbRef(x => x.Source, "InvestSource");


            InvestSources = Database.GetCollection<InvestSource>();
            InvestCharges = Database.GetCollection<InvestCharge>();
            InvestPoints = Database.GetCollection<InvestPoint>();
            InvestBenifits = Database.GetCollection<InvestBenifit>();
            CategoryOfCapitals = Database.GetCollection<CategoryOfCapital>();
            InvestDepositAutoPoints = Database.GetCollection<DepositAutoPoint>();

        }
    }
}
