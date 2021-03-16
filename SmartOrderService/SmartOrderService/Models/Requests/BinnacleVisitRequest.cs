using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class BinnacleVisitRequest
    {
        public string UserCode { get; set; }
        public string BranchCode { get; set; }
        public string LastSync { get; set; }
    }
}