using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class CustomerNotFoundException : Exception
    {
        public string CustomerCode { set; get; }
        public int CustomerId { set; get; }

        public CustomerNotFoundException(string CustomerCode) : base()
        {
            this.CustomerCode = CustomerCode;
        }

        public CustomerNotFoundException(int CustomerId) : base()
        {
            this.CustomerId = CustomerId;
        }
    }
}