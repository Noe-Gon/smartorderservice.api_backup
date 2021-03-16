using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class CollectBottleDto
    {
        public int CustomerId { get; set; }
        public int InventoryId { get; set; }
        public List<CollectBottleDetailDto> Details { get; set; }


        public CollectBottleDto()
        {
            Details = new List<CollectBottleDetailDto>();

        }

        public bool AddBottle(int ProductId,int Amount) {
            bool result = false;

            var detail = Details.Where(d => d.ProductId == ProductId).FirstOrDefault();

            if (detail != null)
            {

                detail.Amount += Amount;
               
            }
            else {

                Details.Add(new CollectBottleDetailDto() {

                    ProductId = ProductId,
                    Amount = Amount
                });

            }


            return result;
        }

    }
}