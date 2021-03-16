using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class PriceRequest : Request
    {
        public int CustomerId { get; set; }
        public int InventoryId { get; set; }
    }
}