using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class CRMException : Exception
    {
        public CRMException() : base()
        {

        }

        public CRMException(string message) : base(message)
        {
        }
    }
}