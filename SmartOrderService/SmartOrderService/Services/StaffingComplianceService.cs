using RestSharp;
using SmartOrderService.Models.Message;
using SmartOrderService.Models.Responses;
using SmartOrderService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class StaffingComplianceService : IDisposable
    {
        public static StaffingComplianceService Create() => new StaffingComplianceService();

        private UoWConsumer UoWConsumer { get; set; }

        public StaffingComplianceService()
        {
            UoWConsumer = new UoWConsumer();
        }

        public ResponseBase<AuthenticateEmployeeCodeResponse> AuthenticateEmployeeCode(AuthenticateEmployeeCodeRequest request)
        {
            try
            {


                SingleEmployee("1", request.EmployeeCode);

                return ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new AuthenticateEmployeeCodeResponse()
                {
                });
            }
            catch (Exception e)
            {
                return ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                });
            }
        }

        private string SingleEmployee(string idcia, string emp)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["wsempleadosURL"]);
            var requestConfig = new RestRequest("/SingleEmployee", Method.POST);
            requestConfig.RequestFormat = DataFormat.Json;
            
            requestConfig.AddQueryParameter("cia", idcia);
            requestConfig.AddQueryParameter("emp", emp);
            requestConfig.AddQueryParameter("token", "3157ee7d-2295-4b1d-8764-eb682af471fc");

            var RestResponse = client.Execute(requestConfig);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return "OPCD finalizadó con exito";
            }
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return "No insertado. Ya existe un registro con los mismos datos";
            }
            return "OPCD Falló en notificación";
        }

        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }
    }
}