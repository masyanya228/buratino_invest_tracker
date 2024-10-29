using Buratino.DI;
using Buratino.Entities;
using Buratino.Models.DomainService;
using Buratino.Models.DomainService.DomainStructure;
using Buratino.Models.Services.Dto;
using Buratino.Services;

namespace Buratino.Models.Services
{
    public class InvestSourceService : DomainServiceBase<InvestSource>
    {
        public IRepository<InvestPoint> PointRepository { get; set; } = Container.GetRepository<InvestPoint>();
        public IRepository<InvestBenifit> BenifitRepository { get; set; } = Container.GetRepository<InvestBenifit>();
        public IRepository<InvestCharge> ChargeRepository { get; set; } = Container.GetRepository<InvestCharge>();

        /// <summary>
        /// Расчитывает эффективную годовую ставку
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string Stats(Guid id)
        {
            var source = Repository.Get(id);

            if (source.IsBankVklad)
            {
                return CalcStatsForBankVklad(source);
            }
            else
            {
                return CalcStatsByPoints(source);
            }
        }

        //todo - улучшить алгоритм расчета заработка
        private string CalcStatsForBankVklad(InvestSource source)
        {
            var allCharges = ChargeRepository.GetAll().Where(x => x.Source.Id == source.Id).OrderBy(x => x.TimeStamp).ToArray();
            if (allCharges.Length == 0)
                throw new Exception("Вы пока не пополняли поток");

            source.EffectiveBase = source.BVPS;
            source.EffectiveBaseTimeStamp = DateTime.Now;
            source.LastBalance = allCharges.Sum(x => x.Increment);
            source.TotalCharged = allCharges.Where(x => x.Increment > 0).Sum(x => x.Increment);

            if (source.BVCapitalisation)
            {
                source.EffectiveBase = new InvestCalcService().CalcCapitalEffectiveBase(source.BVPeriodVyplat, source.LastBalance, source.TimeStamp, source.BVEndStamp, source.BVPS);
            }

            Save(source);

            return source.EffectiveBase + "%";
        }

        private string CalcStatsByPoints(InvestSource source)
        {
            var allCharges = ChargeRepository.GetAll().Where(x => x.Source.Id == source.Id).OrderBy(x => x.TimeStamp).ToArray();

            if (allCharges.Length == 0)
            {
                source.LastBalance = 0;
                source.EffectiveBase = 0;
                Save(source);
                return "";
            }

            var allPoints = PointRepository.GetAll().Where(x => x.Source.Id == source.Id).OrderBy(x => x.TimeStamp).ToArray();

            var allBenefits = BenifitRepository.GetAll().Where(x => x.Source.Id == source.Id).OrderBy(x => x.TimeStamp).ToArray();

            var startDate = allCharges.FirstOrDefault().TimeStamp;

            DateTime lastPoint = allPoints.LastOrDefault()?.TimeStamp ?? DateTime.MinValue;
            DateTime lastCharge = allCharges.Last().TimeStamp;

            var endDate = lastPoint;
            if (endDate < lastCharge)
                endDate = lastCharge;

            var lastBalance = allPoints.LastOrDefault()?.Amount ?? 0;
            if (lastPoint < lastCharge)
                lastBalance = lastBalance + allCharges.Where(x => x.TimeStamp > lastPoint).Sum(x => x.Increment);

            var profit = lastBalance - allCharges.Sum(x => x.Increment) + allBenefits.Sum(x => x.Value);

            var days = 0m;
            var curMoney = 0m;
            var totalDays = 0m;
            var totalMoneys = 0m;
            for (int i = 0; i < allCharges.Length - 1; i++)
            {
                var prev = allCharges[i];
                var cur = allCharges[i + 1];
                days = (decimal)(cur.TimeStamp - prev.TimeStamp).TotalDays;

                curMoney += prev.Increment;

                totalDays += days;
                totalMoneys += days * curMoney;
            }

            curMoney += allCharges.Last().Increment;
            days = (decimal)endDate.Subtract(allCharges.Last().TimeStamp).TotalDays;
            totalDays += days;
            totalMoneys += days * curMoney;

            if (totalDays > 0)
            {
                var midBalance = totalMoneys / totalDays;
                var periodOfInvesting = (decimal)endDate.Subtract(startDate).TotalDays;

                decimal effectiveBase = Math.Round(profit / periodOfInvesting * 365.25m / midBalance * 100, 1);

                source.EffectiveBase = effectiveBase;
                source.EffectiveBaseTimeStamp = DateTime.Now;
                source.LastBalance = lastBalance;
                source.TotalCharged = allCharges.Sum(x => x.Increment);
            }
            else
            {
                source.EffectiveBase = 0;
                source.EffectiveBaseTimeStamp = DateTime.Now;
                source.LastBalance = lastBalance;
                source.TotalCharged = allCharges.Sum(x => x.Increment);
            }

            Save(source);

            return source.EffectiveBase + "%";
        }

