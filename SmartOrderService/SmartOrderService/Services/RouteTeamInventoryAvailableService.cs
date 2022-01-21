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
            List<SalePromotion> salePromotion = sale.SalePromotions;
            foreach (var productInventory in salesDetail)
            {
                var product = db.so_route_team_inventory_available.Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(productInventory.ProductId)).FirstOrDefault();
                product.Available_Amount -= productInventory.Amount;
                //verificar si el producto generá un envase vacio
                var bottle = db.so_product_bottle.Where(x => productInventory.ProductId == x.productId).Select(x => x.so_product1).FirstOrDefault();
                //Si es así verificar si existe en el inventario
                if(bottle != null)
                {
                    var exisbottle = db.so_route_team_inventory_available.Where(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(bottle.productId)).FirstOrDefault();
                    if(exisbottle == null) //Si no existe agregarlo
                    {
                        var newbottle = new so_route_team_inventory_available()
                        {
                            Available_Amount = productInventory.Amount,
                            createOn = DateTime.Now,
                            inventoryId = sale.InventoryId,
                            productId = bottle.productId
                        };
                        db.so_route_team_inventory_available.Add(newbottle);
                    }
                    else //Si existe aumentar su cantidad
                    {
                        exisbottle.Available_Amount += productInventory.Amount;
                    }
                }
                
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