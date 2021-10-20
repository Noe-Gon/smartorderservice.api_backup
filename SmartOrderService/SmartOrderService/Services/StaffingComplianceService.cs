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
            var userList = UoWConsumer.UserRepository
                .Get(x => x.code == request.EmployeeCode && x.branchId == request.BranchId && x.type == 6);
            var user = userList.FirstOrDefault();

            if (user == null)
                throw new EntityNotFoundException("No se encuentró al usuario");

            string idcia = userList.Select(x => x.so_branch.so_company.code).FirstOrDefault();

            var routeTeam = UoWConsumer.RouteTeamRepository
                .Get(x => x.userId == user.userId)
                .FirstOrDefault();

            if (routeTeam == null)
                throw new EntityNotFoundException("No se encuentró al usuario en un equipo");


            var route = UoWConsumer.RouteRepository
                .Get(x => x.routeId == routeTeam.routeId)
                .FirstOrDefault();

            var employee = SingleEmployee(idcia, user.code);

            if (employee == null)
                throw new EntityNotFoundException("No se encontró al usuarió en WsEmpleados");

            return ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new AuthenticateEmployeeCodeResponse()
            {
                UserId = user.userId,
                UserName = employee.name + " " + employee.lastname,
                BranchId = user.branchId,
                BranchName = user.so_branch.name,
                Date = DateTime.Now,
                RoleId = routeTeam.roleTeamId,
                RoleName = routeTeam.roleTeamId == (int)ERolTeam.Ayudante ? "Ayudante" : "Impulsor",
                RouteId = routeTeam.routeId,
                RouteName = route.name
            });
        }

        public ResponseBase<AuthenticateLeaderCodeResponse> AuthenticateLeaderCode(AuthenticateLeaderCodeRequest request)
        {
            try
            {
                var response = AuthenticateEmployeeCode(new AuthenticateEmployeeCodeRequest 
                { 
                    BranchId = request.BranchId, 
                    EmployeeCode = request.EmployeeCode 
                }).Data;

                return ResponseBase<AuthenticateLeaderCodeResponse>.Create(new AuthenticateLeaderCodeResponse
                {
                    BranchId = response.BranchId,
                    BranchName = response.BranchName,
                    Date = response.Date,
                    RoleId = response.RoleId,
                    RoleName = response.RoleName,
                    RouteId = response.RouteId,
                    RouteName = response.RouteName,
                    UserId = response.UserId,
                    UserName = response.UserName
                });
            }
            catch (Exception)
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
                .Get(x => x.code == request.EmployeeCode && x.branchId == request.BranchId && x.type == 6)
                .FirstOrDefault();

                if (user != null)
                {
                    var routeId = UoWConsumer.RouteTeamRepository
                        .Get(x => x.userId == user.userId)
                        .Select(x => x.routeId)
                        .FirstOrDefault();

                    if (routeId != 0) 
                    { 
                        var newAuthenticationLog = new so_authentication_log
                        {
                            LeaderAuthenticationCodeId = leaderCode.Id,
                            Status = true,
                            WasLeaderCodeAuthorization = true,
                            CreatedDate = DateTime.Now,
                            UserCode = leaderCode.Code,
                            UserId = user.userId,
                            RouteId = routeId
                        };

                        UoWConsumer.AuthentificationLogRepository.Insert(newAuthenticationLog);
                        UoWConsumer.Save();
                    }
                }

                return new ResponseBase<AuthenticateLeaderCodeResponse>()
                {
                    Data = null,
                    Errors = null,
                    Status = true
                };
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