using Newtonsoft.Json;
using RestSharp;
using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.Message;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace SmartOrderService.Services
{
    public class Ope20Service
    {
        public bool CallCrewOpe20(CrewOpe20Request request)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["API_OPE20_URL"]);
            string SubscriptionKey = ConfigurationManager.AppSettings["EXTERNAL_ROUTE_KEY_Subscription_key"];
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var requestConfig = new RestRequest("api/external-route/api/crew", Method.PUT);
            requestConfig.RequestFormat = DataFormat.Json;
            requestConfig.AddHeader("Ocp-Apim-Subscription-Key", SubscriptionKey);
            requestConfig.AddJsonBody(request);

            var RestResponse = client.Execute(requestConfig);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            throw new ExternalAPIException(RestResponse.StatusCode.ToString() + ": " + RestResponse.Content);
        }

        public bool IsCediInOpe20(string code)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["OPE20_DistributionCenter_URL"]);
            var requestConfig = new RestRequest("api/DistributionCenter?branch=" + code, Method.GET);
            requestConfig.RequestFormat = DataFormat.Json;

            var RestResponse = client.Execute(requestConfig);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var response = JsonConvert.DeserializeObject<List<DistributionCenterResponse>>(RestResponse.Content);

                if (response.Count() == 0)
                    return false;

                return response.FirstOrDefault().Status;
            }
            return false;
        }

        public CloseRouteNotificationResponse CloseRouteNotification(CloseRouteNotificationRequest request)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["API_OPE20_URL"]);
            string SubscriptionKey = ConfigurationManager.AppSettings["EXTERNAL_SALE_KEY_Subscription_key"];
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var requestConfig = new RestRequest("api/external-sale/api/sale/closeroutenotification", Method.PUT);
            requestConfig.RequestFormat = DataFormat.Json;
            requestConfig.AddHeader("Ocp-Apim-Subscription-Key", SubscriptionKey);
            requestConfig.AddJsonBody(request);

            var RestResponse = client.Execute(requestConfig);
            if (RestResponse.StatusCode != HttpStatusCode.OK)
                throw new ExternalAPIException("Error en Ope20.");

            if (RestResponse.StatusCode != HttpStatusCode.OK)
                throw new ExternalAPIException(RestResponse.Content);

            var response = JsonConvert.DeserializeObject<CloseRouteNotificationResponse>(RestResponse.Content);

            if (response.ErrorCode != 0)
                throw new ExternalAPIException(response.Message);

            return response;
        }
    }
}