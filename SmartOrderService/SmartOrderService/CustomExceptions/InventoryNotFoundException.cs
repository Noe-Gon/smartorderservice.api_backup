using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class InventoryNotFoundException : Exception
    {
        public InventoryNotFoundException() : base()
        {

        }

        public InventoryNotFoundException(string message) : base(message)
        {

        }
    }
}