using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Message;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class UserController : ApiController
    {
        // GET: api/User
        public HttpResponseMessage Get([FromUri] UserRequest request)
        {
            
            HttpResponseMessage response;
            try
            {

                var user = new UserService().getUser(request.UserCode,request.Branchcode);
                response = Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (NoUserFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "No se encontro un usuario con esos valores");
            }
                      catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Uppsss...");
            }

            return response;
        }

        // GET: api/User/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/User
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/User/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/User/5
        public void Delete(int id)
        {
        }

        [HttpPut, Route("api/User/UpdateTrackingConfiguration/{userCode}/BranchCode/{branchCode}")]
        public HttpResponseMessage updateUserTrackingConfiguration(string userCode, string branchCode, 
                                                                    [FromBody] TrackingConfigurationDto dto)
        {
            HttpResponseMessage response = null;
            bool success = new UserService().updateTrackingConfiguration(userCode, branchCode, dto.Id);
            response = Request.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.Conflict);
            return response;
        }

        [HttpGet]
        [Route("~/api/Authenticate/EmployeeCode")]
        public IHttpActionResult AuthenticateEmployeeCode(string code, int routeId, int userId)
        {
            try
            {
                using (var service = StaffingComplianceService.Create())
                {
                    var response = service.AuthenticateEmployeeCode(new AuthenticateEmployeeCodeRequest
                    {
                        EmployeeCode = code,
                        RouteId = routeId,
                        UserId = userId
                    });

                    if (response.Status)
                        return Content(HttpStatusCode.Accepted, response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (UserInUseException e)
            {
                return Content(HttpStatusCode.Conflict, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (UnauthorizedAccessException e)
            {
                return Content(HttpStatusCode.Unauthorized, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (ExternalAPIException e)
            {
                return Content(HttpStatusCode.BadRequest, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (EntityNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (NoUserFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (WorkdayNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (NoRouteConfigTripulacsException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }

        [HttpPost]
        [Route("~/api/Authenticate/LeaderCode")]
        public IHttpActionResult AuthenticateLeaderCode(AuthenticateLeaderCodeRequest request)
        {
            try
            {
                using (var service = StaffingComplianceService.Create())
                {
                    var response = service.AuthenticateLeaderCode(request);

                    if (response.Data != null)
                        return Content(HttpStatusCode.Accepted, response);

                    return Content(HttpStatusCode.OK, response);
                }
            }
            catch (LeaderCodeNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<AuthenticateLeaderCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (LeaderCodeExpiredException e)
            {
                return Content(HttpStatusCode.BadRequest, ResponseBase<AuthenticateLeaderCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (EntityNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (UserInUseException e)
            {
                return Content(HttpStatusCode.Conflict, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<AuthenticateLeaderCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }

        [HttpPost]
        [Route("~/api/Authenticate/EmployeeCode/Single")]
        public IHttpActionResult AuthenticateEmployeeCodeSingle(AuthenticateEmployeeCodeRequest request)
        {
            try
            {
                using (var service = StaffingComplianceService.Create())
                {
                    var response = service.AuthenticateEmployeeCodeGet(request);

                    if (response.Status)
                        return Content(HttpStatusCode.Accepted, response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Content(HttpStatusCode.Unauthorized, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (ExternalAPIException e)
            {
                return Content(HttpStatusCode.BadRequest, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (EntityNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (NoUserFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (WorkdayNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (UserInUseException e)
            {
                return Content(HttpStatusCode.Conflict, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }

        [HttpPost]
        [Route("~/api/Authenticate/EmployeeCode/Tripulac")]
        public IHttpActionResult AuthenticateEmployeeCodeTripulac(AuthenticateEmployeeCodeRequest request)
        {
            try
            {
                using (var service = StaffingComplianceService.Create())
                {
                    var response = service.AuthenticateEmployeeCodeV2(request);

                    if (response.Status)
                        return Content(HttpStatusCode.Accepted, response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Content(HttpStatusCode.Unauthorized, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (ExternalAPIException e)
            {
                return Content(HttpStatusCode.BadRequest, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (EntityNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (NoUserFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (WorkdayNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (RelatedDriverNotFoundException e)
            {
                return Content(HttpStatusCode.Forbidden, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (UserInUseException e)
            {
                return Content(HttpStatusCode.Conflict, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<AuthenticateEmployeeCodeResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }
    }
}
