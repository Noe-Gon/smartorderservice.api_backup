using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class ApiPreventaException : Exception
    {
        public ApiPreventaException() : base()
        {

        }

        public ApiPreventaException(string message) : base(message)
        {

        }
    }

    public class ApiPreventaNoAuthorizationException : Exception
    {
        public ApiPreventaNoAuthorizationException() : base()
        {

        }

        public ApiPreventaNoAuthorizationException(string message) : base(message)
        {

        }
    }
}