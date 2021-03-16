using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class RegisterCustomerBillingException : Exception
    {


        public RegisterCustomerBillingException(string errorMessage) : base(errorMessage)
        {

        }
    }
}