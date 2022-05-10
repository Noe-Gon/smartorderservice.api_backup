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

        public void UpdateRouteTeamInventory(Sale sale)
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