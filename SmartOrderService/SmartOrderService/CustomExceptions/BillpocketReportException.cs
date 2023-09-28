using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class BillpocketReportException : Exception
    {
        public BillpocketReportException() : base()
        {

        }

        public BillpocketReportException(string message) : base(message)
        {

        }
    }
}