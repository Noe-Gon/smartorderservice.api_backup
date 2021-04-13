using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class InventoryNotOpenException:Exception
    {
        public InventoryNotOpenException() : base()
        {

        }

        public InventoryNotOpenException(string message) : base(message)
        {

        }
    }
}