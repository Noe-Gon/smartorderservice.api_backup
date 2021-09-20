using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Message;
using SmartOrderService.Models.Requests;
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
        public StaffingComplianceService GetService()
        {
            return StaffingComplianceService.Create();
        }
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
        public IHttpActionResult AuthenticateEmployeeCode(string Code)
        {
            using (var service = GetService())
            {
                var response = service.AuthenticateEmployeeCode(new AuthenticateEmployeeCodeRequest
                {
                    EmployeeCode = Code
                });

                if (response.Status)
                    return Ok(response);
                else
                    return Content(HttpStatusCode.BadRequest, response);
            }
        }
    }
}
