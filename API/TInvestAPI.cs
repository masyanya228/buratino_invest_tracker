using Buratino.API.Dto;
using Buratino.Xtensions;
using Newtonsoft.Json;

using RestSharp;

namespace Buratino.API
{
    public class TInvestAPI
    {
        const string tokenAPI = "t.9BmkVm3uGX6xtaQkLQL9IpKfmBFwmmSDPGWYjdLIZJQFHIH5FrXuRFGCYT2OvuybsdFqmv9WmYjQs1m6kjQb0Q";

        /// <summary>
        /// Возвращает все облигации
        /// </summary>
        /// <returns></returns>
        public Bonds GetAllBonds()
        {
            var url = "https://invest-public-api.tinkoff.ru/rest/tinkoff.public.invest.api.contract.v1.InstrumentsService/Bonds";
            RestClient restClient = new RestClient(url);
            RestRequest restRequest = new RestRequest(url, Method.Post);
            restRequest.AddHeader("Authorization", "Bearer " + tokenAPI);
            restRequest.AddBody(new
            {
                instrumentStatus = "INSTRUMENT_STATUS_UNSPECIFIED"
            });
            if (AllBondCache(out Bonds bonds, out string cacheName))
            {
                return bonds;
            }
            else
            {
                Thread.Sleep(550);
                var resp = restClient.Execute(restRequest);
                File.WriteAllText(cacheName, resp.Content);
                return JsonConvert.DeserializeObject<Bonds>(resp.Content);
            }
        }

        public bool AllBondCache(out Bonds bonds, out string cacheName)
        {
            string path = @"Cache/tinvest/";
            cacheName = Path.Combine(path, $"ab.txt");
            if (!Directory.Exists(path))
            {
                var res = Directory.CreateDirectory(path);
            }
            if (File.Exists(cacheName))
            {
                bonds = JsonConvert.DeserializeObject<Bonds>(File.ReadAllText(cacheName));
                return true;
            }
            else
            {
                bonds = null;
                return false;
            }
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

        public GetAccounts GetAccounts()
        {
            var url = "https://invest-public-api.tinkoff.ru/rest/tinkoff.public.invest.api.contract.v1.MarketDataService/GetLastPrices";
            RestClient restClient = new RestClient(url);
            RestRequest restRequest = new RestRequest(url, Method.Post);
            restRequest.AddHeader("Authorization", "Bearer " + tokenAPI);
            restRequest.AddBody(new
            {
                status = "ACCOUNT_STATUS_OPEN"
            });
            var resp = restClient.Execute(restRequest);
            var obj = JsonConvert.DeserializeObject<GetAccounts>(resp.Content);
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

        public OperationHistory GetOperations(long accountId, string instrumentUid = null)
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
                //operationTypes = new string[]
                //{
                //    "OPERATION_TYPE_INPUT",
                //    "OPERATION_TYPE_OUTPUT",
                //    "OPERATION_TYPE_DIV_EXT",
                //    "OPERATION_TYPE_OUTPUT_ACQUIRING",
                //    "OPERATION_TYPE_INPUT_ACQUIRING",
                //    "OPERATION_TYPE_OUT_MULTI",
                //    "OPERATION_TYPE_INP_MULTI",
                //}
            });
            var resp = restClient.Execute(restRequest);
            var obj = JsonConvert.DeserializeObject<OperationHistory>(resp.Content);
            //if(obj.Items.Where(x => x.Type.NotIn(
            //    Enums.OperationType.OPERATION_TYPE_BROKER_FEE,
            //    Enums.OperationType.OPERATION_TYPE_BUY,
            //    Enums.OperationType.OPERATION_TYPE_COUPON,
            //    Enums.OperationType.OPERATION_TYPE_INPUT))
            //    .Any())
            //{
            //    throw new Exception("Новый тип операции");
            //}
            return obj;
        }
    }
}
