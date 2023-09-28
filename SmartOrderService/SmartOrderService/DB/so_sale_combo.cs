using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class so_sale_combo
    {
        public so_sale_combo()
        {
            so_sale_combo_details = new List<so_sale_combo_detail>();
        }
        [Key]
        public int saleComboId { get; set; }

        public int saleId { get; set; }

        public int? promotionReferenceId { get; set; }

        public string name { get; set; }

        public string code { get; set; }

        public int amount { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_sale so_sale { get; set; }

        public virtual ICollection<so_sale_combo_detail> so_sale_combo_details { get; set; }
    }
}