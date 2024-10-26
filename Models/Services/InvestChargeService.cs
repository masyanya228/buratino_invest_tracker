using Buratino.DI;
using Buratino.Entities;
using Buratino.Models.DomainService.DomainStructure;
using Buratino.Models.Services.Dto;

namespace Buratino.Models.Services
{
    public class InvestChargeService : DomainServiceBase<InvestCharge>
    {
        /// <summary>
        /// Апи
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public PlanChargeDto ChargesByYear(int target)
        {
            var curYear = DateTime.Now.Year;
            var startDay = new DateTime(curYear, 1, 1);
            var endDay = new DateTime(curYear, 1, 1).AddYears(1);
            var test = endDay.Subtract(startDay).TotalDays;
            var yearProgress = (DateTime.Now - startDay).TotalDays / (endDay - startDay).TotalDays;

            var all = GetAll()
                .Where(x => x.TimeStamp.Year == curYear)
                .ToArray();

            var body = string.Join("\r\n", all
                .OrderByDescending(x => x.TimeStamp)
                .Select(x => $"{x.Source}\t{x.Increment}\t{x.TimeStamp}"));

            return new PlanChargeDto()
            {
                AmountOfCharges = all.Count(),
                TotalFact = all.Sum(x => x.Increment),
                TodayPlan = Math.Round(target * yearProgress, 0),
                ChargeProgress = Math.Round(all.Sum(x => x.Increment) / target * 100),
                YearProgress = Math.Round(yearProgress * 100)
            };
        }

        public override InvestCharge Save(InvestCharge entity)
        {
            var res = base.Save(entity);
            Container.Get<InvestCalcService>().CalcCatiptalCategories(Container.GetRepository<InvestSource>().Get(entity.Source.Id));
            return res;
        }

    }
}
