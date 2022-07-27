using SmartOrderService.Models.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_delivery_additional_data : AuditDate
    {
        
        public int? deliveryStatusId { get; set; }

        public virtual so_delivery_status DeliveryStatus { get; set; }

        public int deliveryId { get; set; }
        public so_delivery Delivery { get; set; }

        public so_delivery_additional_data() : base()
        {

        }
    }
}