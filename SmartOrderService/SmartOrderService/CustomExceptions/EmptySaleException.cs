using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class EmptySaleException : Exception
    {
        public EmptySaleException() : base()
        {

        }

        public EmptySaleException(string message) : base(message)
        {

        }
    }
}