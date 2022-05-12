using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SmartOrderService.Models;

using SmartOrderService.DB;

namespace SmartOrderService.Mappers
{
    public class SalePromotionDetailProductMapper : IMapper<SalePromotionDetailProduct, so_sale_promotion_detail>
    {
        public so_sale_promotion_detail toEntity(SalePromotionDetailProduct model)
        {
            return new so_sale_promotion_detail()
            {
                productId = model.ProductId,
                amount = model.Amount,
                price = Convert.ToDouble(model.price),
                import = Decimal.ToDouble(model.Import),
                base_price = model.base_price,
                discount = model.discount,
                discount_amount = model.discount_amount,
                discount_percent = model.discount_percent,
                discount_without_taxes = model.discount_without_taxes,
                ieps = model.ieps,
                ieps_fee = model.ieps_fee,
                ieps_fee_rate = model.ieps_fee_rate,
                ieps_rate = model.ieps_rate,
                ieps_snack = model.ieps_snack,
                ieps_snack_rate = model.ieps_snack_rate,
                liters = model.liters,
                net_price = model.net_price,
                price_without_taxes = model.price_without_taxes,
                vat_tax = model.vat,
                vat_tax_rate = model.vat_rate,
                sale_price = model.price
            };
        }

        public SalePromotionDetailProduct toModel(so_sale_promotion_detail entity)
        {
            return new SalePromotionDetailProduct()
            {
                ProductId = entity.productId,
                Amount = entity.amount,
                Status = entity.status,
                base_price = entity.base_price,
                discount = entity.discount,
                discount_amount = entity.discount_amount,
                discount_percent = entity.discount_percent,
                discount_without_taxes = entity.discount_without_taxes,
                ieps = entity.ieps,
                ieps_fee = entity.ieps_fee,
                ieps_fee_rate = entity.ieps_fee_rate,
                ieps_rate = entity.ieps_rate,
                ieps_snack = entity.ieps_snack,
                ieps_snack_rate = entity.ieps_snack_rate,
                liters = entity.liters,
                net_price = entity.net_price,
                price_without_taxes = entity.price_without_taxes,
                vat = entity.vat_tax,
                vat_rate = entity.vat_tax_rate,
                price = entity.sale_price
            };
        }
    }
}