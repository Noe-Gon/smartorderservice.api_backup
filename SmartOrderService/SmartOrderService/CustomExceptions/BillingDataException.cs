using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class BillingDataException : Exception
    {
        public BillingDataException(string cia) : base("No existe el emisor de facturación (" + cia + ")")
        {

        }
    }
}