﻿using Buratino.Analitics.Dto;
using Buratino.API;
using Buratino.API.Dto;
using Buratino.Entities;
using Buratino.Enums;
using Buratino.Services.Dto;
using Buratino.Xtensions;

namespace Buratino.Services
{
    public class TInvestService
    {
        public string GetBondAnalizeTable()
        {
            return Analize()
                .OrderBy(x => x.Instrument.Name)
                .Select(x => $"{x.Instrument.Name}\t{x.Instrument.Brand.LogoName}\t{x.Instrument.Ticker}\t{x.Instrument.Uid}\t{x.Instrument.Figi}\t{x.Instrument.RiskLevel}\t{x.GetYearlyIncome()}\t{x.Instrument.CouponQuantityPerYear}\t{x.EndDate.Subtract(DateTime.Now).TotalDays / 30}\t{x.Instrument.Nominal.Units}\t{x.TotalPrice}\t{x.Instrument.Sector}")
                .Join("\r\n");
        }

        public BondMetric[] Analize()
        {
            var api = new TInvestAPI();
            var obj = api.GetBonds();
            var now = DateTime.Now.AddMonths(6);

            var filtered = obj.Instruments
                .Where(x => x.Currency == "rub")
                .Where(x => x.CouponQuantityPerYear >= 4)
                .Where(x => x.MaturityDate > now && (x.CallDate.HasValue ? x.CallDate > now : true))
                .Where(x => x.Nominal.Units < 5000)
                .Where(x => x.BuyAvailableFlag)
                .Where(x => x.SellAvailableFlag)
                .Where(x => !x.FloatingCouponFlag)
                .Where(x => !x.PerpetualFlag)
                .Where(x => !x.PerpetualFlag)
                .Where(x => x.RealExchange.In("REAL_EXCHANGE_MOEX", "REAL_EXCHANGE_RTS"))
                .Where(x => !x.ForQualInvestorFlag)
                .Where(x => x.LiquidityFlag)
                .Where(x => x.RiskLevel.In("RISK_LEVEL_LOW", "RISK_LEVEL_MODERATE"))
                .Select(x => new BondMetric() { Instrument = x })
                .ToArray();

            var uids = filtered.Select(x => x.Instrument.Uid);

            //Расчет цены покупки
            var lastPrices = api.GetLastPrices(uids.ToArray());
            foreach (var item in filtered)
            {
                var instrument = lastPrices.LastPrices.FirstOrDefault(x => x.InstrumentUid == item.Instrument.Uid);
                item.LastPrice = instrument.Price;
            }

            //Купоны
            foreach (var uid in uids)
            {
                var coupons = api.GetBondCoupons(uid);
                var instrument = filtered.FirstOrDefault(x => x.Instrument.Uid == uid);
                instrument.Coupons = coupons;
            }
            return filtered;
        }

        public BondMetric[] GetMyBondsMetrics(long account)
        {
            var api = new TInvestAPI();
            var bonds = api.GetPortfolio(account);
            var now = DateTime.Now.AddMonths(6);

            var allBonds = api.GetBonds();

            var bondMetrics = bonds.Positions
                .Where(x => x.InstrumentType == "bond")
                .Select(x => new BondMetric() { Instrument = allBonds.Instruments.FirstOrDefault(y => y.Uid == x.InstrumentUid) })
                .ToArray();

            var uids = bondMetrics.Select(x => x.Instrument.Uid);

            //Расчет цены покупки
            var lastPrices = api.GetLastPrices(uids.ToArray());
            foreach (var bond in bondMetrics)
            {
                var instrument = lastPrices.LastPrices.FirstOrDefault(x => x.InstrumentUid == bond.Instrument.Uid);
                bond.LastPrice = instrument.Price;
            }

            //Купоны
            foreach (var uid in uids)
            {
                var coupons = api.GetBondCoupons(uid);
                var instrument = bondMetrics.FirstOrDefault(x => x.Instrument.Uid == uid);
                instrument.Coupons = coupons;
            }
            return bondMetrics;
        }

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
            foreach (var position in positions.Positions)
            {
                if (position.InstrumentType == "bond")
                {
                    var bond = api.GetBondBy(position.InstrumentUid);
                    sumNominal += position.Quantity.Units * bond.Instrument.Nominal.GetInRub();
                    text += $"{position.InstrumentUid}\t{bond.Instrument.Name}\t{position.Quantity.Units * bond.Instrument.Nominal.GetInRub()}\r\n";
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
