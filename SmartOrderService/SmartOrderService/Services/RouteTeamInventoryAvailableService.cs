using AutoMapper;
using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models;
using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class RouteTeamInventoryAvailableService
    {
        private SmartOrderModel db = new SmartOrderModel();

        public RouteTeamInventoryAvailableService()
        {

        }

        public RouteTeamInventoryAvailableService (SmartOrderModel dbAux)
        {
            this.db = dbAux;
        }

        public List<RouteTeamInventoryDto> GetRouteTeamInventories(int inventoryId)
        {
            var teamInventoryList = GetInventoryTeamByInventoryId(inventoryId);
            List<RouteTeamInventoryDto> routeTeamInventories = new List<RouteTeamInventoryDto>();
            foreach (var inventory in teamInventoryList)
            {
                routeTeamInventories.Add(Mapper.Map<RouteTeamInventoryDto>(inventory));
            }
            return routeTeamInventories;
        }

        public GetRouteTeamInventory GetRouteTeamInventoriesv2(int inventoryId)
        {
            var response = new GetRouteTeamInventory();
            var teamInventoryList = GetInventoryTeamByInventoryId(inventoryId);
            List<RouteTeamInventoryDto> routeTeamInventories = new List<RouteTeamInventoryDto>();
            foreach (var inventory in teamInventoryList)
            {
                routeTeamInventories.Add(Mapper.Map<RouteTeamInventoryDto>(inventory));
            }
            int routeId = (from g in db.so_inventory
                           join i in db.so_route_team
                           on g.userId equals i.userId
                           where g.inventoryId == inventoryId
                           select i.routeId).FirstOrDefault();

            response.Products = routeTeamInventories;
            response.Articles = GetArticlesByRouteId(routeId);
            return response;
        }

        public void UpdateRouteTeamInventory(Sale sale)
        {
            List<SaleDetail> salesDetail = sale.SaleDetails;
            List<SalePromotion> salePromotion = sale.SalePromotions;
            var amountSaled = 0;

            foreach (var saleProduct in salesDetail)
            {
                var availableProduct = db.so_route_team_inventory_available
                    .Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(saleProduct.ProductId))
                    .FirstOrDefault().Available_Amount;
                if (availableProduct >= saleProduct.Amount)
                {
                    db.so_route_team_inventory_available
                    .Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(saleProduct.ProductId))
                    .FirstOrDefault().Available_Amount -= saleProduct.Amount;
                    amountSaled += saleProduct.Amount;
                }
            }
            if (amountSaled == 0)
            {
                throw new EmptySaleException("La venta no se ha podido realizar porque no hay productos disponibles");
            }
            db.SaveChanges();

            var promotionIndex = 0;
            var totalPromotion = 0;
            foreach (var Promotion in salePromotion)
            {
                var amountPromotionsSaled = 0;
                foreach (var productPromotion in Promotion.DetailProduct)
                {
                    var availableProduct = db.so_route_team_inventory_available
                        .Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(productPromotion.ProductId))
                        .FirstOrDefault();
                    if (productPromotion.Amount != 0 && availableProduct.Available_Amount >= productPromotion.Amount)
                    {
                        availableProduct.Available_Amount -= productPromotion.Amount;
                        amountPromotionsSaled += productPromotion.Amount;
                    }
                    else
                    {
                        productPromotion.Amount = 0;
                    }
                }
                sale.SalePromotions[promotionIndex].Amount = amountPromotionsSaled;
                totalPromotion += amountPromotionsSaled;
            }
            if (totalPromotion > 0)
            {
                db.SaveChanges();
            }
        }

        public void UpdateRouteTeamInventory(SaleTeam sale)
        {
            List<SaleDetail> salesDetail = sale.SaleDetails;
            List<SalePromotion> salePromotion = sale.SalePromotions;
            bool isEmptySale = true;

            foreach (var productInventory in salesDetail)
            {
                db.so_route_team_inventory_available
                    .Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(productInventory.ProductId))
                    .FirstOrDefault().Available_Amount -= productInventory.Amount;
                isEmptySale = false;
            }
            db.SaveChanges();
            var promotionIndex = 0;
            foreach (var Promotion in salePromotion)
            {
                var amountPromotionsSaled = 0;
                foreach (var productPromotion in Promotion.DetailProduct)
                {
                    var availableProduct = db.so_route_team_inventory_available
                        .Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(productPromotion.ProductId))
                        .FirstOrDefault();
                    if (productPromotion.Amount != 0 && availableProduct.Available_Amount >= productPromotion.Amount)
                    {
                        availableProduct.Available_Amount -= productPromotion.Amount;
                        amountPromotionsSaled += productPromotion.Amount;
                        isEmptySale = false;
                    }
                    else
                    {
                        productPromotion.Amount = 0;
                    }
                }
                sale.SalePromotions[promotionIndex].Amount = amountPromotionsSaled;
            }
            db.SaveChanges();
            if (isEmptySale)
            {
                throw new EmptySaleException("La venta no se ha podido realizar porque no hay productos disponibles");
            }
        }

        public void UpdateRouteTeamInventory(SaleTeamv3 sale)
        {
            List<SaleDetail> salesDetail = sale.SaleDetails;
            List<SalePromotion> salePromotion = sale.SalePromotions;
            bool isEmptySale = true;
            var amountSaled = 0;

            foreach (var productInventory in salesDetail)
            {
                var availableProduct = db.so_route_team_inventory_available
                    .Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(productInventory.ProductId))
                    .FirstOrDefault().Available_Amount -= productInventory.Amount;
                db.so_route_team_inventory_available
                    .Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(productInventory.ProductId))
                    .FirstOrDefault().modifiedon = DateTime.Now;

                isEmptySale = false;
            }
            db.SaveChanges();
            var promotionIndex = 0;
            foreach (var Promotion in salePromotion)
            {
                var amountPromotionsSaled = 0;
                foreach (var productPromotion in Promotion.DetailProduct)
                {
                    var availableProduct = db.so_route_team_inventory_available
                        .Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(productPromotion.ProductId))
                        .FirstOrDefault();
                    if (productPromotion.Amount != 0 && availableProduct.Available_Amount >= productPromotion.Amount)
                    {
                        availableProduct.Available_Amount -= productPromotion.Amount;
                        availableProduct.modifiedon = DateTime.Now;
                        amountPromotionsSaled += productPromotion.Amount;
                        isEmptySale = false;
                    }
                    else
                    {
                        productPromotion.Amount = 0;
                    }
                }
                sale.SalePromotions[promotionIndex].Amount = amountPromotionsSaled;
            }
            db.SaveChanges();
            if (isEmptySale)
            {
                throw new EmptySaleException("La venta no se ha podido realizar porque no hay productos disponibles");
            }
        }

        public void UpdateRouteTeamInventoryLoyalty(SaleTeamWithPoints sale)
        {
            List<SaleDetail> salesDetail = sale.SaleDetails;
            foreach (var productInventory in salesDetail)
            {
                var product = db.so_route_team_inventory_available.Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(productInventory.ProductId)).FirstOrDefault();
                product.Available_Amount -= productInventory.Amount;
                db.SaveChanges();
            }
            /*
            if (sale.SaleDetailsLoyalty.Count() > 0)
            {
                so_sale_with_points saleWithPoint = new so_sale_with_points
                {
                    saleId = sale.SaleId,
                    date = DateTime.Parse(sale.Date),
                    userId = sale.UserId,
                    customerId = sale.CustomerId,
                    createdby = sale.UserId,
                    createdon = DateTime.Now,
                    modifiedby = sale.UserId,
                    modifiedon = DateTime.Now
                };
                db.so_sale_with_points.Add(saleWithPoint);
                db.SaveChanges();
                foreach (var loyaltyProductInventory in sale.SaleDetailsLoyalty)
                {
                    var loyaltyProductId = db.so_product.Where(p => p.code.Equals(loyaltyProductInventory.code)).Select(p => p.productId).FirstOrDefault();
                    so_sale_with_points_details saleWithPointsDetails = new so_sale_with_points_details
                    {
                        saleWithPointsId = saleWithPoint.saleWithPointsId,
                        productId = loyaltyProductId,
                        Amount = loyaltyProductInventory.Amount,
                        pointsPerUnit = loyaltyProductInventory.points,
                        totalPoints = loyaltyProductInventory.points * loyaltyProductInventory.Amount,
                        createdby = sale.UserId,
                        createdon = DateTime.Now,
                        modifiedby = sale.UserId,
                        modifiedon = DateTime.Now
                    };
                    db.so_sale_with_points_details.Add(saleWithPointsDetails);
                    db.SaveChanges();
                }

                foreach (var loyaltyProductInventory in sale.SaleDetailsLoyalty)
                {
                    var loyaltyProductId = db.so_product.Where(p => p.code.Equals(loyaltyProductInventory.code)).Select(p => p.productId).FirstOrDefault();
                    var product = db.so_route_team_inventory_available.Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(loyaltyProductId)).FirstOrDefault();
                    product.Available_Amount -= loyaltyProductInventory.Amount;
                    db.SaveChanges();
                }
            }
            */
        }

        public List<so_route_team_inventory_available> GetInventoryTeamByInventoryId(int inventoryId)
        {
            return db.so_route_team_inventory_available.Where(s => s.inventoryId.Equals(inventoryId)).ToList();
        }

        public List<RouteTeamInventoryArticle> GetArticlesByRouteId(int routeId)
        {
            var response = (from g in db.so_article_promotional_route
                            join sap in db.so_article_promotional
                            on g.article_promotionalId equals sap.id
                            where g.routeId == routeId
                            select new RouteTeamInventoryArticle
                            {
                                ArticleId = sap.id,
                                AvailableAmount = g.amount
                            }).ToList();

            return response;
        }

        public List<so_route_team_inventory_available> GetRemainingInventory(int inventoryId)
        {
            var inventoryAvailable = db.so_route_team_inventory_available.Where(s => s.inventoryId.Equals(inventoryId)).ToList();
            var inventoryCloneObject = new List<so_route_team_inventory_available>();
            foreach (var routeProduct in inventoryAvailable)
            {
                inventoryCloneObject.Add((so_route_team_inventory_available)routeProduct.Clone());
                routeProduct.Available_Amount = 0;
            }
            db.SaveChanges();
            return inventoryCloneObject.Where(s => s.Available_Amount > 0).ToList();
        }

    }
}