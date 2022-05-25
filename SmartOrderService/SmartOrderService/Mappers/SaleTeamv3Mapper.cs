using SmartOrderService.DB;
using SmartOrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Mappers
{
    public class SaleTeamv3Mapper : IMapper<SaleTeamv3, so_sale>
    {
        public so_sale toEntity(SaleTeamv3 model)
        {
            so_sale sale = new so_sale();
            sale.date = DateTime.Parse(model.Date);
            sale.tag = model.CustomerTag;
            sale.total_cash = model.TotalCash;

            if (model.DeliveryId > 0)
                sale.deliveryId = model.DeliveryId;

            sale.inventoryId = model.InventoryId;

            sale.total_credit = model.TotalCredit;
            sale.customerId = model.CustomerId;
            sale.userId = model.UserId;

            return sale;
        }

        public SaleTeamv3 toModel(so_sale entity)
        {
            SaleTeamv3 sale = new SaleTeamv3();

            sale.CustomerId = entity.customerId;
            sale.Date = String.Format("{0:dd/MM/yyyy HH:mm:ss}", entity.date);
            sale.TotalCash = entity.total_cash;
            sale.CustomerTag = entity.tag;
            sale.DeliveryId = entity.deliveryId.HasValue ? entity.deliveryId.Value : 0;
            sale.InventoryId = entity.inventoryId.Value;
            sale.UserId = entity.userId;
            sale.SaleId = entity.saleId;

            return sale;
        }
    }
}