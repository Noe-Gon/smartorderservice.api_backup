using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class ReceptionBottleService
    {
        private SmartOrderModel db = new SmartOrderModel();

        public void createReceptionBottle(CollectBottleDto dto,int UserId) {

            var reception = new so_reception_bottle();
            var date = DateTime.Now;

            reception.customerId = dto.CustomerId;
            reception.userId = UserId;
            reception.createdby = UserId;
            reception.modifiedby = UserId;
            reception.createdon = date;
            reception.date = date;
            reception.modifiedon = date;
            reception.so_reception_bottle_detail = createDetails(dto.Details);
            reception.status = true;
            reception.inventoryId = dto.InventoryId;

            db.so_reception_bottle.Add(reception);
            db.SaveChanges();


        }

        private List<so_reception_bottle_detail> createDetails(List<CollectBottleDetailDto> details)
        {
            List<so_reception_bottle_detail> newDetails = new List<so_reception_bottle_detail>();

           foreach(var detail in details)
            {
                newDetails.Add(new so_reception_bottle_detail() {
                    productId = detail.ProductId,
                    amount = detail.Amount,
                    status = true
                });

            }

            return newDetails;
        }
    }
}