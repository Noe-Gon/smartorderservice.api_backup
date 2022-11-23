using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class InventoryInProgressException : Exception
    {
        public InventoryInProgressException() : base()
        {
        }

        public InventoryInProgressException(string message) : base(message)
        {
        }
    }
}