using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class TravelNotStartedException:Exception
    {
        public TravelNotStartedException() : base()
        {

        }

        public TravelNotStartedException(string message) : base(message)
        {

        }
    }
}