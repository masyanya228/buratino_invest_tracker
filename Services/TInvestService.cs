using Buratino.API;
using Buratino.Entities;
using Buratino.Enums;
using Buratino.Services.Dto;

namespace Buratino.Services
{
    public class TInvestService
    {
        /// <summary>
        /// Возвращает оценку стоимости портфеля
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public decimal GetAccountValue(long accountId)
        {
            var api = new TInvestAPI();
            var positions = api.GetPortfolio(accountId);
            var sumNominal = 0m;
            var sumOther = 0m;
            var text = "";
            foreach (var security in positions.Positions)
            {
                if (security.InstrumentType == "bond")
                {
                    var bond = api.GetBondBy(security.InstrumentUid);
                    sumNominal += security.Quantity.Units * bond.Instrument.Nominal.GetInRub();
                    text += $"{security.InstrumentUid}\t{bond.Instrument.Name}\t{security.Quantity.Units * bond.Instrument.Nominal.GetInRub()}\r\n";
                }
            }

            sumOther = positions.TotalAmountCurrencies.Units
                + positions.TotalAmountShares.Units
                + positions.TotalAmountEtf.Units
                + positions.TotalAmountFutures.Units
                + positions.TotalAmountOptions.Units
                + positions.TotalAmountSp.Units;
            return sumNominal + sumOther;
        }

        /// <summary>
        /// Возвращает распределение капитала по категориям инвестиций
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<CategoryOfCapital> GetCategoriresOfCapital(InvestSource source)
        {
            var api = new TInvestAPI();
            var positions = api.GetPortfolio(source.TInvestAccountId);

            return new List<CategoryOfCapital>()
            {
                new CategoryOfCapital()
                {
                    Source = source,
                    CategoryOfCapitalEnum = CategoryEnum.Currency,
                    Value = positions.TotalAmountCurrencies.Units
                },
                new CategoryOfCapital()
                {
                    Source = source,
                    CategoryOfCapitalEnum = CategoryEnum.Share,
                    Value = positions.TotalAmountShares.Units
                },
                new CategoryOfCapital()
                {
                    Source = source,
                    CategoryOfCapitalEnum = CategoryEnum.ETF,
                    Value = positions.TotalAmountEtf.Units
                },
                new CategoryOfCapital()
                {
                    Source = source,
                    CategoryOfCapitalEnum = CategoryEnum.Bond,
                    Value = positions.TotalAmountBonds.Units
                },
            };
        }

        /// <summary>
        /// Возвращает изменении в пополнениях/выводах
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public HistoryOpsDiff GetHistoryDiff(InvestSource source)
        {
            var accountId = source.TInvestAccountId;
            var charges = source.Charges;

            var added = new List<InvestCharge>();
            var removed = new List<InvestCharge>();
            var edited = new List<InvestCharge>();

            var api = new TInvestAPI();
            var opHis = api.GetOperations(accountId.ToString());
            var ops = opHis.Items.Where(x => x.Type == OperationType.OPERATION_TYPE_INPUT || x.Type == OperationType.OPERATION_TYPE_OUTPUT)
                .Select(x => new KeyValuePair<DateTime, decimal>(x.Date.Date, x.Payment.Units))
                .GroupBy(x => x.Key)
                .Select(x => new KeyValuePair<DateTime, decimal>(x.Key, x.Sum(y => y.Value)))
                .Where(x => x.Value != 0)
                .ToArray();

            added = ops.Where(x => !charges.Any(y => y.TimeStamp.Date == x.Key)).Select(x => new InvestCharge()
            {
                TimeStamp = x.Key,
                Increment = x.Value,
            }).ToList();

            removed = charges.Where(x => !ops.Any(y => y.Key == x.TimeStamp.Date))
                .ToList();

            edited = ops.Select(x =>
            {
                var charge = charges.FirstOrDefault(y => y.TimeStamp.Date == x.Key && y.Increment != x.Value);
                if (charge is null) return null;
                charge.Increment = x.Value;
                return charge;
            }
            ).Where(x => x is not null)
            .ToList();

            return new HistoryOpsDiff()
            {
                Added = added,
                Edited = edited,
                Removed = removed
            };
        }
    }
}
