﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class NoUserFoundException : Exception
    {
        public NoUserFoundException() : base()
        {

        }

        public NoUserFoundException(string message) : base(message)
        {
        }
    }
}