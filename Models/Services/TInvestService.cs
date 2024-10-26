using Buratino.API;
using Buratino.Entities;

namespace Buratino.Models.Services
{
    public class TInvestService
    {
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

        public IEnumerable<CategoryOfCapital> GetCategoriresOfCapital(InvestSource source)
        {
            var api = new TInvestAPI();
            var positions = api.GetPortfolio(source.TInvestAccountId);

            return new List<CategoryOfCapital>()
            {
                new CategoryOfCapital()
                {
                    Source = source,
                    CategoryOfCapitalEnum = Enums.CategoryEnum.Currency,
                    Value = positions.TotalAmountCurrencies.Units
                },
                new CategoryOfCapital()
                {
                    Source = source,
                    CategoryOfCapitalEnum = Enums.CategoryEnum.Share,
                    Value = positions.TotalAmountShares.Units
                },
                new CategoryOfCapital()
                {
                    Source = source,
                    CategoryOfCapitalEnum = Enums.CategoryEnum.ETF,
                    Value = positions.TotalAmountEtf.Units
                },
                new CategoryOfCapital()
                {
                    Source = source,
                    CategoryOfCapitalEnum = Enums.CategoryEnum.Bond,
                    Value = positions.TotalAmountBonds.Units
                },
            };
        }
    }
}
