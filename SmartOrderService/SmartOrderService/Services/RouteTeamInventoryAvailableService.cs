using AutoMapper;
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

        public void UpdateRouteTeamInventory(Sale sale)
        {
            List<SaleDetail> salesDetail = sale.SaleDetails;
            List<SalePromotion> salePromotion = sale.SalePromotions;

            foreach (var productInventory in salesDetail)
            {
                db.so_route_team_inventory_available
                    .Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(productInventory.ProductId))
                    .FirstOrDefault().Available_Amount -= productInventory.Amount;
            }
            db.SaveChanges();

            foreach (var Promotion in salePromotion)
            {
                foreach (var producPromotion in Promotion.DetailProduct)
                {
                    db.so_route_team_inventory_available
                        .Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(producPromotion.ProductId))
                        .FirstOrDefault().Available_Amount -= producPromotion.Amount;
                }
            }
            db.SaveChanges();
        }

        public void UpdateRouteTeamInventory(SaleTeam sale)
        {
            List<SaleDetail> salesDetail = sale.SaleDetails;
            //List<SalePromotion> salePromotion = sale.SalePromotions;
            foreach (var productInventory in salesDetail)
            {
                var product = db.so_route_team_inventory_available.Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(productInventory.ProductId)).FirstOrDefault();
                product.Available_Amount -= productInventory.Amount;
                db.SaveChanges();
            }

            /*
            foreach (var Promotion in salePromotion)
            {
                foreach (var producPromotion in Promotion.DetailProduct)
                {
                    var product = db.so_route_team_inventory_available.Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(producPromotion.ProductId)).FirstOrDefault();
                    product.Available_Amount -= producPromotion.Amount;
                    db.SaveChanges();
                }
            }*/
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
            if (sale.SaleDetailsLoyalty.Count() > 0)
            {
                so_sale_with_points saleWithPoint = new so_sale_with_points
                {
                    saleId = sale.SaleId,
                    date = DateTime.Parse(sale.Date),
                    userId = sale.UserId,
                    customerId = sale.CustomerId,
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
        }

        public List<so_route_team_inventory_available> GetInventoryTeamByInventoryId(int inventoryId)
        {
            return db.so_route_team_inventory_available.Where(s => s.inventoryId.Equals(inventoryId)).ToList();
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