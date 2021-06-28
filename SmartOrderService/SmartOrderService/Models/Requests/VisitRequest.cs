using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class VisitRequest
    {
        public int UserId { get; set; }
        public int? InventoryId { get; set; }
    }
}