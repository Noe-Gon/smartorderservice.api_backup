using Newtonsoft.Json;
using RestSharp;
using SmartOrderService.Interfaces;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Responses;
using SmartOrderService.Utils;
using System;
using System.Configuration;

namespace SmartOrderService.Services
{
    public sealed class APIPreventaService : IAPIPreventaService
    {
        private string _baseUrl;
        private string _apiKey { get; set; }

        private static APIPreventaService _instance;
        private APIPreventaService()
        {
            string baseUrl = ConfigurationManager.AppSettings["API_Preventa_URL"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new Exception("No se encuentra configurado el valor de API_Preventa_URL en el archivo de configuración");
            }
            _baseUrl = baseUrl;
            string apiKey = ConfigurationManager.AppSettings["API_Preventa_key"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("No se encuentra configurado el valor de API_Preventa_key en el archivo de configuración");
            }
            _apiKey = apiKey;
        }
        public static APIPreventaService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new APIPreventaService();
            }
            return _instance;
        }

        public ClosingPreclosingResponse SendPreSales(ClosingPreclosingDTO request)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(_baseUrl);
            var requestConfig = new RestRequest($"api/v{request.version}/closing/preclosing?branchCode={request.branchCode}&routeCode={request.routeCode}", Method.POST);
            requestConfig.AddHeader("x-api-key", _apiKey);
            requestConfig.RequestFormat = DataFormat.Json;

            var response = client.Execute(requestConfig);
            StatusCodeValidatorPreSales.ValidateClosingPreclosing(response);
            return JsonConvert.DeserializeObject<ClosingPreclosingResponse>(response.Content);
        }
    }
}