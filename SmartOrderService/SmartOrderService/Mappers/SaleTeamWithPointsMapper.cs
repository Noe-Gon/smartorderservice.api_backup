using System;
using SmartOrderService.DB;
using SmartOrderService.Models;

namespace SmartOrderService.Mappers
{
    public class SaleTeamWithPointsMapper : IMapper<SaleTeamWithPoints, so_sale>
    {
        public so_sale toEntity(SaleTeamWithPoints model)
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

        public SaleTeamWithPoints toModel(so_sale entity)
        {
            SaleTeamWithPoints sale = new SaleTeamWithPoints();

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