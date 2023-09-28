using RestSharp;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Message;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace SmartOrderService.Services
{
    public class BConnectService
    {
        private string BConnect_URL = ConfigurationManager.AppSettings["BConnect_URL"];
        private string BConnect_XApiKey = ConfigurationManager.AppSettings["BConnect_x-api-key"];

        public async Task POST_Customer_TeamVisit(CustomerVisitDto requestBody)
        {
            var cliente = new RestClient();
            cliente.BaseUrl = new Uri(BConnect_URL);
            
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var request = new RestRequest("api/Customer/TeamVisit", Method.POST);
            request.AddHeader("x-api-key", BConnect_XApiKey);
            request.AddJsonBody(requestBody);

            var response = cliente.Execute(request);
        }
    }
}