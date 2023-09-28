using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class DeliveryComboDto
    {
        public int Id { get; set; }
        public int? PromotionReferenceId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public IList<ComboItemDto> Items { get; set; }
        public string Code { get; set; }
    }
}