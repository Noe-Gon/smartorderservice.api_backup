﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class DeliveryRequest : Request
    {
        public int InventoryId { get; set; }
    }
}