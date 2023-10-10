using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class InternalServerException : Exception
    {
        public InternalServerException() : base()
        {

        }

        public InternalServerException(string message) : base(message)
        {

        }
    }
}