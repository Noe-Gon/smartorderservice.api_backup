using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class LoyaltyPointsRequest
    {
        public string uuid { get; set; }
        public int points { get; set; }
    }
}