using Newtonsoft.Json;
using RestSharp;
using SmartOrderService.DB;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class LoyaltyEnsitechService
    {
        private SmartOrderModel db = new SmartOrderModel();
        private string url = ConfigurationManager.AppSettings["Loyalty_URL"];
        private string autorizacion = ConfigurationManager.AppSettings["Loyalty_AUTH"];

        public List<LoyaltyGetCustomerList> GetCustomerUuid(string branchCode, string routeCode)
        {
            string endpoint = url + "beneficiary/branch/" + branchCode + "/route/" + routeCode;
            try
            {
                var client = new RestClient(endpoint);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");

                if (autorizacion != null)
                {
                    request.AddHeader("x-api-key", autorizacion);
                }

                IRestResponse response = client.Execute(request);
                var responseUuid = JsonConvert.DeserializeObject<List<LoyaltyGetCustomerList>>(response.Content);
                return responseUuid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase<LoyaltyUuidResponse> GetConsumerUuidByCustomerCode(string request)
        {
            var endpoint = url + "beneficiary/customer?customerCode=" + request;
            try
            {
                var client = new RestClient(endpoint);
                var restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddHeader("x-api-key", autorizacion);
                IRestResponse response = client.Execute(restRequest);
                return ResponseBase<LoyaltyUuidResponse>.Create(JsonConvert.DeserializeObject<LoyaltyUuidResponse>(response.Content));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase<LoyaltyGetPointsResponse> GetConsumerPoints(string uuid)
        {
            var endpoint = url + "beneficiary/" + uuid + "/points";
            try
            {
                var client = new RestClient(endpoint);
                var restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddHeader("x-api-key", autorizacion);
                IRestResponse response = client.Execute(restRequest);
                return ResponseBase<LoyaltyGetPointsResponse>.Create(JsonConvert.DeserializeObject<LoyaltyGetPointsResponse>(response.Content));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LoyaltyRedemptionRefundResponse RedemptionPoints(LoyaltyPointsRequest request)
        {
            var endpoint = url + "beneficiary/points/redemption";
            var json = JsonConvert.SerializeObject(request);
            try
            {
                var client = new RestClient(endpoint);
                var restRequest = new RestRequest(Method.POST);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddHeader("x-api-key", autorizacion);
                restRequest.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(restRequest);
                var responseObject = JsonConvert.DeserializeObject<LoyaltyRedemptionRefundResponse>(response.Content);
                return responseObject;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LoyaltyRedemptionRefundResponse RefundPoints(LoyaltyPointsRequest request)
        {
            var endpoint = url + "beneficiary/points/refund";
            var json = JsonConvert.SerializeObject(request);
            try
            {
                var client = new RestClient(endpoint);
                var restRequest = new RestRequest(Method.POST);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddHeader("x-api-key", autorizacion);
                restRequest.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(restRequest);
                var responseObject = JsonConvert.DeserializeObject<LoyaltyRedemptionRefundResponse>(response.Content);
                return responseObject;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LoyaltyGetProductsResponse GetConsumerProducts(string uuid)
        {
            var endpoint = url + "beneficiary/" + uuid + "/products";
            try
            {
                var client = new RestClient(endpoint);
                var restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddHeader("x-api-key", autorizacion);
                IRestResponse response = client.Execute(restRequest);
                var jsonFormat = "{\"ProductConfig\": " + response.Content + "}";
                var responseRefined = JsonConvert.DeserializeObject<LoyaltyGetProductsResponse>(jsonFormat);
                return responseRefined;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LoyaltyGetRulesResponse GetRules(int routeId)
        {
            var data = db.so_route.Where(x => x.routeId == routeId)
                .Select(x => new { RouteCode = x.code, BranchCode = x.so_branch.code})
                .FirstOrDefault();

            var endpoint = url + "beneficiary/branch/" + data.BranchCode + "/route/" + data.RouteCode + "/rules";
            try
            {
                var client = new RestClient(endpoint);
                var restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddHeader("x-api-key", autorizacion);
                IRestResponse response = client.Execute(restRequest);
                var jsonFormat = "{\"RulesConfig\": " + response.Content + "}";
                var responseRefined = JsonConvert.DeserializeObject<LoyaltyGetRulesResponse>(jsonFormat);
                return responseRefined;
            }
            catch (Exception ex)
            {
                throw new Exception("El beneficiario no cuenta con reglas de canje configurados o no existe");
            }
        }

        public LoyaltyPostBeneficiaryResponse PostBeneficiary(LoyaltyPostBenficiaryRequest request)
        {
            var endpoint = url + "beneficiary";
            try
            {
                var client = new RestClient(endpoint);
                var restRequest = new RestRequest(Method.POST);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddHeader("x-api-key", autorizacion);
                restRequest.RequestFormat = DataFormat.Json;
                restRequest.AddJsonBody(request);
                IRestResponse response = client.Execute(restRequest);
                var responseObject = JsonConvert.DeserializeObject<LoyaltyPostBeneficiaryResponse>(response.Content);
                return responseObject;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}