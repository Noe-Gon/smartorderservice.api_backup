using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class InventoryReviewRequest
    {

        public int RouteId { get; set; }

        public int Type { get; set; }

        public int State { get; set; }

    }
}