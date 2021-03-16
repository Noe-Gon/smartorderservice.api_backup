using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class CustomerException : Exception
    {
        public CustomerException() : base("No existe el cliente")
        {

        }

        public CustomerException(string message) : base(message)
        {

        }
    }
}