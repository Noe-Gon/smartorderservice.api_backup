using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Locks
{
    public class InventoryLock
    {
        public int key { get; set; }
        public InventoryService InventoryService { get; set; }
    }
}