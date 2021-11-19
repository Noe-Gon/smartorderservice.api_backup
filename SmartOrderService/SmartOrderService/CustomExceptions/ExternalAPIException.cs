using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class ExternalAPIException : Exception
    {
        public ExternalAPIException() : base()
        {

        }

        public ExternalAPIException(string message) : base(message)
        {

        }
    }
}