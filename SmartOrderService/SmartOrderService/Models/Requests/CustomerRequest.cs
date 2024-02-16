using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class CustomerRequest : Request
    {
        public int UserId { get; set; }
        public string Code { get; set; }
        public int BranchCode { get; set; }
        public int RouteCode { get; set; }
        public bool SetUpBilling { get; set; }
        public int BrancId { get; set; }

    }

    public class CustomerWithVarioRequest : CustomerRequest
    {
        public int routeId { get; set; }
    }
}