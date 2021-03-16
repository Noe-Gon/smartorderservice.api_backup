using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class CellarNoticeRequest
    {
        public int RouteId { get; set; }

        public string Date { get; set; }
    }
}