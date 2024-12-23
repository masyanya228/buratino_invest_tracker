﻿using Buratino.API.Dto;

using Newtonsoft.Json;

using RestSharp;

namespace Buratino.API
{
    public class TInvestAPI
    {
        public AccountPositions GetPositions(long accaountId = 2099881492)
        {
            var url = "https://invest-public-api.tinkoff.ru/rest/tinkoff.public.invest.api.contract.v1.OperationsService/GetPositions";
            RestClient restClient = new RestClient(url);
            RestRequest restRequest = new RestRequest(url, Method.Post);
            restRequest.AddHeader("Authorization", "Bearer t.tMh7y3HUV6e_Fa6XIQtT2EGRoBOgd1YdMpzVULODY_1K9XG7Br6GUDdphYIeOUdigJyUHHBkwuLyhRSMGPLnBA");
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
            restRequest.AddHeader("Authorization", "Bearer t.tMh7y3HUV6e_Fa6XIQtT2EGRoBOgd1YdMpzVULODY_1K9XG7Br6GUDdphYIeOUdigJyUHHBkwuLyhRSMGPLnBA");
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
            restRequest.AddHeader("Authorization", "Bearer t.tMh7y3HUV6e_Fa6XIQtT2EGRoBOgd1YdMpzVULODY_1K9XG7Br6GUDdphYIeOUdigJyUHHBkwuLyhRSMGPLnBA");
            restRequest.AddBody(new
            {
                id_type = "INSTRUMENT_ID_TYPE_UID",
                id = instrumentUid
            });
            var resp = restClient.Execute(restRequest);
            var obj = JsonConvert.DeserializeObject<Bond>(resp.Content);
            return obj;
        }

        public Etf GetEtfBy(string instrumentUid)
        {
            var url = "https://invest-public-api.tinkoff.ru/rest/tinkoff.public.invest.api.contract.v1.InstrumentsService/EtfBy";
            RestClient restClient = new RestClient(url);
            RestRequest restRequest = new RestRequest(url, Method.Post);
            restRequest.AddHeader("Authorization", "Bearer t.tMh7y3HUV6e_Fa6XIQtT2EGRoBOgd1YdMpzVULODY_1K9XG7Br6GUDdphYIeOUdigJyUHHBkwuLyhRSMGPLnBA");
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
            restRequest.AddHeader("Authorization", "Bearer t.tMh7y3HUV6e_Fa6XIQtT2EGRoBOgd1YdMpzVULODY_1K9XG7Br6GUDdphYIeOUdigJyUHHBkwuLyhRSMGPLnBA");
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
