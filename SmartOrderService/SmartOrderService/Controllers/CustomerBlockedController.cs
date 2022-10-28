
using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class CustomerBlockedController : ApiController
    {
        public CustomerBlockedService GetService()
        {
            return CustomerBlockedService.Create();
        }

        [HttpPost]
        [Route("~/api/CustomersBlocked")]
        public IHttpActionResult GetCustomersBlocked(GetCustomersBlockedRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.GetCustomersBlocked(request);

                    if (response.Status)
                        return Ok(response);

                    return Content(System.Net.HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, ResponseBase<List<GetCustomersBlockedResponse>>.Create(new List<string>()
                {
                    e.Message
                }));
            }

        }

        [HttpPost]
        [Route("~/api/CustomerBlock")]
        public IHttpActionResult CustomerBlock(BlockCustomerRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var reponse = service.BlockCustomer(request);

                    if (reponse.Status)
                        return Ok(reponse);

                    return Content(System.Net.HttpStatusCode.BadRequest, reponse);
                }
            }
            catch (WorkdayNotFoundException e)
            {
                return Content(System.Net.HttpStatusCode.NotFound, ResponseBase<BlockCustomerResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, ResponseBase<BlockCustomerResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }

        }

        [HttpPost]
        [Route("~/api/CustomerUnblock")]
        public IHttpActionResult CustomerUnblock(UnblockCustomerRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.UnblockCustomer(request);

                    if (response.Status)
                        return Ok(response);

                    return Content(System.Net.HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, ResponseBase<UnblockCustomerResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }

        [HttpPost]
        [Route("~/api/ClearCustomerBlocked")]
        public IHttpActionResult ClearCustomerBlocked(ClearBlockedCustomerRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.ClearBlockedCustomer(request);

                    if (response.Status)
                        return Ok(response);

                    return Content(System.Net.HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, ResponseBase<ClearBlockedCustomerResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }

        }
    }
}