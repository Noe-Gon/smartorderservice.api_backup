﻿using Newtonsoft.Json;
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
using System.Data.Entity;
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
            bool inTripulacs = false;
            List<string> exceptionMessages = new List<string>();
            //Notificar a la API
            try
            {
                var requestNotify = new NotifyWorkdayRequest();
                //Si es impulsor
                if (routeTeam.roleTeamId == (int)ERolTeam.Impulsor)
                {
                    //Buscar si el ayudante se autenticó antes que el impulsor para registrarlo. 
                    int ayudanteId = UoWConsumer.RouteTeamRepository
                        .Get(x => x.roleTeamId == (int)ERolTeam.Ayudante && x.routeId == request.RouteId)
                        .Select(x => x.userId)
                        .FirstOrDefault();

                    var ayudante = UoWConsumer.AuthentificationLogRepository
                        .Get(x => !x.WasLeaderCodeAuthorization && DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now)
                                    && x.UserId == ayudanteId && x.RouteId == request.RouteId)
                        .FirstOrDefault();

                    string ayudanteCode = ayudante != null ? ayudante.UserCode : null;

                    requestNotify.auxiliarid = Convert.ToInt32(ayudanteCode);
                    requestNotify.impulsorId = Convert.ToInt32(request.EmployeeCode);
                    requestNotify.routeId = Convert.ToInt32(routeBranch.Route.code);
                    requestNotify.posId = Convert.ToInt32(routeBranch.Branch.code);

                    var response = NotifyWorkday(requestNotify);
                    if (response == "\"{\\\"errors\\\":[]}\"")
                    {
                        //Lógica del fallo con el registro en tripulacs
                        inTripulacs = true;
                        if (ayudanteCode != null)
                        {
                            ayudante.Status = true;
                            UoWConsumer.AuthentificationLogRepository.Update(ayudante);
                        }
                    }
                }
                //Si es ayudante
                else
                {
                    //Buscar si ya inicio un Impulsor
                    int? impulsorId = UoWConsumer.RouteTeamRepository
                        .Get(x => x.roleTeamId == (int)ERolTeam.Impulsor && x.routeId == request.RouteId)
                        .Select(x => x.userId)
                        .FirstOrDefault();

                    if (impulsorId == null)
                    {
                        return ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new AuthenticateEmployeeCodeResponse()
                        {
                        });
                        throw new NoUserFoundException("No se encontró al impulsor en WBC. Revisar que exista el impulsor en la ruta por medio de la tabla so_route_team");
                    }

                    var isWorkDayActive = UoWConsumer.WorkDayRepository
                        .Get(x => x.userId == impulsorId);
                    if (isWorkDayActive == null)
                    {
                        return ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new AuthenticateEmployeeCodeResponse()
                        {
                        });
                        throw new WorkdayNotFoundException("No se encontró el Work Day del impulsor.");
                    }


                    var impulsor = UoWConsumer.AuthentificationLogRepository
                        .Get(x => !x.WasLeaderCodeAuthorization && DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now) 
                                    && x.UserId == impulsorId && x.RouteId == request.RouteId)
                        .FirstOrDefault();
                    if (impulsor != null)
                    {
                        //Lógica del fallo con el registro en tripulacs
                        inTripulacs = true;
                        var impulsorCode = impulsor.UserCode;
                        requestNotify.auxiliarid = Convert.ToInt32(request.EmployeeCode);
                        requestNotify.impulsorId = Convert.ToInt32(impulsorCode);
                        requestNotify.routeId = Convert.ToInt32(routeBranch.Route.code);
                        requestNotify.posId = Convert.ToInt32(routeBranch.Branch.code);
                        
                        var response = NotifyWorkday(requestNotify);
                        if (response == "\"{\\\"errors\\\":[]}\"")
                        {
                            //Lógica del fallo con el registro en tripulacs
                            inTripulacs = true;
                        }
                        if (response == "\"{\\\"errors\\\":[{\\\"error\\\":9001,\\\"message\\\":\\\"No existe la tripulación configurada para la ruta " + routeBranch.Route.code + ".\\\"}]}\"")
                        {
                            exceptionMessages.Add("No se encuentra configurada la ruta en tripulacs");
                            //throw new NoRouteConfigTripulacsException("No se encuentra configurada la ruta en tripulacs");
                        }
                        if (response == "\"{\\\"errors\\\":[{\\\"error\\\":404,\\\"message\\\":\\\"El id del impulsor no es válido\\\"}]}\"")
                        {
                            exceptionMessages.Add("El id del impulsor no es válido");
                        }
                    }
                    else
                    {
                        exceptionMessages.Add("El impulsor no se ha autenticado");
                    }
                }
            }
            catch (UserInUseException e)
            {
                throw e;
            }
            catch (Exception e) {
                exceptionMessages.Add("Error al notificar a WSempleados: " + e.Message);
            }

            //Buscar si ya existe un registro para este usuario
            var existLog = UoWConsumer.AuthentificationLogRepository
                .Get(x => x.RouteId == request.RouteId && x.UserId == request.UserId && !x.WasLeaderCodeAuthorization
                            && DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now))
                .FirstOrDefault();

            if(existLog == null)
            {
                var newLogging = new so_authentication_log()
                {
                    LeaderAuthenticationCodeId = null,
                    WasLeaderCodeAuthorization = false,
                    CreatedDate = DateTime.Now,
                    LeaderCode = null,
                    ModifiedDate = null,
                    Status = inTripulacs,
                    RouteId = routeBranch.Route.routeId,
                    UserCode = request.EmployeeCode,
                    UserId = user.userId,
                    UserName = employee.name + " " + employee.lastname,
                    IsSynchronized = true
                };

                UoWConsumer.AuthentificationLogRepository.Insert(newLogging);
                UoWConsumer.Save();
            }
            else
            {
                if (existLog.UserCode != request.EmployeeCode)
                    throw new UnauthorizedAccessException("Otro Empleado ya inicio sesión con este usuario");
            }

            var responseAuthentication = ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new AuthenticateEmployeeCodeResponse()
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
            responseAuthentication.Errors = exceptionMessages;
            return responseAuthentication;
        }

        public ResponseBase<AuthenticateLeaderCodeResponse> AuthenticateEmployeeCodeGet(AuthenticateEmployeeCodeRequest request)
        {
            var user = UoWConsumer.UserRepository
                .Get(x => x.userId == request.UserId)
                .FirstOrDefault();

            if (user == null)
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

            var log = Getlog(request.EmployeeCode, DateTime.Now);
            if (log != null)
                if (log.UserId != request.UserId)
                    throw new UserInUseException("Este codigó ya ha sido usado");

            var routeTeam = UoWConsumer.RouteTeamRepository
                    .Get(x => x.userId == request.UserId)
                    .FirstOrDefault();

            if (routeTeam == null)
                throw new EntityNotFoundException("El usuario no esta asignado a un equipo");

            var response = new AuthenticateLeaderCodeResponse()
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
            };

            return ResponseBase<AuthenticateLeaderCodeResponse>.Create(response);
        }

        public ResponseBase<AuthenticateEmployeeCodeResponse> AuthenticateEmployeeCodeV2(AuthenticateEmployeeCodeRequest request)
        {
            var user = UoWConsumer.UserRepository
                .Get(x => x.userId == request.UserId)
                .FirstOrDefault();

            if (user == null)
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

            var log = Getlog(request.EmployeeCode, DateTime.Now);
            if (log != null)
                if (log.UserId != request.UserId)
                    throw new UserInUseException("Este codigó ya ha sido usado");
                else
                {
                    if (log.UserCode == request.EmployeeCode)
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
                    else
                        throw new UserInUseException("El usuario inicio sesión con otro codigo de empleado");
                }
            
            bool inTripulacs = false;
            List<string> exceptionMessages = new List<string>();

            //Notificar a la API
            try
            {
                var requestNotify = new NotifyWorkdayRequest();
                //Si es impulsor
                if (routeTeam.roleTeamId == (int)ERolTeam.Impulsor)
                {
                    //Buscar si el ayudante se autenticó antes que el impulsor para registrarlo. 
                    int ayudanteId = UoWConsumer.RouteTeamRepository
                        .Get(x => x.roleTeamId == (int)ERolTeam.Ayudante && x.routeId == request.RouteId)
                        .Select(x => x.userId)
                        .FirstOrDefault();

                    var ayudante = UoWConsumer.AuthentificationLogRepository
                        .Get(x => !x.WasLeaderCodeAuthorization && DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now)
                                    && x.UserId == ayudanteId && x.RouteId == request.RouteId)
                        .FirstOrDefault();

                    string ayudanteCode = ayudante != null ? ayudante.UserCode : null;

                    requestNotify.auxiliarid = Convert.ToInt32(ayudanteCode);
                    requestNotify.impulsorId = Convert.ToInt32(request.EmployeeCode);
                    requestNotify.routeId = Convert.ToInt32(routeBranch.Route.code);
                    requestNotify.posId = Convert.ToInt32(routeBranch.Branch.code);

                    var response = NotifyWorkday(requestNotify);
                    if (response == "\"{\\\"errors\\\":[]}\"")
                    {
                        //Lógica del fallo con el registro en tripulacs
                        inTripulacs = true;
                        if (ayudanteCode != null)
                        {
                            ayudante.Status = true;
                            UoWConsumer.AuthentificationLogRepository.Update(ayudante);
                        }
                    }
                }
                //Si es ayudante
                else
                {
                    //Buscar si ya inicio un Impulsor
                    int? impulsorId = UoWConsumer.RouteTeamRepository
                        .Get(x => x.roleTeamId == (int)ERolTeam.Impulsor && x.routeId == request.RouteId)
                        .Select(x => x.userId)
                        .FirstOrDefault();

                    if (impulsorId == null)
                    {
                        return ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new AuthenticateEmployeeCodeResponse()
                        {
                        });
                        throw new NoUserFoundException("No se encontró al impulsor en WBC. Revisar que exista el impulsor en la ruta por medio de la tabla so_route_team");
                    }

                    var isWorkDayActive = UoWConsumer.WorkDayRepository
                        .Get(x => x.userId == impulsorId);
                    if (isWorkDayActive == null)
                    {
                        return ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new AuthenticateEmployeeCodeResponse()
                        {
                        });
                        throw new WorkdayNotFoundException("No se encontró el Work Day del impulsor.");
                    }


                    var impulsor = UoWConsumer.AuthentificationLogRepository
                        .Get(x => !x.WasLeaderCodeAuthorization && DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now)
                                    && x.UserId == impulsorId && x.RouteId == request.RouteId)
                        .FirstOrDefault();
                    if (impulsor != null)
                    {
                        //Lógica del fallo con el registro en tripulacs
                        inTripulacs = true;
                        var impulsorCode = impulsor.UserCode;
                        requestNotify.auxiliarid = Convert.ToInt32(request.EmployeeCode);
                        requestNotify.impulsorId = Convert.ToInt32(impulsorCode);
                        requestNotify.routeId = Convert.ToInt32(routeBranch.Route.code);
                        requestNotify.posId = Convert.ToInt32(routeBranch.Branch.code);

                        var response = NotifyWorkday(requestNotify);
                        if (response == "\"{\\\"errors\\\":[]}\"")
                        {
                            //Lógica del fallo con el registro en tripulacs
                            inTripulacs = true;
                        }
                        if (response == "\"{\\\"errors\\\":[{\\\"error\\\":9001,\\\"message\\\":\\\"No existe la tripulación configurada para la ruta " + routeBranch.Route.code + ".\\\"}]}\"")
                        {
                            exceptionMessages.Add("No se encuentra configurada la ruta en tripulacs");
                            //throw new NoRouteConfigTripulacsException("No se encuentra configurada la ruta en tripulacs");
                        }
                        if (response == "\"{\\\"errors\\\":[{\\\"error\\\":404,\\\"message\\\":\\\"El id del impulsor no es válido\\\"}]}\"")
                        {
                            exceptionMessages.Add("El id del impulsor no es válido");
                        }
                    }
                    else
                    {
                        throw new RelatedDriverNotFoundException("El impulsor no se ha autenticado");
                    }
                }
            }
            catch (RelatedDriverNotFoundException e)
            {
                throw e;
            }
            catch (UserInUseException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                exceptionMessages.Add("Error al notificar a WSempleados: " + e.Message);
            }

            if (exceptionMessages.Count() > 0)
                throw new ExternalAPIException(exceptionMessages.First());

            var newLogging = new so_authentication_log()
            {
                LeaderAuthenticationCodeId = null,
                WasLeaderCodeAuthorization = false,
                CreatedDate = DateTime.Now,
                LeaderCode = null,
                ModifiedDate = null,
                Status = inTripulacs,
                RouteId = routeBranch.Route.routeId,
                UserCode = request.EmployeeCode,
                UserId = user.userId,
                UserName = employee.name + " " + employee.lastname,
                IsSynchronized = true
            };

            UoWConsumer.AuthentificationLogRepository.Insert(newLogging);
            UoWConsumer.Save();

            var responseAuthentication = ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new AuthenticateEmployeeCodeResponse()
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
            responseAuthentication.Errors = exceptionMessages;
            return responseAuthentication;
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

            if (user == null)
                throw new EntityNotFoundException("No se encuentró al usuario en WByC");

            var routeBranch = UoWConsumer.RouteRepository
                .Get(x => x.routeId == request.RouteId)
                .Select(x => new { Route = x, Code = x.so_branch.so_company.code, Branch = x.so_branch })
                .FirstOrDefault();

            if (routeBranch == null)
                throw new EntityNotFoundException("No se encuentró el branch o la ruta");

            var log = Getlog(request.EmployeeCode, DateTime.Now);
            if (log != null)
                if (log.UserId == request.UserId)
                    if (log.UserCode == request.EmployeeCode)
                        return new ResponseBase<AuthenticateLeaderCodeResponse>()
                        {
                            Data = null,
                            Errors = null,
                            Status = true
                        };
                    else
                        throw new UserInUseException("El usuario inicio sesión con otro codigo de empleado");
                //else
                //    throw new UserInUseException("Este codigó ya ha sido usado");

            var employee = SingleEmployee(routeBranch.Code, request.EmployeeCode);

            var newAuthenticationLog = new so_authentication_log
            {
                LeaderAuthenticationCodeId = leaderCode.Id,
                Status = true,
                WasLeaderCodeAuthorization = true,
                CreatedDate = DateTime.Now,
                UserCode = request.EmployeeCode,
                UserId = user.userId,
                RouteId = request.RouteId,
                LeaderCode = leaderCode.Code,
                UserName = employee == null ? null : employee.name + " " + employee.lastname,
                IsSynchronized = false
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

        private string GetTokenAWSEmployee()
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["wsempleadosURL"]);
            var requestConfig = new RestRequest("/AuthenticateUser", Method.POST);
            requestConfig.RequestFormat = DataFormat.Json;

            requestConfig.AddParameter("username", ConfigurationManager.AppSettings["wsempleadosUSER"]);
            requestConfig.AddParameter("password", ConfigurationManager.AppSettings["wsempleadosPASSWORD"]);
            requestConfig.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            var RestResponse = client.Execute(requestConfig);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var contentString = RestResponse.Content.Replace("</string>", "");
                var aray = contentString.Split('>');
                return aray.Last();
            }

            throw new ExternalAPIException("Falló al intentar obtener el token. " + RestResponse.StatusCode + ": " + RestResponse.ErrorMessage);
        }

        private string NotifyWorkday(NotifyWorkdayRequest request)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIdeOPECDV1"]);
            var requestConfig = new RestRequest("/api/v1/crews", Method.POST);
            requestConfig.RequestFormat = DataFormat.Json;

            requestConfig.AddBody(request);

            var RestResponse = client.Execute(requestConfig);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                throw new Exception(RestResponse.Content);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
                throw new UserInUseException("Este código ya fue agregado");
            return JsonConvert.SerializeObject(RestResponse.Content);
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

        public so_authentication_log Getlog(string userCode, DateTime? date)
        {
            return UoWConsumer.AuthentificationLogRepository
                .Get(x => x.UserCode == userCode && DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(date))
                .FirstOrDefault();
        }

        private void RegisterLog()
        {

        }

        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }
    }
}