using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public partial class so_sale_combo_detail
    {
        [Key]
        public int saleComboDetailId { get; set; }

        public int saleComboId { get; set; }

        public int productId { get; set; }

        public int amount { get; set; }

        public bool is_gift { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_sale_combo so_sale_combo { get; set; }

        public virtual so_product so_product { get; set; }
    }
}