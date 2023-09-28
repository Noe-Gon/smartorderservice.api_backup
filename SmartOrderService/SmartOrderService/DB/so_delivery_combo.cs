using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartOrderService.DB
{
    public partial class so_delivery_combo
    {
        public so_delivery_combo()
        {
            so_delivery_combo_details = new HashSet<so_delivery_combo_detail>();
        }

        [Key]
        public int deliveryComboId { get; set; }

        public int deliveryId { get; set; }

        public int? promotionReferenceId { get; set; }

        public string name { get; set; }

        public string code { get; set; }

        public int amount { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }


        public virtual so_delivery so_delivery { get; set; }

        public virtual ICollection<so_delivery_combo_detail> so_delivery_combo_details { get; set; }
    }
}