        /// <summary>
        /// Расчитывает показатели доходности по всем потокам
        /// </summary>
        /// <returns></returns>
        public string StatsAll()
        {
            var sources = Repository.GetAll()
                .Where(x => !x.IsClosed)
                .ToArray();
            return StatsAll(sources);
        }

        /// <summary>
        /// Расчитывает показатели доходности по потокам, сохасно типу дохода (с начислениями и без)
        /// </summary>
        /// <param name="v">0-без начислений, 1-с начислениями</param>
        /// <returns></returns>
        protected string StatsAll(int v)
        {
            var sources = GetAll()
                .Where(x => (int)x.ProfitType == v)
                .Where(x => !x.IsClosed)
                .ToArray();
            return StatsAll(sources);
        }

        private string StatsAll(InvestSource[] sources)
        {
            foreach (var item in sources)
            {
                Stats(item.Id);
            }
            sources = Repository.GetAll()
                .ToArray()
                .Where(x => sources.Select(y => y.Id).Contains(x.Id))
                .ToArray();

            var body = "";
            var totalPerMonth = 0m;
            var totalPerYear = 0m;
            foreach (var item in sources)
            {
                decimal perYear = item.LastBalance * item.EffectiveBase / 100;
                decimal perMonth = perYear / 12;

                totalPerMonth += perMonth;
                totalPerYear += perYear;

                body += $"{item}" +
                    $"\t{item.EffectiveBase}%" +
                    $"\t{item.LastBalance} баланс" +
                    $"\t{Math.Round(perMonth)} в мес" +
                    $"\t{Math.Round(perYear)} в год\r\n";
            }

            return $"{body}" +
                $"\tИтого: {Math.Round(totalPerYear / sources.Sum(x => x.LastBalance) * 100, 2)}%" +
                $"\t{sources.Sum(x => x.LastBalance)} баланс" +
                $"\t{Math.Round(totalPerMonth)} в мес" +
                $"\t{Math.Round(totalPerYear)} в год";
        }

        protected string StatsBV()
        {
            var sources = GetAll().Where(x => !x.IsClosed && x.IsBankVklad).ToArray();

            var body = "";
            var totalPerMonth = 0m;
            var totalPerYear = 0m;
            foreach (var item in sources)
            {
                decimal perYear = item.LastBalance * item.EffectiveBase / 100;
                decimal perMonth = perYear / 12;

                totalPerYear += perYear;
                totalPerMonth += perMonth;

                var ps = item.BVCapitalisation
                    ? $"{item.BVPS}({item.EffectiveBase})%"
                    : $"{item.BVPS}%";

                body += $"{item}" +
                    $"\t{ps}" +
                    $"\t{item.LastBalance} баланс" +
                    $"\t{Math.Round(item.BVEndStamp.Subtract(item.TimeStamp).TotalDays / 365.25, 1)} лет" +
                    $"\tзакрытие {item.BVEndStamp.ToShortDateString()} ({Math.Round(item.BVEndStamp.Subtract(DateTime.Now).TotalDays / 30.5, 1)} мес)" +
                    $"\t{Math.Round(perMonth)} в мес" +
                    $"\t{Math.Round(perYear)} в год\r\n";
            }

            return $"{body}" +
                $"\tИтого: {Math.Round(totalPerYear / sources.Sum(x => x.LastBalance) * 100, 2)}%" +
                $"\t{sources.Sum(x => x.LastBalance)} баланс" +
                $"\t{Math.Round(totalPerMonth)} в мес" +
                $"\t{Math.Round(totalPerYear)} в год";
        }

        public bool IsExists(string name)
        {
            name = name.ToLower();
            return GetAll().Where(x => x.Name.ToLower() == name).Any();
        }

        public InvestSource Find(string name)
        {
            return GetAll()
                .Where(x => x.Name.ToLower() == name.ToLower()).SingleOrDefault()
                ?? throw new ArgumentOutOfRangeException("Такой поток не найден");
        }

        /// <summary>
        /// Апи
        /// </summary>
        /// <returns></returns>
        public InvestStatsDto GetStatsAll()
        {
            var sources = GetAll()
                .ToArray();

            var totalPerMonth = 0m;
            var totalPerYear = 0m;
            foreach (var item in sources)
            {
                decimal perYear = item.LastBalance * item.EffectiveBase / 100;
                decimal perMonth = perYear / 12;

                totalPerMonth += perMonth;
                totalPerYear += perYear;
            }

            decimal totalBalance = sources.Sum(x => x.LastBalance);
            totalBalance = totalBalance != 0
                ? totalBalance
                : 1;
            return new InvestStatsDto()
            {
                Percent = Math.Round(totalPerYear / totalBalance * 100, 2),
                Balance = totalBalance,
                IncomePerMonth = Math.Round(totalPerMonth),
                IncomePerYear = Math.Round(totalPerYear)
            };
        }


        /// <summary>
        /// Апи
        /// </summary>
        /// <returns></returns>
        public override IQueryable<InvestSource> GetAll()
        {
            StatsAll();
            return base.GetAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public override InvestSource Get(Guid v)
        {
            var source = base.Get(v);
            Stats(source.Id);
            return base.Get(v);
        }
    }
}
