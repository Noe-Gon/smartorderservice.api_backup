using SmartOrderService.DB;
using SmartOrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Mappers
{
    public class SaleReplacementMapper : IMapper<SaleReplacement, so_sale_replacement>
    {
        public so_sale_replacement toEntity(SaleReplacement model)
        {
            return new so_sale_replacement() {
                replacementId = model.ReplacementId,
                amount = model.Amount
            };
        }

        public SaleReplacement toModel(so_sale_replacement entity)
        {
            return new SaleReplacement() {
                ReplacementId = entity.replacementId,
                Amount=entity.amount
            };
        }
    }
}