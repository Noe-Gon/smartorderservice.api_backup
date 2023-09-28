using SmartOrderService.Models.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_delivery_status : AuditDate
    {
        [Column("deliveryStatusId")]
        public int deliveryStatusId { get; set; }

        [Column("code")]
        public string Code { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("status")]
        public bool Status { get; set; }

        public virtual ICollection<so_delivery_additional_data> DeliveryAdditionalData { get; set; }

        public so_delivery_status() : base()
        {

        }
    }
}