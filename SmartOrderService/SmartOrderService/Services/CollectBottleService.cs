using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class CollectBottleService
    {
        private SmartOrderModel db = new SmartOrderModel();

        public CollectBottleDto getCollect(int customerId) {

            var collects = db.so_collect_bottle.Where(c => c.customerId == customerId && c.status == 1).ToList();

            CollectBottleDto dto = new CollectBottleDto();

            dto.CustomerId = customerId;

            foreach (var collect in collects) {

                var details = collect.so_collect_bottle_detail.Where(d => d.status == 1);

                foreach(var detail in details)
                {

                    dto.AddBottle(detail.productId,detail.amount);

                }

            }

            return dto;

        }

    }
}