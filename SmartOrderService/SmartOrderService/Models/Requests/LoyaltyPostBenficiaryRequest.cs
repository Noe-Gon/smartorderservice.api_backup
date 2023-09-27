using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class LoyaltyPostBenficiaryRequest
    {
        public string customerCode { get; set; }
        public string name { get; set; }
    }
}