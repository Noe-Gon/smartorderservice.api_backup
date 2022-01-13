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

    #region Create Order And Send to APIPreventa
    public class SendOrderRequest
    {
        public string DeliveryDate { get; set; }
        public int RouteId { get; set; }
        public int CustomerId { get; set; }
        public List<SendOrderProduct> Products { get; set; }
    }

    public class SendOrderProduct
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }

    public class SendOrderResponse
    {
        public string Msg { get; set; }
    }
    #endregion

    #region Send Delivery To PreventaAPI
    public class SendDeliveryToPreventaAPIRequest
    {
        public SendDeliveryToPreventaAPIRequest()
        {
            createdOn = DateTime.Now;
            products = new List<SendDeliveryToPreventaAPIProduct>();
        }

        public DateTime createdOn { get; set; }
        public string orderId { get; set; }
        public string deliveryDate { get; set; }
        public string routeId { get; set; }
        public string branchId { get; set; }
        public int customerId { get; set; }
        public string originSystem { get; set; }
        public List<SendDeliveryToPreventaAPIProduct> products { get; set; }
    }

    public class SendDeliveryToPreventaAPIProduct
    {
        public int productId { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
    }
    #endregion

    #region Get Deliveries
    public class GetDeliveriesRequest
    {
        public int UserId { get; set; }
        public int? InventoryId { get; set; }
        public DateTime? Date { get; set; }
    }

    public class GetDeliveriesResponse
    {
        public int UserId { get; set; }
        public int SaleId { get; set; }
        public int DeliveryId { get; set; }
        public string Address { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public int State { get; set; }
        public List<GetDeliveriesProduct> Products { get; set; }
    }

    public class GetDeliveriesProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public double TotalPrice { get; set; }
    }
    #endregion

}