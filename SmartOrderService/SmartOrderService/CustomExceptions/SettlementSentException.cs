using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class SettlementSentException : Exception
    {
        public SettlementSentException() : base()
        {

        }

        public SettlementSentException(string errorMessage) : base(errorMessage)
        {

        }
    }
}