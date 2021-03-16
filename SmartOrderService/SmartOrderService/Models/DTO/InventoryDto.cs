using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class InventoryDto
    {

        public int InventoryId { get; set; }
        public string Code { get; set; }

        public int Load { get; set; }

        public string Date { get; set; }

        public bool IsCurrent { get; set; }

        public bool Status { get; set; }

        public List<InventoryDetailDto> Details { get; set; }

        public List<InventoryReplacementDto> Replacements { get; set; }

        public List<InventoryArticleDto> Articles { get; set; }

        public InventoryDto()
        {
            Details = new List<InventoryDetailDto>();
            Replacements = new List<InventoryReplacementDto>();
            Articles = new List<InventoryArticleDto>();

        }

    }
}