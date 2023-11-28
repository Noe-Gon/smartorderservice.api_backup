using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class InvalidModelException : Exception
    {
        public InvalidModelException() : base()
        {

        }

        public InvalidModelException(string message) : base(message)
        {
        }
    }
}