using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class GetCustomersBlockedResponse
    {
        public int UserId { get; set; }
        public int CustomerId { get; set; }
    }

    public class BlockCustomerResponse
    {
        public string Msg { get; set; }
    }

    public class UnblockCustomerResponse
    {
        public string Msg { get; set; }
    }

    public class ClearBlockedCustomerResponse
    {
        public string Msg { get; set; }
    }
}