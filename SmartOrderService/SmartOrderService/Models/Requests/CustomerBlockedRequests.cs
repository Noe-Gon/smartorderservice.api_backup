using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class GetCustomersBlockedRequest
    {
        public int UserId { get; set; }
        public int? InventoryId { get; set; }
    }

    public class BlockCustomerRequest
    {
        public int InventoryId { get; set; }
        public int UserId { get; set; }
        public int CustomerId { get; set; }
    }

    public class UnblockCustomerRequest
    {
        public int InventoryId { get; set; }
        public int UserId { get; set; }
        public int CustomerId { get; set; }
    }

    public class ClearBlockedCustomerRequest
    {
        public int UserId { get; set; }
    }
}