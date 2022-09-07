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
        public int UserId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int RouteId { get; set; }
        public int CustomerId { get; set; }
        public double TotalCash { get; set; }
        public double TotalCredit { get; set; }
        public List<SendOrderProduct> Products { get; set; }
    }

    public class NewDeliveryUpdateRequest : SendOrderRequest
    {
        public int OrderId { get; set; }
    }

    public class SendOrderProduct
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Single Price { get; set; }
        public double Import { get; set; }
        public int CreditAmount { get; set; }
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
            originSystem = "wbcprev";
        }

        public DateTime createdOn { get; set; }
        public string orderId { get; set; }
        public string deliveryDate { get; set; }
        public string routeId { get; set; }
        public string branchId { get; set; }
        public int customerId { get; set; }
        public string originSystem { get; set; }
        public SendDeliveryToPreventaAPICustomer customer { get; set; }
        public List<SendDeliveryToPreventaAPIProduct> products { get; set; }
    }

    public class SendDeliveryToPreventaAPICustomer
    {
        public string contact { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public int customerId { get; set; }
        public bool prePaid { get; set; }
        public int originSistemId { get; set; }
        public string originSistem { get; set; }
        public int branchId { get; set; }
        public int routeId { get; set; }
    }

    public class SendDeliveryToPreventaAPIProduct
    {
        public int productId { get; set; }
        public int quantity { get; set; }
        public Single price { get; set; }
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
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public List<GetDeliveriesProduct> Products { get; set; }
    }

    public class GetDeliveriesProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
    }
    #endregion

    #region Delivery delivered
    public class DeliveredRequest
    {
        public int deliveryId { get; set; }
    }

    public class DeliveredResponse
    {
        public string Msg { get; set; }
    }
    #endregion

    #region cancel Delivery
    public class CancelDeliveryRequest
    {
        public int deliveryId { get; set; }
    }

    public class CancelDeliveryResponse
    {
        public string Msg { get; set; }
    }
    #endregion

    #region Update Delivery API PREVENTA
    public class UpdateDeliveryAPIPreventaRequest
    {
        public string groupCode { get; set; }
        public string statusCode { get; set; }
    }

    #endregion

    #region Update Delivery Status
    public class UpdateDeliveryStatus
    {
        public int SaleId { get; set; }
    }
    #endregion
}