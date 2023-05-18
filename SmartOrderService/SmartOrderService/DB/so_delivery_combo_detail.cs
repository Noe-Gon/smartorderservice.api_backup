using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartOrderService.DB
{
    public partial class so_delivery_combo_detail
    {
        [Key]
        public int deliveryComboDetailId { get; set; }

        public int deliveryComboId { get; set; }

        public int productId { get; set; }

        public int amount { get; set; }

        public bool is_gift { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_delivery_combo so_delivery_combo { get; set; }

        public virtual so_product so_product { get; set; }
    }
}