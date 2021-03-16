using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class BadPropiertiesException : Exception
    {
        public BadPropiertiesException() : base()
        {

        }

        public BadPropiertiesException(string message) : base(message)
        {

        }
    }
}