using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class InvoiceException : Exception
    {
        public InvoiceException() { }

        public InvoiceException(string message) : base(message){}
    }
}