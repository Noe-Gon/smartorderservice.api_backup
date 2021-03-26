using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartOrderService.Models.DTO;
using SmartOrderService.Services;
using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.Requests;
using SmartOrderService.Utils;

namespace SmartOrderService.Controllers
{
    public class WorkdayController : ApiController
    {
        // GET: api/Workday
        public HttpResponseMessage Get([FromUri]WorkDayRequest request)
        {
            HttpResponseMessage response;

            try
            {
               if(!String.IsNullOrEmpty(request.WorkDayId))
                {
                    Workday workDay = new WorkdayService().getWorkDay(request.WorkDayId);
                    if(workDay == null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.NoContent);
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, workDay);
                    }
                }
               else
                { 

                    var date = DateUtils.getDateTime(request.Date);

                    var service = new WorkdayService();

                    var journeys =  service.RetrieveWorkDay(request.BranchCode, request.UserCode, date);

                    response = Request.CreateResponse(HttpStatusCode.OK,journeys);
                }

            }
            catch (Exception e)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError,e.Message);
            }
           

            return response;

        }

        // GET: api/Workday/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Workday
        public HttpResponseMessage Post([FromBody]Workday workday)
        {
            WorkdayService service = new WorkdayService();
            HttpResponseMessage response;
            try {

                workday = service.createWorkday(workday.UserId);
                response = Request.CreateResponse(HttpStatusCode.Created, workday);
            }
            catch (NotSupportedException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Created);
            }
            catch (NoUserFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Usuario no registrado");
            }
          
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Error: "+e.Message);
            }

            return response;
        }

        // PUT: api/Workday/5
        public HttpResponseMessage Put([FromBody]Workday workday)
        {
            WorkdayService service = new WorkdayService();
            HttpResponseMessage response;
            try
            {
                workday = service.FinishWorkday(workday);
                response = Request.CreateResponse(HttpStatusCode.Accepted, workday);
            }
            catch (NoCustomerVisitException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e.Message );
            }
            catch (WorkdayNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "no se encontro la jornada");
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Error: "+e.Message);
            }

            return response;
        }

        // DELETE: api/Workday/5
        public void Delete(int id)
        {
        }
    }
}
