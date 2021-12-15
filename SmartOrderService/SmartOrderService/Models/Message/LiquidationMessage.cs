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
        public GetLiquidationRepaymentsResponse()
        {
            Total = 0;
            TotalCard = 0;
            TotalCash = 0;
            Customers = new List<GetLiquidationRepaymentsCustomer>();
        }

        public double Total { get; set; }
        public double TotalCash { get; set; }
        public double TotalCard { get; set; }

        public List<GetLiquidationRepaymentsCustomer> Customers { get; set; }
    }

    public class GetLiquidationRepaymentsCustomer
    {
        public string Name { get; set; }
        public int CustomerId { get; set; }
        public double CashAmount { get; set; }
        public double CardAmount { get; set; }
        public List<GetLiquidationRepaymentsRepayment> Sales { get; set; }
    }

    public class GetLiquidationRepaymentsRepayment
    {
        public int SaleId { get; set; }
        public double Total { get; set; }
        public string PaymentMethod { get; set; }
        public List<GetLiquidationRepaymentsDetail> Details { get; set; }
    }

    public class GetLiquidationRepaymentsDetail
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string BarCode { get; set; }
        public int Amount { get; set; }
        public double ProductPrice { get; set; }
        public double TotalPrice { get; set; }
    }
    #endregion
}