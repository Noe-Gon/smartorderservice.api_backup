using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class LoyaltyGetCustomerList
    {
        public string uuid { get; set; }
        public string customerCode { get; set; }
    }
}