using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class Request
    {
        public int BranchId { get; set; }
        public string LastUpdate { get; set; }
    }
}