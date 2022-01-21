using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class ConfigurationValueNotFoundException : Exception
    {
        public ConfigurationValueNotFoundException() : base()
        {

        }

        public ConfigurationValueNotFoundException(string message) : base(message)
        {

        }
    }
}