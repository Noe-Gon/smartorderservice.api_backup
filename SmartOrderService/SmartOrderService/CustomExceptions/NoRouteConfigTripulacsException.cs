using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class NoRouteConfigTripulacsException : Exception
    {
        public NoRouteConfigTripulacsException() : base()
        {

        }

        public NoRouteConfigTripulacsException(string message) : base(message)
        {
        }
    }
}