using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Locks
{
    public class SaleTeamLock
    {
        public int LastUser { get; set; }
        public string SaleDate { get; set; }
        public int CustomerId { get; set; }
        public int DeliveryId { get; set; }
        public SaleService SaleService { get; set; }
    }
}