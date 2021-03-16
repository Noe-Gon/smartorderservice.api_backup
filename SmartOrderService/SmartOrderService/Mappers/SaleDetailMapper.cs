using SmartOrderService.DB;
using SmartOrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Mappers
{
    public class SaleDetailMapper : IMapper<SaleDetail, so_sale_detail>
    {
        public so_sale_detail toEntity(SaleDetail model)
        {
            so_sale_detail detail = new so_sale_detail();

            detail.amount = model.Amount;
            detail.productId = model.ProductId;
            detail.credit_amount = model.CreditAmount;
            detail.import = (double)model.Import;
            detail.price = (double)( model.price == null ? 0 : (decimal)model.price);
            detail.base_price = model.base_price;
            detail.discount = model.discount;
            detail.discount_amount = model.discount_amount;
            detail.discount_percent = model.discount_percent;
            detail.net_price = model.net_price;
            detail.price_without_taxes = model.price_without_taxes;
            detail.discount_without_taxes = model.discount_without_taxes;
            detail.vat = model.vat;
            detail.ieps = model.ieps;
            detail.ieps_fee = model.ieps_fee;
            detail.ieps_snack = model.ieps_snack;
            detail.ieps_rate = model.ieps_rate;
            detail.ieps_fee_rate = model.ieps_fee_rate;
            detail.ieps_snack_rate = model.ieps_snack_rate;
            detail.liters = model.liters;
            detail.vat_tax = model.vat;
            detail.vat_tax_rate = model.vat_rate;
            detail.sale_price = model.price;
            

            return detail;
        }

        public SaleDetail toModel(so_sale_detail entity)
        {
            SaleDetail model = new SaleDetail();
            model.Amount = entity.amount;
            model.ProductId = entity.productId;
            model.CreditAmount = entity.credit_amount;
            model.Import = (decimal) entity.import;
            model.PriceValue = (decimal)entity.price;
            model.base_price = entity.base_price;
            model.discount = entity.discount;
            model.discount_amount = entity.discount_amount;
            model.discount_percent = entity.discount_percent;
            model.net_price = entity.net_price;
            model.price_without_taxes = entity.price_without_taxes;
            model.discount_without_taxes = entity.discount_without_taxes;
            model.vat = entity.vat;
            model.ieps = entity.ieps;
            model.ieps_fee = entity.ieps_fee;
            model.ieps_snack = entity.ieps_snack;
            model.vat_rate = entity.vat_tax_rate;
            model.ieps_rate = entity.ieps_rate;
            model.ieps_fee_rate = entity.ieps_fee_rate;
            model.ieps_snack_rate = entity.ieps_snack_rate;
            model.liters = entity.liters;
            model.price = entity.sale_price;
            model.vat = entity.vat_tax;
            model.vat_rate = entity.vat_tax_rate;
            return model;
        }
    }
}