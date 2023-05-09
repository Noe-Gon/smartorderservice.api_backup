using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class DeliveryDto
    {
        public int DeliveryId { get; set;}

        public int CustomerId { get; set; }

        public int InventoryId { get; set; }

        public string DeliveryCode { get; set; }

        public List<DeliveryDetailDto> DeliveryDetail { get; set; }

        public List<DeliveryReplacementDto> DeliveryReplacements { get; set; }

        public List<DeliveryPromotionDto> DeliveryPromotions { get; set; }
        public IList<DeliveryComboDto> Combos { get; set; }

        public bool Status { get; set; }

        public DeliveryDto()
        {
            DeliveryDetail = new List<DeliveryDetailDto>();
            DeliveryReplacements = new List<DeliveryReplacementDto>();
            DeliveryPromotions = new List<DeliveryPromotionDto>();
            Combos = new List<DeliveryComboDto>();
        }
    }

    public class GetDeliveriesByInventoryResponse
    {
        public int DeliveryId { get; set; }

        public int CustomerId { get; set; }

        public int InventoryId { get; set; }

        public string DeliveryCode { get; set; }

        public int StatusId { get; set; }

        public string StatusName { get; set; }

        public string StatusCode { get; set; }

        public string originSystem { get; set; }

        public string originSystemDescription { get; set; }

        public List<DeliveryDetailDto> DeliveryDetail { get; set; }

        public List<DeliveryReplacementDto> DeliveryReplacements { get; set; }

        public List<DeliveryPromotionDto> DeliveryPromotions { get; set; }

        public bool Status { get; set; }

        public GetDeliveriesByInventoryResponse()
        {
            DeliveryDetail = new List<DeliveryDetailDto>();
            DeliveryReplacements = new List<DeliveryReplacementDto>();
            DeliveryPromotions = new List<DeliveryPromotionDto>();
        }
    }

    public class GetDeliveriesByWorkDay : GetDeliveriesByInventoryResponse
    {

    }
}