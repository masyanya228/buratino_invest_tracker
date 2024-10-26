using Buratino.DI;
using Buratino.Entities;
using Buratino.Enums;
using Buratino.Models.DomainService;

namespace Buratino.Models.Services
{
    public class InvestCalcService
    {
        public IRepository<InvestSource> SourceRepository { get; set; } = Container.GetRepository<InvestSource>();
        public IRepository<CategoryOfCapital> CategoryOfCapitalRepository { get; set; } = Container.GetRepository<CategoryOfCapital>();

        public string CEB(params string[] parts)
        {
            if (parts.Length == 2
                && int.TryParse(parts[0], out int months)
                && decimal.TryParse(parts[1], out decimal ps))
                return CalcCapitalEffectiveBase(PeriodType.Monthly, 50000, DateTime.Now, DateTime.Now.AddMonths(months), ps).ToString();
            return null;
        }

        public decimal CalcCapitalEffectiveBase(PeriodType periodVyplat, decimal lastBalance, DateTime timeStamp, DateTime endStamp, decimal ps)
        {
            var capProfit = CalcCapitalProfit(periodVyplat, lastBalance, timeStamp, endStamp, ps);
            return Math.Round(capProfit / lastBalance / ((decimal)(endStamp - timeStamp).TotalDays / 365.25m) * 100, 1);
        }

        public decimal CalcCapitalProfit(PeriodType periodVyplat, decimal lastBalance, DateTime timeStamp, DateTime endStamp, decimal ps)
        {
            if (periodVyplat == PeriodType.Monthly)
            {
                var startMoney = lastBalance;
                var profit = 0m;
                DateTime periodStart = timeStamp;
                while (periodStart < endStamp)
                {
                    profit += (startMoney + profit) * ps / 12 / 100.0m;
                    periodStart = periodStart.AddMonths(1);
                }
                return profit;
            }
            throw new NotImplementedException();
        }

        public IEnumerable<CategoryOfCapital> GetCapitalCategories()
        {
            var sources = SourceRepository.GetAll()
                .Where(x => !x.IsClosed)
                .ToArray();
            var result = new List<CategoryOfCapital>();
            foreach (var source in sources)
            {
                var lastCats = CategoryOfCapitalRepository.GetAll()
                    .Where(x => x.Source.Id == source.Id)
                    .ToArray()
                    .GroupBy(x => x.CategoryOfCapitalEnum)
                    .Select(x => x.OrderByDescending(y => y.TimeStamp).First());
                result.AddRange(lastCats.Any()
                    ? lastCats
                    : CalcCatiptalCategories(source));
            }
            return result;
        }

        public IEnumerable<CategoryOfCapital> CalcCatiptalCategories(InvestSource source)
        {
            if (source.Category is CategoryEnum.TInvestAuto)
            {
                return source.TInvestAccountId != 0
                    ? new TInvestService().GetCategoriresOfCapital(source)
                        .Select(x =>
                        {

                            CategoryOfCapitalRepository.Insert(x);
                            return x;
                        })
                    : throw new ArgumentNullException(nameof(source.TInvestAccountId));
            }
            else
            {
                var capCat = new CategoryOfCapital()
                {
                    Source = source,
                    CategoryOfCapitalEnum = source.Category,
                    Value = source.LastBalance,
                };
                if (capCat.CategoryOfCapitalEnum == CategoryEnum.DepositAuto)
                    capCat.CategoryOfCapitalEnum = CategoryEnum.Deposit;
                CategoryOfCapitalRepository.Insert(capCat);
                return new List<CategoryOfCapital>() { capCat };
            }
        }
    }
}
