using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using System.Data.SqlClient;

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

        public List<PromotionResult> GetSalePromotions(int UserId, int ProductId)
        {
            List<PromotionResult> lstPromociones = new List<PromotionResult>();
            
            using (var context = db)
            {
                var userId = new SqlParameter("@UserId", UserId);
                var productId = new SqlParameter("@ProductId", ProductId);
                lstPromociones = context.Database.SqlQuery<PromotionResult>("sp_getPromotions @UserId, @ProductId", userId, productId).ToList();
            }

            return lstPromociones;
        }
    }
}