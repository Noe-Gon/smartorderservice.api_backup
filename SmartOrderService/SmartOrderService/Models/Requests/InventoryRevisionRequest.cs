using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class InventoryRevisionRequest
    {

        public int RevisionType { get; set; }
        public int InventoryId { get; set; }
        public int RouteId { get; set; }
        public string Date { get; set; }
        public int RevisionState { get; set; }

    }
}