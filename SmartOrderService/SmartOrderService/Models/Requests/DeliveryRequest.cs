using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class DeliveryRequest : Request
    {
        public int InventoryId { get; set; }
    }

    public class SendDeliveryToPreventaAPIRequest
    {
        public DateTime createdOn { get; set; }
        public string orderId { get; set; }
        public DateTime deliveryDate { get; set; }
        public string routeId { get; set; }
        public string branchId { get; set; }
        public int customerId { get; set; }
        public string originSystem { get; set; }
        public SendDeliveryToPreventaAPIProduct products { get; set; }
    }

    public class SendDeliveryToPreventaAPIProduct
    {
        public int productId { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
    }
}