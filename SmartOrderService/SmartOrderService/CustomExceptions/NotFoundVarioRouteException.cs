using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class NotFoundVarioRouteException : Exception
    {
        public NotFoundVarioRouteException() : base()
        {

        }

        public NotFoundVarioRouteException(string message) : base(message)
        {

        }
    }
}