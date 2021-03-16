using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class WorkDayRequest
    {
        public string BranchCode { get; set; }
        public string UserCode { get; set; }
        public string Date { get; set; }
        public string WorkDayId { get; set; }
    }
}