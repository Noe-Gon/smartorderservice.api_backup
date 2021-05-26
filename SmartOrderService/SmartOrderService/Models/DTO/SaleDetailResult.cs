using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models
{
    public class SaleDetailResult : SaleDetail
    {
        public int AmountSold { get; set; }

        public SaleDetailResult()
        {

        }

        public SaleDetailResult(SaleDetail saleDetail) {
            this.PriceValue = saleDetail.PriceValue;
            this.Amount = saleDetail.Amount;
            this.CreditAmount = saleDetail.CreditAmount;
            this.Import = saleDetail.Import;
            this.ProductId = saleDetail.ProductId;
            this.price = saleDetail.price;
            this.vat = saleDetail.vat;
            this.vat_rate = saleDetail.vat_rate;
            this.base_price = saleDetail.base_price;
            this.discount = saleDetail.discount;
            this.discount_amount = saleDetail.discount_amount;
            this.discount_percent = saleDetail.discount_percent;
            this.net_price = saleDetail.net_price;
            this.price_without_taxes = saleDetail.price_without_taxes;
            this.discount_without_taxes = saleDetail.discount_without_taxes;
            this.ieps = saleDetail.ieps;
            this.ieps_fee = saleDetail.ieps_fee;
            this.ieps_snack = saleDetail.ieps_snack;
            this.ieps_rate = saleDetail.ieps_rate;
            this.ieps_fee_rate = saleDetail.ieps_fee_rate;
            this.ieps_snack_rate = saleDetail.ieps_fee_rate;
            this.liters = saleDetail.liters;
        }
    }
}