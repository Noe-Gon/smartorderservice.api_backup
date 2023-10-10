using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class Ope20Exception : Exception
    {
        public Ope20Exception() : base()
        {

        }

        public Ope20Exception(string message) : base(message)
        {
        }
    }
}