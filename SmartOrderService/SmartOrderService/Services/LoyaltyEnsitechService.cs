using Newtonsoft.Json;
using RestSharp;
using SmartOrderService.DB;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class LoyaltyEnsitechService
    {
        private SmartOrderModel db = new SmartOrderModel();
        private string url = "https://is68s2j0b1.execute-api.us-east-1.amazonaws.com/dev/";
        private string autorizacion = "a9c332d2-ba38-405f-9cf7-57bcd787eba1";
        private string json = "";

        public List<LoyaltyGetCustomerList> GetCustomerUuid(string branchCode, string routeCode)
        {
            string endpoint = url + "beneficiary/branch/" + branchCode + "/route/" + routeCode;
            try
            {
                var client = new RestClient(endpoint);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);

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
            var autorizacion = "a9c332d2-ba38-405f-9cf7-57bcd787eba1";
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
            json = JsonConvert.SerializeObject(request);
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
            json = JsonConvert.SerializeObject(request);
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


    }
}