using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class InventoryNotClosedException:Exception
    {
        public InventoryNotClosedException() : base()
        {

        }

        public InventoryNotClosedException(string message) : base(message)
        {

        }
    }

    public class InventoryNotClosedByUserException : Exception
    {
        public InventoryNotClosedByUserException() : base()
        {

        }

        public InventoryNotClosedByUserException(string message) : base(message)
        {

        }
    }
}