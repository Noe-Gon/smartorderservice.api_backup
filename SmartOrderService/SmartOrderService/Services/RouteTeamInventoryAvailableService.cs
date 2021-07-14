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

        public void RecordRouteTeamInventory(int inventoryId)
        {
            var inventoryDetailList = db.so_inventory_detail.Where(s => s.inventoryId.Equals(inventoryId)).ToList();
            using(var dbContextTransaction = db.Database.BeginTransaction())
            {
                foreach(var inventoryDetail in inventoryDetailList)
                {
                    so_route_team_inventory_available routeTeamInventory = new so_route_team_inventory_available();
                    routeTeamInventory.inventoryId = inventoryDetail.inventoryId;
                    routeTeamInventory.productId = inventoryDetail.productId;
                    routeTeamInventory.createOn = DateTime.Today;
                    routeTeamInventory.Available_Amount = inventoryDetail.amount;
                    db.so_route_team_inventory_available.Add(routeTeamInventory);
                    db.SaveChanges();
                }
                dbContextTransaction.Commit();
            }
        }

        public void UpdateRouteTeamInventory(Sale sale)
        {
            List<SaleDetail> salesDetail = sale.SaleDetails;
            List<SalePromotion> salePromotion = sale.SalePromotions;
            foreach (var productInventory in salesDetail)
            {
                var product = db.so_route_team_inventory_available.Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(productInventory.ProductId)).FirstOrDefault();
                product.Available_Amount -= productInventory.Amount;
                db.SaveChanges();
            }
            foreach (var Promotion in salePromotion)
            {
                foreach (var producPromotion in Promotion.DetailProduct)
                {
                    var product = db.so_route_team_inventory_available.Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(producPromotion.ProductId)).FirstOrDefault();
                    product.Available_Amount -= producPromotion.Amount;
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
            return GetInventoryTeamByInventoryId(inventoryId).Where(s => s.Available_Amount > 0).ToList();
        }

    }
}