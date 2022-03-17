using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_delivery_additional_data
    {
        public int deliveryAdditionalDataId { get; set; }


        public int? deliveryStatusId { get; set; }

        public so_delivery_status DeliveryStatus { get; set; }

        public int deliveryId { get; set; }
        public virtual so_delivery Delivery { get; set; }
    }
}