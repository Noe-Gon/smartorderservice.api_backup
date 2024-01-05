using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Newtonsoft.Json;
using RestSharp;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.ServiceModel.Description;

namespace SmartOrderService.Services
{
    public class CRMService
    {
        public string URL;

        public CRMService()
        {
            try
            {
                URL = ConfigurationManager.AppSettings["URL_API_CRM"];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool IsCFEInCRM(string cfe, string id = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(URL);
            var request = new RestRequest("api/crm/consumer/existCFE", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("CFEcode", cfe);
            var RestResponse = client.Execute(request);
            string content = RestResponse.Content;
            var jsonObject = JsonConvert.DeserializeObject<ResponseBase<MVIsInCRM>>(content);
            if (RestResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(jsonObject.Errors[0] != null ? jsonObject.Errors[0] : "Error no identificado en API_CRM");
            }
            return jsonObject.Data.IsInCRM;
        }

        public Guid? GetIdRoute(string routeCode, string brachCode)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(URL);
            var request = new RestRequest("api/crm/route", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("routeCode", routeCode);
            request.AddParameter("brachCode", brachCode);
            var RestResponse = client.Execute(request);
            string content = RestResponse.Content;
            var jsonObject = JsonConvert.DeserializeObject<ResponseBase<MVIdFromCRM>>(content);
            if (RestResponse.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            return jsonObject.Data.CrmId;
        }

        public Guid? ConsumerToCRM(CRMConsumerRequest consumer)
        {
            try
            {
                return ExecuteActionConsumerToCRM(consumer, Method.POST);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Guid? UpdateConsumerToCRM(CRMConsumerRequest consumer)
        {
            try
            {
                return ExecuteActionConsumerToCRM(consumer, Method.PUT);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Guid? DeleteConsumerToCRM(CRMConsumerRequest consumer)
        {
            try
            {
                return ExecuteActionConsumerToCRM(consumer, Method.PUT);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private Guid? ExecuteActionConsumerToCRM(CRMConsumerRequest consumer, Method type)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(URL);
            var request = new RestRequest("api/crm/consumer", type);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(consumer);
            var RestResponse = client.Execute(request);
            string content = RestResponse.Content;
            var jsonObject = JsonConvert.DeserializeObject<ResponseBase<MVIdFromCRM>>(content);
            if (RestResponse.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            return jsonObject.Data.CrmId;
        }
    }
}