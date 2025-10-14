using Buratino.Analitics.Dto;
using Buratino.API.Dto;
using Buratino.Xtensions;
using Newtonsoft.Json;

using RestSharp;

namespace Buratino.API
{
    public class TInvestAPI
    {
        const string tokenAPI = "t.9BmkVm3uGX6xtaQkLQL9IpKfmBFwmmSDPGWYjdLIZJQFHIH5FrXuRFGCYT2OvuybsdFqmv9WmYjQs1m6kjQb0Q";

        public Bonds Bonds()
        {
            var url = "https://invest-public-api.tinkoff.ru/rest/tinkoff.public.invest.api.contract.v1.InstrumentsService/Bonds";
            RestClient restClient = new RestClient(url);
            RestRequest restRequest = new RestRequest(url, Method.Post);
            restRequest.AddHeader("Authorization", "Bearer " + tokenAPI);
            restRequest.AddBody(new
            {
                instrumentStatus = "INSTRUMENT_STATUS_UNSPECIFIED"
            });
            var resp = new { Content = File.ReadAllText("C:\\Users\\User\\Desktop\\Bonds.txt") };//restClient.Execute(restRequest);
            var obj = JsonConvert.DeserializeObject<Bonds>(resp.Content);

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
            var lastPrices = GetLastPrices(uids.ToArray());
            foreach (var item in filtered)
            {
                var instrument = lastPrices.LastPrices.FirstOrDefault(x => x.InstrumentUid == item.Instrument.Uid);
                item.LastPrice = instrument.Price;
            }

            //Купоны
            foreach (var uid in uids)
            {
                var coupons = GetBondCoupons(uid);
                var instrument = filtered.FirstOrDefault(x => x.Instrument.Uid == uid);
                instrument.Coupons = coupons;
            }

            var resTable = filtered
                .OrderBy(x => x.Instrument.Name)
                .Select(x => $"{x.Instrument.Name}\t{x.Instrument.Brand.LogoName}\t{x.Instrument.Ticker}\t{x.Instrument.Uid}\t{x.Instrument.Figi}\t{x.Instrument.RiskLevel}\t{x.GetYearlyIncome()}\t{x.Instrument.CouponQuantityPerYear}\t{x.EndDate.Subtract(DateTime.Now).TotalDays / 30}\t{x.Instrument.Nominal.Units}\t{x.TotalPrice}\t{x.Instrument.Sector}")
                .Join("\r\n");
            return obj;
        }

        public GetLastPrices GetLastPrices(string[] uids)
        {
            var url = "https://invest-public-api.tinkoff.ru/rest/tinkoff.public.invest.api.contract.v1.MarketDataService/GetLastPrices";
            RestClient restClient = new RestClient(url);
            RestRequest restRequest = new RestRequest(url, Method.Post);
            restRequest.AddHeader("Authorization", "Bearer " + tokenAPI);
            restRequest.AddBody(new
            {
                instrumentId = uids
            });
            var resp = restClient.Execute(restRequest);
            var obj = JsonConvert.DeserializeObject<GetLastPrices>(resp.Content);
            return obj;
        }

        public AccountPositions GetPositions(long accaountId = 2099881492)
        {
            var url = "https://invest-public-api.tinkoff.ru/rest/tinkoff.public.invest.api.contract.v1.OperationsService/GetPositions";
            RestClient restClient = new RestClient(url);
            RestRequest restRequest = new RestRequest(url, Method.Post);
            restRequest.AddHeader("Authorization", "Bearer " + tokenAPI);
            restRequest.AddBody(new
            {
                accountId = accaountId
            });
            var resp = restClient.Execute(restRequest);
            var obj = JsonConvert.DeserializeObject<AccountPositions>(resp.Content);
            return obj;
        }

        public Portfolio GetPortfolio(long accaountId = 2099881492)
        {
            var url = "https://invest-public-api.tinkoff.ru/rest/tinkoff.public.invest.api.contract.v1.OperationsService/GetPortfolio";
            RestClient restClient = new RestClient(url);
            RestRequest restRequest = new RestRequest(url, Method.Post);
            restRequest.AddHeader("Authorization", "Bearer " + tokenAPI);
            restRequest.AddBody(new
            {
                accountId = accaountId
            });
            var resp = restClient.Execute(restRequest);
            var obj = JsonConvert.DeserializeObject<Portfolio>(resp.Content);
            return obj;
        }

