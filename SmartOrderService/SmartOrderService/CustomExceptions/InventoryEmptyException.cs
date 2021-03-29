﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class InventoryEmptyException : Exception
    {
        public InventoryEmptyException() : base()
        {

        }

        public InventoryEmptyException(string message) : base(message)
        {

        }
    }
}