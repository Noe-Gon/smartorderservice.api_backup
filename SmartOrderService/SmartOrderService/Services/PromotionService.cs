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

        public PromotionResult GetSalePromotions(int UserId, int InventaryId)
        {
            List<PromotionGiftArticleCatalogDto> lstGiftArticleCatalog = new List<PromotionGiftArticleCatalogDto>();
            List<PromotionCatalogDto> lstpromotionCatalogDto = new List<PromotionCatalogDto>();
            List<PromotionProductDto> lstpromotionProductDto = new List<PromotionProductDto>();
            List<PromotionGiftProductDto> lstpromotionGiftProductDto = new List<PromotionGiftProductDto>();
            List<PromotionGiftArticleDto> lstpromotionpromotionGiftArticleDto = new List<PromotionGiftArticleDto>();
            PromotionResult PromotionResultDto = new PromotionResult();

            using (var context = db)
            {
                SqlParameter userId = new SqlParameter("@UserId", UserId);
                SqlParameter inventaryId = new SqlParameter("@InventaryId", InventaryId);
                SqlParameter optionId = new SqlParameter("@OptionId", 1);

                //Recuperando el catálogo de articulos
                lstGiftArticleCatalog = context.Database.SqlQuery<PromotionGiftArticleCatalogDto>("sp_getPromotions @UserId, @InventaryId, @OptionId", userId, inventaryId, optionId).ToList();

                //Recuperando la configuración de las promociones
                userId = new SqlParameter("@UserId2", UserId);
                inventaryId = new SqlParameter("@InventaryId2", InventaryId);
                optionId = new SqlParameter("@OptionId2", 2);
                lstpromotionCatalogDto = context.Database.SqlQuery<PromotionCatalogDto>("sp_getPromotions @UserId2, @InventaryId2, @OptionId2", userId, inventaryId, optionId).ToList();

                //Recuperando los productos a vender para la promoción
                userId = new SqlParameter("@UserId3", UserId);
                inventaryId = new SqlParameter("@InventaryId3", InventaryId);
                optionId = new SqlParameter("@OptionId3", 3);
                lstpromotionProductDto = context.Database.SqlQuery<PromotionProductDto>("sp_getPromotions @UserId3, @InventaryId3, @OptionId3", userId, inventaryId, optionId).ToList();

                //Recuperando los productos de regalo de la promoción
                userId = new SqlParameter("@UserId4", UserId);
                inventaryId = new SqlParameter("@InventaryId4", InventaryId);
                optionId = new SqlParameter("@OptionId4", 4);
                lstpromotionGiftProductDto = context.Database.SqlQuery<PromotionGiftProductDto>("sp_getPromotions @UserId4, @InventaryId4, @OptionId4", userId, inventaryId, optionId).ToList();

                //Recuperando los articulos de regalo de la promoción
                userId = new SqlParameter("@UserId5", UserId);
                inventaryId = new SqlParameter("@InventaryId5", InventaryId);
                optionId = new SqlParameter("@OptionId5", 5);
                lstpromotionpromotionGiftArticleDto = context.Database.SqlQuery<PromotionGiftArticleDto>("sp_getPromotions @UserId5, @InventaryId5, @OptionId5", userId, inventaryId, optionId).ToList();
            }

            PromotionResultDto.PromotionGiftArticleCatalogDto = lstGiftArticleCatalog;
            PromotionResultDto.PromotionCatalogDto = lstpromotionCatalogDto;
            PromotionResultDto.PromotionGiftProductDto = lstpromotionGiftProductDto;
            PromotionResultDto.PromotionProductDto = lstpromotionProductDto;
            PromotionResultDto.PromotionGiftArticleDto = lstpromotionpromotionGiftArticleDto;

            return PromotionResultDto;
        }
    }
}