        public Bond GetBondBy(string instrumentUid)
        {
            var url = "https://invest-public-api.tinkoff.ru/rest/tinkoff.public.invest.api.contract.v1.InstrumentsService/BondBy";
            RestClient restClient = new RestClient(url);
            RestRequest restRequest = new RestRequest(url, Method.Post);
            restRequest.AddHeader("Authorization", "Bearer " + tokenAPI);
            restRequest.AddBody(new
            {
                id_type = "INSTRUMENT_ID_TYPE_UID",
                id = instrumentUid
            });
            var resp = restClient.Execute(restRequest);
            var obj = JsonConvert.DeserializeObject<Bond>(resp.Content);
            return obj;
        }

        public BondCoupons GetBondCoupons(string figi)
        {
            var url = "https://invest-public-api.tinkoff.ru/rest/tinkoff.public.invest.api.contract.v1.InstrumentsService/GetBondCoupons";
            RestClient restClient = new RestClient(url);
            RestRequest restRequest = new RestRequest(url, Method.Post);
            restRequest.AddHeader("Authorization", "Bearer " + tokenAPI);
            restRequest.AddBody(new
            {
                figi,
                from = DateTime.Now.AddMonths(-1),
                to = DateTime.Now.AddYears(10)
            });
            if (BondCouponsCache(figi, out BondCoupons coupons, out string cacheName))
            {
                return coupons;
            }
            else
            {
                Thread.Sleep(550);
                var resp = restClient.Execute(restRequest);
                File.WriteAllText(cacheName, resp.Content);
                return JsonConvert.DeserializeObject<BondCoupons>(resp.Content);
            }
        }

        public bool BondCouponsCache(string figi, out BondCoupons coupons, out string cacheName)
        {
            string path = @"Cache/tinvest/";
            cacheName = Path.Combine(path, $"bc_{figi}.txt");
            if (!Directory.Exists(path))
            {
                var res = Directory.CreateDirectory(path);
            }
            if (File.Exists(cacheName))
            {
                coupons = JsonConvert.DeserializeObject<BondCoupons>(File.ReadAllText(cacheName));
                return true;
            }
            else
            {
                coupons = null;
                return false;
            }
        }

        public Etf GetEtfBy(string instrumentUid)
        {
            var url = "https://invest-public-api.tinkoff.ru/rest/tinkoff.public.invest.api.contract.v1.InstrumentsService/EtfBy";
            RestClient restClient = new RestClient(url);
            RestRequest restRequest = new RestRequest(url, Method.Post);
            restRequest.AddHeader("Authorization", "Bearer " + tokenAPI);
            restRequest.AddBody(new
            {
                id_type = "INSTRUMENT_ID_TYPE_UID",
                id = instrumentUid
            });
            var resp = restClient.Execute(restRequest);
            var obj = JsonConvert.DeserializeObject<Etf>(resp.Content);
            return obj;
        }

        public OperationHistory GetOperations(string accountId, string instrumentUid = null)
        {
            var url = "https://invest-public-api.tinkoff.ru/rest/tinkoff.public.invest.api.contract.v1.OperationsService/GetOperationsByCursor";
            RestClient restClient = new RestClient(url);
            RestRequest restRequest = new RestRequest(url, Method.Post);
            restRequest.AddHeader("Authorization", "Bearer " + tokenAPI);
            restRequest.AddBody(new
            {
                accountId,
                instrumentId = instrumentUid,
                limit = 1000,
                operationTypes = new string[]
                {
                    "OPERATION_TYPE_INPUT",
                    "OPERATION_TYPE_OUTPUT",
                    "OPERATION_TYPE_DIV_EXT",
                    "OPERATION_TYPE_OUTPUT_ACQUIRING",
                    "OPERATION_TYPE_INPUT_ACQUIRING",
                    "OPERATION_TYPE_OUT_MULTI",
                    "OPERATION_TYPE_INP_MULTI",
                }
            });
            var resp = restClient.Execute(restRequest);
            var obj = JsonConvert.DeserializeObject<OperationHistory>(resp.Content);
            return obj;
        }
    }
}
