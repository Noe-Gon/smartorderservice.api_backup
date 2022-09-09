using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Message
{
    #region Get Liquidation Sales
    public class GetLiquidationSalesRequest
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public DateTime? Date { get; set; }
    }

    public class GetLiquidationSalesResponse
    {
        public GetLiquidationSalesResponse(){
            Total = 0;
            TotalCard = 0;
            TotalCash = 0;
            Customers = new List<GetLiquidationSalesCustomer>();
        }

        public double Total { get; set; }
        public double TotalCash { get; set; }
        public double TotalCard { get; set; }

        public List<GetLiquidationSalesCustomer> Customers { get; set; }
    }

    public class GetLiquidationSalesCustomer
    {
        public string Name { get; set; }
        public int CustomerId { get; set; }
        public double CashAmount { get; set; }
        public double CardAmount { get; set; }
        public List<GetLiquidationSalesSale> Sales { get; set; }
    }

    public class GetLiquidationSalesSale
    {
        public int SaleId { get; set; }
        public double Total { get; set; }
        public string PaymentMethod { get; set; }
        public List<GetLiquidationSalesDetail> Details { get; set; }
    }

    public class GetLiquidationSalesDetail
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string BarCode { get; set; }
        public int Amount { get; set; }
        public double ProductPrice { get; set; }
        public double TotalPrice { get; set; }
    }
    #endregion

    #region Get Liquidation Promotions
    public class GetLiquidationPromotionsRequest
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public DateTime? Date { get; set; }
    }

    public class GetLiquidationPromotionsResponse
    {
        public GetLiquidationPromotionsResponse()
        {
            Total = 0;
            TotalCard = 0;
            TotalCash = 0;
            Customers = new List<GetLiquidationPromotionsCustomer>();
        }

        public double Total { get; set; }
        public double TotalCash { get; set; }
        public double TotalCard { get; set; }

        public List<GetLiquidationPromotionsCustomer> Customers { get; set; }
    }

    public class GetLiquidationPromotionsCustomer
    {
        public string Name { get; set; }
        public int CustomerId { get; set; }
        public double CashAmount { get; set; }
        public double CardAmount { get; set; }
        public List<GetLiquidationPromotionsPromotion> Promotions { get; set; }
    }

    public class GetLiquidationPromotionsPromotion
    {
        public int SaleId { get; set; }
        public string PromotionType { get; set; }
        public double Total { get; set; }
        public string PaymentMethod { get; set; }
        public List<GetLiquidationPromotionsDetail> Details { get; set; }
    }

    public class GetLiquidationPromotionsDetail
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string BarCode { get; set; }
        public int Amount { get; set; }
        public double ProductPrice { get; set; }
        public double TotalPrice { get; set; }
    }
    #endregion

    #region Get Liquidations repayments 
    public class GetLiquidationRepaymentsRequest
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public DateTime? Date { get; set; }
    }

    public class GetLiquidationRepaymentsResponse
    {
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
    }


    #endregion

    #region Get Empty Bottle
    public class GetEmptyBottleRequest
    {
        public int UserId { get; set; }
        public int? InventoryId { get; set; }
        public DateTime? Date { get; set; }
    }

    public class GetEmptyBottleResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public string BarCode { get; set; }
    }
    #endregion

    #region Send Liquidation
    public class SendLiquidationRequest
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public bool CloseInvetories { get; set; }
        public bool CloseSales { get; set; }
        public DateTime? Date { get; set; }
    }

    public class SendLiquidationResponse
    {
        public string Msg { get; set; }
    }
    #endregion

    #region Get Liquidation Status
    public class GetLiquidationStatusRequest
    {
        public int UserId { get; set; }
        public DateTime? Date { get; set; }
    }

    public class GetLiquidationStatusResponse
    {
        public string Code { get; set; }
    }
    #endregion
}