using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
namespace SmartOrderService.Services
{
    public class PromotionService
    {

        private SmartOrderModel db = new SmartOrderModel();

        public PromotionDto getPromotion(int PromotionId)
        {
            var promotion = db.so_promotion.Where(p => p.promotionId == PromotionId).FirstOrDefault();

            return Mapper.Map<PromotionDto>(promotion);
        }

        public List<PromotionDto> getPromotions()
        {
            var promotions = db.so_promotion;

            return Mapper.Map<List<PromotionDto>>(promotions);
        }
    }
}