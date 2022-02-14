using Newtonsoft.Json;
using RestSharp;
using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models.Enum;
using SmartOrderService.Models.Message;
using SmartOrderService.Models.Responses;
using SmartOrderService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Xml;

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
            var user = UoWConsumer.UserRepository
                .Get(x => x.userId == request.UserId)
                .FirstOrDefault();

            if(user == null)
                throw new EntityNotFoundException("No se encuentró al usuario en WByC");

            var routeBranch = UoWConsumer.RouteRepository
                .Get(x => x.routeId == request.RouteId)
                .Select(x => new { Route = x, Code = x.so_branch.so_company.code, Branch = x.so_branch })
                .FirstOrDefault();

            if (routeBranch == null)
                throw new EntityNotFoundException("No se encuentró el branch o la ruta");

            var employee = SingleEmployee(routeBranch.Code, request.EmployeeCode);

            if (employee == null)
                throw new EntityNotFoundException("No se encontró al usuarió en WsEmpleados");

            var routeTeam = UoWConsumer.RouteTeamRepository
                    .Get(x => x.userId == request.UserId)
                    .FirstOrDefault();

            if (routeTeam == null)
                throw new EntityNotFoundException("El usuario no esta asignado a un equipo");

            //Notificar a la API
            try
            {
                var requestNotify = new NotifyWorkdayRequest();
                //Si es impulsor
                if (routeTeam.roleTeamId == (int)ERolTeam.Impulsor)
                {
                    requestNotify.auxiliarid = null;
                    requestNotify.impulsorId = Convert.ToInt32(request.EmployeeCode);
                    requestNotify.routeId = Convert.ToInt32(routeBranch.Route.code);
                    requestNotify.posId = Convert.ToInt32(routeBranch.Branch.code);
                }
                //Si es ayudante
                else
                {
                    var impulsorId = UoWConsumer.RouteTeamRepository
                    .Get(x => x.routeId == routeBranch.Route.routeId && x.roleTeamId == (int)ERolTeam.Impulsor)
                    .Select(x => x.userId)
                    .FirstOrDefault();

                    var impulsorCode = UoWConsumer.UserRepository.Get(x => x.userId == impulsorId)
                        .Select(x => x.code)
                        .FirstOrDefault();

                    requestNotify.auxiliarid = Convert.ToInt32(request.EmployeeCode);
                    requestNotify.impulsorId = null;
                    requestNotify.routeId = Convert.ToInt32(routeBranch.Route.code);
                    requestNotify.posId = Convert.ToInt32(routeBranch.Branch.code);
                }

                NotifyWorkday(requestNotify);
            }
            catch (Exception) {
                throw new ExternalAPIException("Error al notificar a WSWmpleados");
            }

            var newLogging = new so_authentication_log()
            {
                LeaderAuthenticationCodeId = null,
                WasLeaderCodeAuthorization = false,
                CreatedDate = DateTime.Now,
                LeaderCode = null,
                ModifiedDate = null,
                Status = true,
                RouteId = routeBranch.Route.routeId,
                UserCode = request.EmployeeCode,
                UserId = user.userId
            };

            UoWConsumer.AuthentificationLogRepository.Insert(newLogging);
            UoWConsumer.Save();

            return ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new AuthenticateEmployeeCodeResponse()
            {
                UserId = user.userId,
                UserName = employee.name + " " + employee.lastname,
                BranchId = routeBranch.Route.branchId,
                BranchName = routeBranch.Branch.name,
                Date = DateTime.Now,
                RoleId = routeTeam.roleTeamId,
                RoleName = routeTeam.roleTeamId == (int)ERolTeam.Ayudante ? "Ayudante" : "Impulsor",
                RouteId = routeBranch.Route.routeId,
                RouteName = routeBranch.Route.name
            });
        }

        public ResponseBase<AuthenticateLeaderCodeResponse> AuthenticateLeaderCode(AuthenticateLeaderCodeRequest request)
        {
            var leaderCode = UoWConsumer.LeaderAuthorizationCodeRepository
                .Get(x => x.Code == request.LeaderCode && x.Status)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault();

            if (leaderCode == null)
                throw new LeaderCodeNotFoundException("Código del líder no encontrado");

            if (!leaderCode.Status)
                throw new LeaderCodeExpiredException("El código del lider ha expirado");

            var user = UoWConsumer.UserRepository
            .Get(x => x.userId == request.UserId)
            .FirstOrDefault();

            if (user != null)
            {
                var newAuthenticationLog = new so_authentication_log
                {
                    LeaderAuthenticationCodeId = leaderCode.Id,
                    Status = true,
                    WasLeaderCodeAuthorization = true,
                    CreatedDate = DateTime.Now,
                    UserCode = leaderCode.Code,
                    UserId = user.userId,
                    RouteId = request.RouteId,
                    LeaderCode = request.EmployeeCode
                };

                UoWConsumer.AuthentificationLogRepository.Insert(newAuthenticationLog);
                UoWConsumer.Save();

                return new ResponseBase<AuthenticateLeaderCodeResponse>()
                {
                    Data = null,
                    Errors = null,
                    Status = true
                };
            }
            else
            {
                throw new EntityNotFoundException("No se encuentró al usuario en WByC");
            }
            
        }

        private string GetTokenAWSEmployee()
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["wsempleadosURL"]);
            var requestConfig = new RestRequest("/AuthenticateUser", Method.POST);
            requestConfig.RequestFormat = DataFormat.Json;

            requestConfig.AddParameter("username", "usrbepensa");
            requestConfig.AddParameter("password", "8Aksl8Hh8");
            requestConfig.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            var RestResponse = client.Execute(requestConfig);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var contentString = RestResponse.Content.Replace("</string>", "");
                var aray = contentString.Split('>');
                return aray.Last();
            }

            throw new ExternalAPIException("Falló al intentar obtener el token");
        }

        private void NotifyWorkday(NotifyWorkdayRequest request)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIdeOPECDV1"]);
            var requestConfig = new RestRequest("/api/v1/crews", Method.POST);
            requestConfig.RequestFormat = DataFormat.Json;

            requestConfig.AddBody(request);

            var RestResponse = client.Execute(requestConfig);
        }

        private Employee SingleEmployee(string idcia, string emp)
        {

            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["wsempleadosURL"]);
            var requestConfig = new RestRequest("/SingleEmployee", Method.POST);
            requestConfig.RequestFormat = DataFormat.Json;

            requestConfig.AddParameter("cia", idcia);
            requestConfig.AddParameter("emp", emp);
            var token = GetTokenAWSEmployee();
            requestConfig.AddParameter("token", token);
            requestConfig.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            var RestResponse = client.Execute(requestConfig);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var singleEmployee = JsonConvert.DeserializeObject<SingleEmployee>(RestResponse.Content);
                if (singleEmployee.employee.Count() == 0)
                    return null;

                return singleEmployee.employee.FirstOrDefault();
            }

            throw new ExternalAPIException("Falló al intentar obtener la información del usuario");
        }

        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }
    }
}