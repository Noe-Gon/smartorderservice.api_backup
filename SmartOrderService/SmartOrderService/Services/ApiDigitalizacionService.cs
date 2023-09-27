using Newtonsoft.Json;
using RestSharp;
using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.Message;
using SmartOrderService.Models.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class ApiDigitalizacionService
    {
        public ResponseBase<SaleSyncsApiResponse> SaleSyncApi(SaleSyncsRequest request)
        {
            var URL = GetConfiguracionValue("AWSDigitalizacionURL");
            var token = GetConfiguracionValue("AWSDigitalizacionToken");

            var client = new RestClient();
            client.BaseUrl = new Uri(URL);
            var requestConfig = new RestRequest("sale-syncs?authToken=" + token, Method.POST);
            requestConfig.RequestFormat = DataFormat.Json;
            requestConfig.AddBody(new SaleSyncsApiRequest
            {
                branchId = request.BranchCode,
                routeId = request.RouteCode,
                date = request.Date.ToString("yyyy-MM-dd"),
                SaleState = "CreateSyncSaleState"
            });

            var RestResponse = client.Execute(requestConfig);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var response = JsonConvert.DeserializeObject<SaleSyncsApiResponse>(RestResponse.Content);
                return ResponseBase<SaleSyncsApiResponse>.Create(response);
            }

            if (RestResponse.StatusCode == System.Net.HttpStatusCode.Forbidden)
                throw new UnauthorisedException("Apikey no válido ");

            if (RestResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                throw new InternalServerException("Error interno de Api Digitalización");

            throw new Exception("Error no definido");
        }

        public ResponseBase<GetSaleSyncStatusResponse> GetSaleSyncStatus(GetSaleSyncStatusRequest request)
        {
            var URL = GetConfiguracionValue("AWSDigitalizacionURL");
            var token = GetConfiguracionValue("AWSDigitalizacionToken");

            var client = new RestClient();
            client.BaseUrl = new Uri(URL);
            var requestConfig = new RestRequest("sale-syncs/" + request.executionIdAws + "?authToken=" + token, Method.GET);

            var RestResponse = client.Execute(requestConfig);

            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var response = JsonConvert.DeserializeObject<GetSaleSyncStatusResponse>(RestResponse.Content);
                return ResponseBase<GetSaleSyncStatusResponse>.Create(response);
            }

            if (RestResponse.StatusCode == System.Net.HttpStatusCode.Forbidden)
                throw new UnauthorisedException("Apikey no válido ");

            if (RestResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                throw new InternalServerException("Error interno de Api Digitalización");

            throw new Exception("Error no definido");
        }

        public string GetConfiguracionValue(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            if (value == null)
                throw new ConfigurationValueNotFoundException("No se encontró el valor de: " + key + " dentro de WByC");

            return value;
        }
    }
}