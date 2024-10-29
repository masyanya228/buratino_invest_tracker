using Buratino.DI;
using Buratino.Entities;
using Buratino.Models.DomainService;

namespace Buratino.Models.Services
{
    public class InvestIncomeService
    {
        public IRepository<DepositAutoPoint> DepositAutoPointRepository { get; set; } = Container.GetRepository<DepositAutoPoint>();

        public IRepository<InvestCharge> ChargeRepository { get; set; } = Container.GetRepository<InvestCharge>();

        public IRepository<InvestPoint> PointRepository { get; set; } = Container.GetRepository<InvestPoint>();
        
        public IRepository<InvestSource> SourceRepository { get; set; } = Container.GetRepository<InvestSource>();

        /// <summary>
        /// Возвращает всю историю
        /// </summary>
        /// <returns></returns>
        public Dictionary<InvestSource, List<decimal>> GetAllIncomeByAllTime()
        {
            Dictionary<InvestSource, List<decimal>> totalPerMonth = new();
            foreach (var source in SourceRepository.GetAll())
            {
                totalPerMonth.Add(source, GetIncomeByAllTime(source).ToList());
            }
            return totalPerMonth;
        }

        /// <summary>
        /// Возвращает историю доходов за последние месяцы
        /// </summary>
        /// <param name="source"></param>
        /// <param name="months"></param>
        /// <returns></returns>
        public IEnumerable<decimal> GetIncomeByLastMonths(InvestSource source, int months)
        {
            var endPoint = source.Points.OrderByDescending(x => x.TimeStamp).FirstOrDefault()?.TimeStamp ?? DateTime.Now;
            endPoint = endPoint.Date.AddDays(-endPoint.Day + 1);
            var startPeriod = endPoint.AddMonths(-months + 1);
            return GetIncomeByPeriod(source, startPeriod, months);
        }

        /// <summary>
        /// Возвращает историю доходов за год
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<decimal> GetIncomeByYear(InvestSource source)
        {
            var startPeriod = DateTime.Now.Date.AddDays(-source.TimeStamp.Day + 1).AddMonths(-12);
            return GetIncomeByPeriod(source, startPeriod, 12);
        }

        /// <summary>
        /// Возвращает историю доходов за всё время
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<decimal> GetIncomeByAllTime(InvestSource source)
        {
            var endPoint = source.Points.OrderByDescending(x => x.TimeStamp).FirstOrDefault()?.TimeStamp ?? DateTime.Now;
            endPoint = endPoint.Date.AddDays(-endPoint.Day + 1);
            var months = (int)Math.Ceiling(endPoint.Subtract(source.TimeStamp).TotalDays / 365 * 12) + 1;
            return GetIncomeByPeriod(source, source.TimeStamp.Date.AddDays(-source.TimeStamp.Day + 1), months);
        }

        /// <summary>
        /// Возвращает историю доходов для любого источника
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startPeriod"></param>
        /// <param name="months"></param>
        /// <returns></returns>
        public IEnumerable<decimal> GetIncomeByPeriod(InvestSource source, DateTime startPeriod, int months)
        {
            return source.Category == Enums.CategoryEnum.DepositAuto
                ? GetDepositAutoIncomeByPeriod(source, startPeriod, months)
                : GetPointIncomeByPeriod(source, startPeriod, months);
        }

        /// <summary>
        /// Возвращает историю доходов за указанный период для источника с типом DepositAuto
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startPeriod"></param>
        /// <param name="months"></param>
        /// <returns></returns>
        public IEnumerable<decimal> GetDepositAutoIncomeByPeriod(InvestSource source, DateTime startPeriod, int months)
        {
            List<decimal> perMonth = new();
            for (var month = 1; month <= months; month++)
            {
                var lap = GetDepositAutoIncomeByMonth(source, startPeriod.AddMonths(month - 1));
                perMonth.Add(lap?.Increment ?? 0);
            }
            return perMonth.Reverse<decimal>();
        }

