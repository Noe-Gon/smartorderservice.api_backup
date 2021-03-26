﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class WorkdayNotFoundException : Exception
    {
        public WorkdayNotFoundException() : base()
        {

        }

        public WorkdayNotFoundException(string message) : base(message)
        {

        }
    }
}