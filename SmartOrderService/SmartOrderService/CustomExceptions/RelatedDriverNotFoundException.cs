using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class RelatedDriverNotFoundException : Exception
    {
        public RelatedDriverNotFoundException(string message) : base(message)
        {

        }
    }
}