        /// <summary>
        /// Возвращает итоговый доход для источника с типом DepositAuto
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startPeriod"></param>
        /// <returns></returns>
        public DepositAutoPoint GetDepositAutoIncomeByMonth(InvestSource source, DateTime startPeriod)
        {
            if (startPeriod > source.BVEndStamp)
                return null;

            if (source.IsClosed && startPeriod > source.CloseDate)
                return null;

            var endPeriod = startPeriod.Date.AddMonths(1);
            var daysInMonth = (decimal)(endPeriod - startPeriod).TotalDays;
            var daysInYear = DateTime.IsLeapYear(startPeriod.Year)
                ? 366
                : 365;

            var curAutoPoint = DepositAutoPointRepository.GetAll()
                .Where(x => x.Source.Id == source.Id && x.TimeStamp == endPeriod)
                .FirstOrDefault();

            if (curAutoPoint != null)
            {
                return curAutoPoint;
            }

            var lastAutoPoint = DepositAutoPointRepository.GetAll()
                .Where(x => x.Source.Id == source.Id && x.TimeStamp == startPeriod)
                .OrderByDescending(x => x.TimeStamp)
                .FirstOrDefault();

            if (lastAutoPoint is null)
            {
                if (source.TimeStamp > startPeriod && source.TimeStamp < endPeriod)
                {
                    lastAutoPoint = new DepositAutoPoint(0, 0, 0, false, 0)
                    {
                        TimeStamp = startPeriod,
                        Source = source,
                    };
                    DepositAutoPointRepository.Insert(lastAutoPoint);
                }
                else
                {
                    lastAutoPoint = GetDepositAutoIncomeByMonth(source, startPeriod.AddMonths(-1));
                }
            }

            var lCharges = ChargeRepository.GetAll()
                .Where(x => x.Source.Id == source.Id && x.TimeStamp > startPeriod && x.TimeStamp < endPeriod)
                .OrderBy(x => x.TimeStamp)
                .ToArray();

            var increment = 0m;
            var startBalance = lastAutoPoint.Amount;
            var totalBalance = 0m;
            var prevBalance = startBalance;
            var midBalance = 0m;

            var prevTimeStamp = lastAutoPoint.TimeStamp;
            foreach (var charge in lCharges)
            {
                var length = (decimal)(charge.TimeStamp - prevTimeStamp).TotalDays;
                totalBalance += prevBalance * length;

                prevBalance += charge.Increment;
                prevTimeStamp = charge.TimeStamp;
            }
            if (lCharges.Any())
            {
                var length = (decimal)(endPeriod - prevTimeStamp).TotalDays;
                totalBalance += prevBalance * length;
                midBalance = totalBalance / daysInMonth;
            }
            else
            {
                midBalance = prevBalance;
            }
            increment = midBalance * source.BVPS / 100 / daysInYear * daysInMonth;

            var lap = new DepositAutoPoint(prevBalance, midBalance, source.BVPS, source.BVCapitalisation, increment)
            {
                TimeStamp = endPeriod,
                Source = source
            };
            DepositAutoPointRepository.Insert(lap);
            return lap;
        }

        /// <summary>
        /// Возвращает историю доходов за указанный период
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startPeriod"></param>
        /// <param name="months"></param>
        /// <returns></returns>
        public IEnumerable<decimal> GetPointIncomeByPeriod(InvestSource source, DateTime startPeriod, int months)
        {
            List<decimal> perMonth = new();
            var year = startPeriod;

            for (var month = 0; month < months; month++)
            {
                var startMonth = year.AddMonths(month);
                var endMonth = year.AddMonths(month + 1);
                decimal income = GetPointIncomeOnMonth(source, startMonth, endMonth);
                perMonth.Add(income);
            }
            return perMonth.Reverse<decimal>();
        }

