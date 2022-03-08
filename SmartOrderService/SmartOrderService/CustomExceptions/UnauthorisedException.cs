using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class UnauthorisedException : Exception
    {
        public UnauthorisedException() : base()
        {

        }

        public UnauthorisedException(string message) : base(message)
        {

        }
    }
}