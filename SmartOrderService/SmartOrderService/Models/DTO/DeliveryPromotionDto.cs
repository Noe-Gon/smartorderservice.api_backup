using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class DeliveryPromotionDto
    {

    public int DeliveryPromotionId { get; set; }
	
	public int PromotionId { get; set; }
	public double Amount { get; set; }
    public bool Status { get; set; }

    public List<DeliveryPromotionDetailDto> DeliveryPromotionDetailProduct { get; set; }
    public List<DeliveryPromotionDetailArticleDto> DeliveryPromotionDetailArticle { get; set; }


        public DeliveryPromotionDto()
        {
            DeliveryPromotionDetailProduct = new List<DeliveryPromotionDetailDto>();
            DeliveryPromotionDetailArticle = new List<DeliveryPromotionDetailArticleDto>();
        }
    }
}