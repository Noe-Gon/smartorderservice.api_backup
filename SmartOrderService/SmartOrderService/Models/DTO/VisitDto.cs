﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class VisitDto
    {
        public int CustomerId {get;set;}
        public int Order { get; set; }
        public Boolean Visited { get; set; }
    }
}