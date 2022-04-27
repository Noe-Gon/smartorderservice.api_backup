using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class InventoryRequest : Request
    {
        public int UserId { get; set; }
        public bool? OnlyCurrent { get; set; }
        public int? InventoryId { get; set; }
    }

    public class LoadInventoryDeliveriesRequest
    {
        public int InventoryId { get; set; }
    }

    public class LoadInventoryDeliveriesResponse
    {
        public string Msg { get; set; }
    }

    #region Response Get Deliveries PreventaApi

    public class PreventaAPIResponseBase<T>
    {
        public bool success { get; set; }
        public string result { get; set; }
        public int code { get; set; }
        public List<T> data { get; set; }
        public string message { get; set; }
    }

    public class DeliveriesPreventaAPIResponse
    {
        public int inventoryId { get; set; }
        public string code { get; set; }
        public int customerId { get; set; }
        public DateTime deliveryDate { get; set; }
        public List<DeliveriesPreventaAPIProduct> products { get; set; }
    }

    public class DeliveriesPreventaAPIProduct
    {
        public int productId { get; set; }
        public int quantity { get; set; }
        public Single price { get; set; }
    }
    #endregion
}