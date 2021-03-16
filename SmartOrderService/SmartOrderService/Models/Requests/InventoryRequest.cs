using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class InventoryRequest : Request
    {
        public int UserId { get; set; }
        public bool OnlyCurrent { get; set; }
    }
}