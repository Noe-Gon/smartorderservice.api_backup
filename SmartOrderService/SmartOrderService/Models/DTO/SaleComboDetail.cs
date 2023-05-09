using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models
{
    public class SaleComboDetail
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public bool IsGift { get; set; }
    }
}