        /// <summary>
        /// Возвращает итоговый доход за период
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startMonth"></param>
        /// <param name="endMonth"></param>
        /// <returns></returns>
        private decimal GetPointIncomeOnMonth(InvestSource source, DateTime startMonth, DateTime endMonth)
        {
            var diff = 0m;
            var charged = 0m;
            var income = 0m;
            IEnumerable<InvestPoint> points = source.Points.OrderBy(x => x.TimeStamp).ToArray();
            IEnumerable<InvestCharge> charges = source.Charges.OrderBy(x => x.TimeStamp).ToArray();

            var firstPoint = points
                .Where(x => x.TimeStamp < startMonth)
                .OrderByDescending(x => x.TimeStamp)
                .FirstOrDefault();

            var midPoints = points
                .Where(x => x.TimeStamp > startMonth && x.TimeStamp < endMonth)
                .OrderBy(x => x.TimeStamp)
                .ToArray();

            var lastPoint = points
                .Where(x => x.TimeStamp > endMonth)
                .OrderBy(x => x.TimeStamp)
                .FirstOrDefault();

            if (firstPoint is null)
            {
                if (charges.Where(x => x.TimeStamp < startMonth).Any())
                {
                    firstPoint = new InvestPoint()
                    {
                        Source = source,
                        TimeStamp = charges
                            .Where(x => x.TimeStamp < startMonth)
                            .OrderBy(x => x.TimeStamp)
                            .Last().TimeStamp,
                        Amount = charges
                            .Where(x => x.TimeStamp < startMonth)
                            .Sum(x => x.Increment),
                    };
                }
            }
            if (firstPoint != null)
            {
                if (midPoints.Any())
                {
                    diff = midPoints.First().Amount - firstPoint.Amount;
                    charged = charges
                        .Where(x => x.TimeStamp > firstPoint.TimeStamp && x.TimeStamp < midPoints.First().TimeStamp)
                        .Sum(x => x.Increment);
                    income += (diff - charged)
                        / (decimal)midPoints.First().TimeStamp.Subtract(firstPoint.TimeStamp).TotalDays
                        * (decimal)midPoints.First().TimeStamp.Subtract(startMonth).TotalDays;


                    diff = midPoints.Last().Amount - midPoints.First().Amount;
                    charged = charges
                        .Where(x => x.TimeStamp > midPoints.First().TimeStamp && x.TimeStamp < midPoints.Last().TimeStamp)
                        .Sum(x => x.Increment);
                    income += diff - charged;

                    if (lastPoint != null)
                    {
                        diff = lastPoint.Amount - midPoints.Last().Amount;
                        charged = charges
                            .Where(x => x.TimeStamp > midPoints.Last().TimeStamp && x.TimeStamp < lastPoint.TimeStamp)
                            .Sum(x => x.Increment);
                        income += (diff - charged)
                            / (decimal)lastPoint.TimeStamp.Subtract(midPoints.Last().TimeStamp).TotalDays
                            * (decimal)endMonth.Subtract(midPoints.Last().TimeStamp).TotalDays;
                    }
                }
                else if (lastPoint != null)
                {
                    diff = lastPoint.Amount - firstPoint.Amount;
                    charged = charges
                        .Where(x => x.TimeStamp > firstPoint.TimeStamp && x.TimeStamp < lastPoint.TimeStamp)
                        .Sum(x => x.Increment);
                    income += (diff - charged)
                        / (decimal)lastPoint.TimeStamp.Subtract(firstPoint.TimeStamp).TotalDays
                        * DateTime.DaysInMonth(endMonth.Year, endMonth.Month);
                }
                else
                {
                    //0
                }
            }
            else
            {
                if (midPoints.Any())
                {
                    diff = midPoints.Last().Amount - midPoints.First().Amount;
                    charged = charges
                        .Where(x => x.TimeStamp > midPoints.First().TimeStamp && x.TimeStamp < midPoints.Last().TimeStamp)
                        .Sum(x => x.Increment);
                    income += diff - charged;

                    if (lastPoint != null)
                    {
                        diff = lastPoint.Amount - midPoints.Last().Amount;
                        charged = charges
                            .Where(x => x.TimeStamp > midPoints.Last().TimeStamp && x.TimeStamp < lastPoint.TimeStamp)
                            .Sum(x => x.Increment);
                        income += (diff - charged)
                            / (decimal)lastPoint.TimeStamp.Subtract(midPoints.Last().TimeStamp).TotalDays
                            * (decimal)endMonth.Subtract(midPoints.Last().TimeStamp).TotalDays;
                    }
                }
                else if (charges.Where(x => x.TimeStamp < endMonth).Any() && lastPoint != null)
                {
                    var startPoint = new InvestPoint()
                    {
                        Source = source,
                        TimeStamp = charges
                            .Where(x => x.TimeStamp < endMonth)
                            .OrderBy(x => x.TimeStamp)
                            .First().TimeStamp,
                        Amount = charges
                            .Where(x => x.TimeStamp < endMonth)
                            .OrderBy(x => x.TimeStamp)
                            .First().Increment,
                    };

                    diff = lastPoint.Amount - startPoint.Amount;
                    charged = charges
                        .Where(x => x.TimeStamp > startPoint.TimeStamp && x.TimeStamp < lastPoint.TimeStamp)
                        .Sum(x => x.Increment);
                    income += (diff - charged)
                            / (decimal)lastPoint.TimeStamp.Subtract(startPoint.TimeStamp).TotalDays
                            * (decimal)endMonth.Subtract(startPoint.TimeStamp).TotalDays;
                }
            }
            decimal benefitPerMonth = GetBenefitsOnPeriod(source, startMonth, endMonth);
            return income + benefitPerMonth;
        }

        /// <summary>
        /// Возвращает сумму бенефитов за период
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startMonth"></param>
        /// <param name="endMonth"></param>
        /// <returns></returns>
        private decimal GetBenefitsOnPeriod(InvestSource source, DateTime startMonth, DateTime endMonth)
        {
            return source.Benifits.Where(x => x.TimeStamp > startMonth && x.TimeStamp < endMonth).Sum(x => x.Value);
        }
    }
}
