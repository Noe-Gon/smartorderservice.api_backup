using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models
{
    public class SaleCombo
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int Amount { get; set; }
        public int? PromotionReferenceId { get; set; }
        public IList<SaleComboDetail> SaleComboDetails { get; set; }

    }
}