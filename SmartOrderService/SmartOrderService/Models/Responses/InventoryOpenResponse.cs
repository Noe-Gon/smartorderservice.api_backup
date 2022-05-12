using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class InventoryOpenResponse
    {
        public int InventoryId { get; set; }
        public bool IsOpen { get; set; }
    }
}