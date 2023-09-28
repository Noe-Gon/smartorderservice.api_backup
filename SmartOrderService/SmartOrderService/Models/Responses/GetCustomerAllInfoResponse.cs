using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class GetCustomerAllInfoResponse : CustomerDto
    {
        public GetCustomerAllInfoResponse()
        {
            PriceList = new List<PriceDto>();
        }

        public List<PriceDto> PriceList { get; set; }
    }
}