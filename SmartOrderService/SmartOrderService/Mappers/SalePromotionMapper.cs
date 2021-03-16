using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SmartOrderService.DB;
using SmartOrderService.Models;

namespace SmartOrderService.Mappers
{
    public class SalePromotionMapper : IMapper<SalePromotion, so_sale_promotion>
    {
        public so_sale_promotion toEntity(SalePromotion model)
        {
            return new so_sale_promotion()
            {
                promotionId = model.PromotionId,
                amount = model.Amount
            };
        }

        public SalePromotion toModel(so_sale_promotion entity)
        {
            return new SalePromotion() {
                PromotionId = entity.promotionId,
                Amount = entity.amount
            };
        }
    }
}