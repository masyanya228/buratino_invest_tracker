using Buratino.Entities;

namespace Buratino.Services
{
    public class InvestSourceService
    {
        public InvestSource AddSource(params string[] parts)
        {
            if (parts.Length == 1)
                return AddSource(parts[0]);
            else if (parts.Length == 2)
                return AddSource(parts[0], parts[1]);
            else if (parts.Length == 3 && DateTime.TryParse(parts[2], out DateTime dateTime))
                return AddSource(parts[0], parts[1], dateTime);
            return null;
        }

        public string Sources(params string[] parts)
        {
            if (parts.Length == 0)
                return string.Join("\r\n", DBContext.InvestPoints.FindAll()
                    .GroupBy(x => x.Source.Id)
                    .Select(x => $"[{x.First().Source.Id}]\t{x.First().Source.Name}\t{x.OrderBy(y => y.TimeStamp).Last().Amount}"));
            return null;
        }

        public string Stats(params string[] parts)
        {
            if (parts.Length == 1)
                return Stats(parts[0]);
            return null;
        }
        public string StatsAll(params string[] parts)
        {
            if (parts.Length == 0)
                return StatsAll();
            else if (parts.Length == 1)
                return StatsAll(int.Parse(parts[0]));
            return null;
        }

        protected InvestSource AddSource(string name)
        {
            var source = new InvestSource()
            {
                Name = name,
            };
            DBContext.InvestSources.Insert(source);
            return source;
        }

        protected InvestSource AddSource(string name, string description)
        {
            var source = new InvestSource()
            {
                Name = name,
                Description = description
            };
            DBContext.InvestSources.Insert(source);
            return source;
        }

        protected InvestSource AddSource(string name, string description, DateTime dateTime)
        {
            var source = new InvestSource()
            {
                Name = name,
                Description = description,
                TimeStamp = dateTime
            };
            DBContext.InvestSources.Insert(source);
            return source;
        }

        /// <summary>
        /// Расчитывает эффективную годовую ставку
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected string Stats(string name)
        {
            var source = Find(name);
            var allCharges = DBContext.InvestCharges.Query().Where(x => x.Source.Id == source.Id).OrderBy(x => x.TimeStamp).ToArray();
            if (allCharges.Length == 0)
                throw new Exception("Вы пока не пополняли поток");

            var allPoints = DBContext.InvestPoints.Query().Where(x => x.Source.Id == source.Id).OrderBy(x => x.TimeStamp).ToArray();

            var startDate = allCharges.FirstOrDefault().TimeStamp;

            DateTime lastPoint = allPoints.LastOrDefault()?.TimeStamp ?? DateTime.MinValue;
            DateTime lastCharge = allCharges.Last().TimeStamp;

            var endDate = lastPoint;
            if (endDate < lastCharge)
                endDate = lastCharge;

            var lastBalance = allPoints.LastOrDefault()?.Amount ?? 0;
            if (lastPoint < lastCharge)
                lastBalance = lastBalance + allCharges.Where(x => x.TimeStamp > lastPoint).Sum(x => x.Increment);

            var profit = lastBalance - allCharges.Sum(x => x.Increment);

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

            var midBalance = totalMoneys / totalDays;
            var periodOfInvesting = (decimal)endDate.Subtract(startDate).TotalDays;

            decimal effectiveBase = Math.Round(profit / periodOfInvesting * 365.25m / midBalance * 100, 1);

            source.EffectiveBase = effectiveBase;
            source.EffectiveBaseTimeStamp = DateTime.Now;
            source.LastBalance = lastBalance;
            source.TotalCharged = curMoney;

            DBContext.InvestSources.Update(source);

            return effectiveBase + "%";
        }

        /// <summary>
        /// Расчитывает показатели доходности по всем потокам
        /// </summary>
        /// <returns></returns>
        protected string StatsAll()
        {
            var sources = DBContext.InvestSources.FindAll().ToArray();
            return StatsAll(sources);
        }

        protected string StatsAll(int v)
        {
            var sources = DBContext.InvestSources.Query().Where(x => ((int)x.ProfitType) == v).ToArray();
            return StatsAll(sources);
        }

        private string StatsAll(InvestSource[] sources)
        {
            foreach (var item in sources)
            {
                if (item.EffectiveBaseTimeStamp.Date > DateTime.Now.Date)
                {
                    continue;
                }
                Stats(item.Name);
            }
            sources = DBContext.InvestSources.FindAll().ToArray();

            var body = "";
            var totalPerMonth = 0m;
            var totalPerYear = 0m;
            foreach (var item in sources)
            {
                decimal perMonth = item.LastBalance * item.EffectiveBase / 100 / 12;
                decimal perYear = item.LastBalance * item.EffectiveBase / 100;

                totalPerMonth += perMonth;
                totalPerYear += perYear;

                body += $"({item})" +
                    $"\t{item.EffectiveBase}%" +
                    $"\t{item.TotalCharged} вложено" +
                    $"\t{Math.Round(perMonth)} в мес" +
                    $"\t{Math.Round(perYear)} в год\r\n";
            }

            return $"{body}" +
                $"\tИтого: {Math.Round(totalPerYear / sources.Sum(x => x.TotalCharged) * 100, 2)}%" +
                $"\t{sources.Sum(x => x.TotalCharged)} вложено" +
                $"\t{Math.Round(totalPerMonth)} в мес" +
                $"\t{Math.Round(totalPerYear)} в год";
        }

        protected bool CheckUniq(string name)
        {
            return DBContext.InvestSources.Query().Where(x => x.Name.ToLower() == name).Exists();
        }

        public InvestSource Find(string name)
        {
            return DBContext.InvestSources.Query()
                .Where(x => x.Name.ToLower() == name.ToLower()).SingleOrDefault()
                ?? throw new ArgumentOutOfRangeException("Такой поток не найден");
        }
    }
}
