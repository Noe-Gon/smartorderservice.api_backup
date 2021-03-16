using RestSharp;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class CountConfigurationService
    {

        public CountConfigurationDto CountRoute(int RouteId) {

            CountConfigurationDto result =null;
            string Url = ConfigurationManager.AppSettings["CellarUrl"];

            try
            {
                var client = new RestClient(Url);

                var request = new RestRequest("count-configurations", Method.GET);
                request.AddParameter("RouteId", RouteId.ToString());
                
                // easily add HTTP Headers
                request.AddHeader("ContentType", "application/json");

                var response = client.Execute<CountConfigurationResponse>(request);

                var configurationResponse = response.Data;


                if (configurationResponse.CountConfigurations.Any())
                {

                    result = configurationResponse.CountConfigurations.FirstOrDefault();

                }

            }
            catch (Exception e)
            {
                result = null;
            }

            return result;
        }

    }
}