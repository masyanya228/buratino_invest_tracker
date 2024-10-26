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

        public Dictionary<InvestSource, List<decimal>> CalcIncomeAll()
        {
            Dictionary<InvestSource, List<decimal>> totalPerMonth = new();
            var year = DateTime.Now.AddDays(-DateTime.Now.Day).AddMonths(-12);
            foreach (var source in SourceRepository.GetAll())
            {
                totalPerMonth.Add(source, GetIncomeByAllTime(source).ToList());
            }
            return totalPerMonth;
        }

        public IEnumerable<decimal> GetIncomeByLastMonths(InvestSource source, int months)
        {
            var startPeriod = DateTime.Now.Date.AddDays(-source.TimeStamp.Day + 1).AddMonths(-months);
            return GetIncomeByPeriod(source, startPeriod, months);
        }

        public IEnumerable<decimal> GetIncomeByYear(InvestSource source)
        {
            var startPeriod = DateTime.Now.Date.AddDays(-source.TimeStamp.Day + 1).AddMonths(-12);
            return GetIncomeByPeriod(source, startPeriod, 12);
        }

        public IEnumerable<decimal> GetIncomeByAllTime(InvestSource source)
        {
            var months = (int)(DateTime.Now.Date.AddDays(-DateTime.Now.Day + 1).Subtract(source.TimeStamp).TotalDays / 365 * 12);
            return GetIncomeByPeriod(source, source.TimeStamp.Date.AddDays(-source.TimeStamp.Day + 1), months);
        }

        public IEnumerable<decimal> GetIncomeByPeriod(InvestSource source, DateTime startPeriod, int months)
        {
            return source.Category == Enums.CategoryEnum.DepositAuto
                ? GetDepositAutoIncomeByPeriod(source, startPeriod, months)
                : GetIncomeByPeriodByPoints(source, startPeriod, months);
        }

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

        public IEnumerable<decimal> GetIncomeByPeriodByPoints(InvestSource source, DateTime startPeriod, int months)
        {
            List<decimal> perMonth = new();
            var year = startPeriod;

            var charges = ChargeRepository.GetAll();
            var points = PointRepository.GetAll();

            var lCharges = charges
                .Where(x => x.Source.Id == source.Id)
                .OrderBy(x => x.TimeStamp)
                .ToArray();

            var lPoints = points
                .Where(x => x.Source.Id == source.Id)
                .OrderBy(x => x.TimeStamp)
                .ToArray();

            for (var month = 1; month < months; month++)
            {
                var startMonth = year.AddMonths(month - 1);
                var endMonth = year.AddMonths(month);

                var diff = 0m;
                var charged = 0m;
                var income = 0m;
                var firstPoint = lPoints
                    .Where(x => x.TimeStamp < startMonth)
                    .OrderByDescending(x => x.TimeStamp)
                    .FirstOrDefault();

                var midPoints = lPoints
                    .Where(x => x.TimeStamp > startMonth && x.TimeStamp < endMonth)
                    .OrderBy(x => x.TimeStamp)
                    .ToArray();

                var lastPoint = lPoints
                    .Where(x => x.TimeStamp > endMonth)
                    .OrderBy(x => x.TimeStamp)
                    .FirstOrDefault();

                if (firstPoint is null)
                {
                    if (lCharges.Where(x => x.TimeStamp < startMonth).Any())
                    {
                        firstPoint = new InvestPoint()
                        {
                            Source = source,
                            TimeStamp = lCharges
                                .Where(x => x.TimeStamp < startMonth)
                                .OrderBy(x => x.TimeStamp)
                                .Last().TimeStamp,
                            Amount = lCharges
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
                        charged = lCharges.Where(x => x.TimeStamp > firstPoint.TimeStamp && x.TimeStamp < midPoints.First().TimeStamp)
                                .Sum(x => x.Increment);
                        income += (diff - charged)
                            / (decimal)midPoints.First().TimeStamp.Subtract(firstPoint.TimeStamp).TotalDays
                            * (decimal)midPoints.First().TimeStamp.Subtract(startMonth).TotalDays;


                        diff = midPoints.Last().Amount - midPoints.First().Amount;
                        charged = lCharges
                            .Where(x => x.TimeStamp > midPoints.First().TimeStamp && x.TimeStamp < midPoints.Last().TimeStamp)
                            .Sum(x => x.Increment);
                        income += diff - charged;

                        if (lastPoint != null)
                        {
                            diff = lastPoint.Amount - midPoints.Last().Amount;
                            charged = lCharges.Where(x => x.TimeStamp > midPoints.Last().TimeStamp && x.TimeStamp < lastPoint.TimeStamp)
                                .Sum(x => x.Increment);
                            income += (diff - charged)
                                / (decimal)lastPoint.TimeStamp.Subtract(midPoints.Last().TimeStamp).TotalDays
                                * (decimal)endMonth.Subtract(midPoints.Last().TimeStamp).TotalDays;
                        }
                    }
                    else if (lastPoint != null)
                    {
                        diff = lastPoint.Amount - firstPoint.Amount;
                        charged = lCharges.Where(x => x.TimeStamp > firstPoint.TimeStamp && x.TimeStamp < lastPoint.TimeStamp)
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
                        charged = lCharges
                            .Where(x => x.TimeStamp > midPoints.First().TimeStamp && x.TimeStamp < midPoints.Last().TimeStamp)
                            .Sum(x => x.Increment);
                        income += diff - charged;

                        if (lastPoint != null)
                        {
                            diff = lastPoint.Amount - midPoints.Last().Amount;
                            charged = lCharges
                                .Where(x => x.TimeStamp > midPoints.Last().TimeStamp && x.TimeStamp < lastPoint.TimeStamp)
                                .Sum(x => x.Increment);
                            income += (diff - charged)
                                / (decimal)lastPoint.TimeStamp.Subtract(midPoints.Last().TimeStamp).TotalDays
                                * (decimal)endMonth.Subtract(midPoints.Last().TimeStamp).TotalDays;
                        }
                    }
                    else if (lCharges.Where(x => x.TimeStamp < endMonth).Any() && lastPoint != null)
                    {
                        var startPoint = new InvestPoint()
                        {
                            Source = source,
                            TimeStamp = lCharges
                                .Where(x => x.TimeStamp < endMonth)
                                .OrderBy(x => x.TimeStamp)
                                .First().TimeStamp,
                            Amount = lCharges
                                .Where(x => x.TimeStamp < endMonth)
                                .OrderBy(x => x.TimeStamp)
                                .First().Increment,
                        };

                        diff = lastPoint.Amount - startPoint.Amount;
                        charged = lCharges.Where(x => x.TimeStamp > startPoint.TimeStamp && x.TimeStamp < lastPoint.TimeStamp)
                                .Sum(x => x.Increment);
                        income += (diff - charged)
                                / (decimal)lastPoint.TimeStamp.Subtract(startPoint.TimeStamp).TotalDays
                                * (decimal)endMonth.Subtract(startPoint.TimeStamp).TotalDays;
                    }
                }
                perMonth.Add(income);
            }
            return perMonth.Reverse<decimal>();
        }
    }
}
