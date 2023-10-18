using SmartOrderService.CustomExceptions;
using SmartOrderService.Interfaces;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Responses;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class PreSalesController : ApiController
    {
        public IPreSales _preSalesService { get; set; }
        public PreSalesController()
        {
            _preSalesService = new PreSalesService();
        }

        [HttpPost, Route("api/presales/send")]
        public HttpResponseMessage SendPreSales([FromBody] SendPreSalesDTO request)
        {
            HttpResponseMessage response;
            try
            {
                var resultService = _preSalesService.SendPreSales(request);
                response = Request.CreateResponse(HttpStatusCode.OK, ResponseBase<bool>.Create(resultService));
            }
            catch (BadRequestException e)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, ResponseBase<bool?>.Create(new List<string>() {
                    e.Message
                }));
            }
            catch (InternalServerException e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, ResponseBase<bool?>.Create(new List<string>() {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, ResponseBase<bool?>.Create(new List<string>() {
                    e.Message
                }));
            }
            return response;
        }
    }
}