﻿using Newtonsoft.Json;
using RestSharp;
using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.Message;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class Ope20Service
    {
        public bool CallCrewOpe20(CrewOpe20Request request)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["OPE20_API"]);
            string SubscriptionKey = ConfigurationManager.AppSettings["OPE20_subscription_key"];
            var requestConfig = new RestRequest("/api/crew", Method.PUT);
            requestConfig.RequestFormat = DataFormat.Json;

            requestConfig.AddHeader("Ocp-Apim-Subscription-Key", SubscriptionKey);
            requestConfig.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            var RestResponse = client.Execute(requestConfig);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            throw new ExternalAPIException("Falló al intentar obtener la información del usuario");
        }

        public bool IsCediInOpe20(string code)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["OPE20_DistributionCenter_URL"]);
            var requestConfig = new RestRequest("/api/DistributionCenter?brnach=" + code, Method.PUT);
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
    }
}