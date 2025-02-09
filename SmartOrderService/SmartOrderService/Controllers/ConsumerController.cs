﻿using Microsoft.AspNet.Identity;
using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class ConsumerController : ApiController
    {
        public ConsumerService GetService()
        {
            return ConsumerService.Create();
        }

        [HttpPost]
        [Route("~/api/consumer")]
        public IHttpActionResult InserConsumer(InsertConsumerRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.InsertConsumer(request);

                    if (response.Status)
                        return Ok(response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (DuplicateEntityException e)
            {
                return Content(HttpStatusCode.Forbidden, ResponseBase<InsertConsumerResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (CRMException e)
            {
                return Content(HttpStatusCode.Conflict, ResponseBase<InsertConsumerResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<InsertConsumerResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }

        [HttpPut]
        [Route("~/api/consumer")]
        public IHttpActionResult UpdateConsumer(UpdateConsumerRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.UpdateConsumer(request);

                    if (response.Status)
                        return Ok(response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (DuplicateEntityException e)
            {
                return Content(HttpStatusCode.Forbidden, ResponseBase<UpdateConsumerResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<UpdateConsumerResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }

        [HttpPost]
        [Route("~/api/consumer/removalrequest")]
        public IHttpActionResult ConsumerRemovalRequest(ConsumerRemovalRequestRequest request)
        {
            using (var service = GetService())
            {
                try
                {
                    var response = service.ConsumerRemovalRequestRequest(request);

                    if (response.Status)
                        return Ok(response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }
        }

        [HttpGet]
        [Route("~/api/consumer")]
        public IHttpActionResult GetConsumers([FromUri]int userId)
        {
            using (var service = GetService())
            {
                try
                {
                    var response = service.GetConsumers(new GetConsumersRequest { userId = userId});

                    if(response.Status)
                        return Ok(response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }
        }

        [HttpPost]
        [Route("~/api/ResendEmail/TicketDigital")]
        public IHttpActionResult ResendTicketDigital(ResendTicketDigitalRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.ResendTicketDigital(request);

                    if (response.Status)
                        return Ok(response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpPost]
        [Route("~/api/ReactivationTicket")]
        public IHttpActionResult ReactivationTicketDigital(ReactivationTicketDigitalRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.ReactivationTicketDigital(request);

                    if (response.Status)
                        return Ok(response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet]
        [Route("~/api/Countries")]
        public IHttpActionResult GetCountries()
        {
            using (var service = GetService())
            {
                var response = service.GetCountries();

                if (response.Status)
                    return Ok(response);
                else
                    return Content(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpGet]
        [Route("~/api/States")]
        public IHttpActionResult GetStates([FromUri]GetStatesRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.GetStates(request);

                    if (response.Status)
                        return Ok(response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<List<GetStatesResponse>>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }

        [HttpGet]
        [Route("~/api/Municipalities")]
        public IHttpActionResult GetMunicipalities([FromUri]GetMunicipalitiesRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.GetMunicipalities(request);

                    if (response.Status)
                        return Ok(response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<List<GetMunicipalitiesResponse>>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }

        [HttpGet]
        [Route("~/api/Neighborhoods")]
        public IHttpActionResult GetNeighborhood([FromUri]GetNeighborhoodsRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.GetNeighborhoods(request);

                    if (response.Status)
                        return Ok(response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<List<GetNeighborhoodsResponse>>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }

        [HttpDelete]
        [Route("~/api/consumer")]
        public IHttpActionResult RemoveConsumer([FromUri]int customerId)
        {
            using (var service = GetService())
            {
                var response = service.RemoveConsumer(new RemoveConsumerRequest { CustomerId = customerId });

                if (response.Status)
                    return Ok(response);

                return Content(HttpStatusCode.BadRequest, response);
            }
        }

        [HttpGet]
        [Route("~/api/CustomerAllInfo")]
        public IHttpActionResult GetCustomerAllInfo([FromUri] PriceRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.GetCustomerAllInfo(request);

                    if (response.Status)
                        return Content(HttpStatusCode.OK, response.Data);

                    return Content(HttpStatusCode.BadRequest, response.Data);
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet]
        [Route("~/api/CustomerUnsynchronized")]
        public IHttpActionResult GetCustomerUnsynchronized([FromUri] GetConsumersRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.GetCustomerUnsynchronized(request);

                    if (response.Status)
                        return Content(HttpStatusCode.OK, response.Data);

                    return Content(HttpStatusCode.BadRequest, response.Data);
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet]
        [Route("~/api/consumer")]
        public IHttpActionResult GetConsumer([FromUri] int customerId)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.GetConsumer(customerId);

                    if (response.Status)
                        return Ok(response);

                    return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (EntityNotFoundException e)
            {
                var response = ResponseBase<GetConsumersResponse>.Create(new List<string>()
                {
                    e.Message
                });

                return Content(HttpStatusCode.NotFound, response);
            }
            catch (Exception e)
            {
                var response = ResponseBase<GetConsumersResponse>.Create(new List<string>()
                {
                    e.Message
                });

                return Content(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost]
        [Route("~/api/Consumer/UnifiedTempCustomer")]
        public IHttpActionResult UnifiedTempCustomer(UnifiedTempCustomerRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.UnifiedTempCustomer(request);

                    if (response.Status)
                        return Ok(response);

                    return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (BadRequestException e)
            {
                var response = ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                });

                return Content(HttpStatusCode.Conflict, response);
            }
            catch (CustomerNotFoundException e)
            {
                var response = ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                });

                return Content(HttpStatusCode.NotFound, response);
            }
            catch (Exception e)
            {
                var response = ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                });

                return Content(HttpStatusCode.InternalServerError, response);
            }
        }
    }
}