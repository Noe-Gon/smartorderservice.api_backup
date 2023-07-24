using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class BranchLimitTimeException : Exception
    {
        public BranchLimitTimeException() : base()
        {

        }

        public BranchLimitTimeException(string message) : base(message)
        {

        }
    }
}