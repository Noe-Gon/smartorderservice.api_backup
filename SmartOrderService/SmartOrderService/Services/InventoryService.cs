﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartOrderService.DB;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Requests;
using SmartOrderService.CustomExceptions;
using OpeCDLib.Models;
using SmartOrderService.Models.Enum;

namespace SmartOrderService.Services
{
    public class InventoryService
    {
        public static int INVENTORY_AVAILABLE = 0;
        public static int INVENTORY_OPEN = 1;
        public static int INVENTORY_CLOSED = 2;

        private SmartOrderModel db = new SmartOrderModel();
        public so_inventory getCurrentInventory(int userId, DateTime? Date)
        {

            DateTime today = DateTime.Today;

            if (Date != null)
                today = Date.Value.Date;

            var inventory = db.so_inventory.Where(i => i.userId.Equals(userId)
                && i.state.Equals(INVENTORY_OPEN)
                && i.status
                && DbFunctions.TruncateTime(i.date) == DbFunctions.TruncateTime(today)
                ).FirstOrDefault();

            if (inventory == null)
            {
                inventory = db.so_inventory.Where(i => i.userId.Equals(userId)
                && i.state.Equals(INVENTORY_AVAILABLE)
                && i.status
                && DbFunctions.TruncateTime(i.date) == DbFunctions.TruncateTime(today)
                ).OrderBy(i => i.order).FirstOrDefault();
            }

            if (inventory == null)
                throw new InventoryEmptyException();

            if (!IsReady(inventory.inventoryId))
                throw new InventoryInProgressException();

            return inventory;
        }

        public bool CloseInventory(int inventoryId)
        {

            var CurrentInventory = db.so_inventory.Where(i => i.inventoryId == inventoryId).FirstOrDefault();

            if (CurrentInventory != null)
            {
                CurrentInventory.state = INVENTORY_CLOSED;
                CurrentInventory.modifiedon = DateTime.Now;
                db.SaveChanges();

                return true;
            }

            return false;
        }

        [Obsolete]
        public bool inventoryFinish(so_inventory inventory)
        {

            if (inventory == null) throw new InventoryEmptyException();

            var deliveryIdsFromInventory = db.so_delivery.Where(i => i.inventoryId == inventory.inventoryId && i.status).Select(d => d.deliveryId).ToList();

            var deliveryIdsFromDevolutions = db.so_delivery_devolution.Where(d => deliveryIdsFromInventory.Contains(d.deliveryId) && d.status).Select(d => d.deliveryId).ToList();

            var deliveryIdsFromSale = db.so_sale.Where(s => s.inventoryId == inventory.inventoryId && s.status).Select(s => s.deliveryId).ToList();

            deliveryIdsFromInventory.RemoveAll(d => deliveryIdsFromDevolutions.Contains(d));

            deliveryIdsFromInventory.RemoveAll(d => deliveryIdsFromSale.Contains(d));


            return deliveryIdsFromInventory.Count() == 0;
            return true;

        }

        public List<TripDto> getTrips(string BranchCode, DateTime Date)
        {
            var inventories = db.so_inventory.Where(i =>
            i.so_user.so_branch.code.Equals(BranchCode) &&
            i.status &&
            i.state >= InventoryService.INVENTORY_OPEN &&
            DbFunctions.TruncateTime(Date) == i.date
            ).ToList();

            List<TripDto> Trips = new List<TripDto>();

            foreach (var inventory in inventories)
            {
                var trip = new TripDto()
                {
                    UserId = inventory.userId,
                    Order = inventory.order,
                    RouteCode = Int32.Parse(inventory.so_user.code),
                    IsFinished = inventory.state == InventoryService.INVENTORY_CLOSED
                };

                Trips.Add(trip);
            }

            return Trips;
        }

        public void OpenInventory(int inventoryId)
        {
            var inventory = db.so_inventory.Where(i => i.inventoryId.Equals(inventoryId)
            && i.status &&
            i.state < INVENTORY_CLOSED
            ).FirstOrDefault();


            if (inventory == null) {
                throw new EntityNotFoundException();
            }

            inventory.state = INVENTORY_OPEN;
            inventory.modifiedon = DateTime.Now;

            db.SaveChanges();

        }

        public bool IsReady(int InventoryId)
        {
            return db.so_inventory_summary.Where(
                i => i.inventoryId == InventoryId
            ).Count() == 1;
        }

        public List<InventoryDto> getInventories(InventoryRequest request)
        {

            List<InventoryDto> Inventories = new List<InventoryDto>();

            var Today = DateTime.Today;

            int UserId = SearchDrivingId(request.UserId);
            bool OnlyCurrent = request.OnlyCurrent;

            if (OnlyCurrent)
            {
                var currentInventory = getCurrentInventory(UserId, null);

                if (currentInventory == null)
                    throw new InventoryEmptyException();

                Inventories.Add(MapInventory(currentInventory));

            }
            else
            {
                var UserInventories = db.so_inventory.Where(i => i.userId == UserId
                && i.state.Equals(INVENTORY_OPEN)
                && i.status
                && DbFunctions.TruncateTime(i.date) == DbFunctions.TruncateTime(Today)
                ).ToList();

                if (!UserInventories.Any())
                    throw new InventoryEmptyException();

                foreach (var UserInventory in UserInventories)
                {
                    Inventories.Add(MapInventory(UserInventory));
                }

            }


            return Inventories;

        }

        public int getInventoryState(int userId,DateTime date)
        {
            if (date == null) {
                date = DateTime.Today;
            }
            date = Convert.ToDateTime("31/01/2020");
            userId = SearchDrivingId(userId);
            var inventory = db.so_inventory.Where(i => i.userId == userId
            && DbFunctions.TruncateTime(i.date) == DbFunctions.TruncateTime(date)
            ).ToList();
            if (!inventory.Any())
            {
                throw new InventoryEmptyException();
            }
            return inventory.FirstOrDefault().state;
        }

        private int SearchDrivingId(int actualUserId)
        {
            so_route_team teamRoute = db.so_route_team.Where(i => i.userId == actualUserId).ToList().FirstOrDefault();
            if (teamRoute == null)
            {
                throw new RelatedDriverNotFoundException(actualUserId);
            }
            int DrivingId = db.so_route_team.Where(i => i.routeId == teamRoute.routeId && i.roleTeamId == (int)ERolTeam.Impulsor).ToList().FirstOrDefault().userId;
            return DrivingId;
        }

        public InventoryRevisionDto getInventoryByRoute(int routeId, DateTime Date)
        {

            //validar que existe una revision del tipo para la fecha

            var existRevision = db.so_inventory_revisions.Where(r => r.so_revision_states.value.Equals("1")
            && r.routeId == routeId
            //&& DbFunctions.TruncateTime(Date) == DbFunctions.TruncateTime(r.createdOn)           
            ).Count() > 0;

            InventoryRevisionDto dto = null;

            List<InventoryClassification> inventoryClassification = new List<InventoryClassification>();

            var user = db.so_user_route.Where(ur => ur.routeId == routeId && ur.status && ur.so_user.type == 1).FirstOrDefault();

            if (user == null || !existRevision)
                throw new InventoryEmptyException();

            var inventory = getCurrentInventory(user.userId, Date);

            var saleService = new SaleService();

            var sales = saleService.getByInventory(inventory.inventoryId);

            var data = saleService.split(sales, user.so_user.branchId, inventory.inventoryId);

            #region clasificando llenos
            var details = inventory.so_inventory_detail.Where(d => d.status);

            var inventoryLlenos = new InventoryClassification();

            inventoryLlenos.ClassificationId = 1;
            inventoryLlenos.Name = "Llenos";


            var userDevolutions = db.so_user_devolutions.Where(d => d.inventoryId == inventory.inventoryId);

            foreach (var detail in details) {

                var productId = detail.productId;
                var amount = detail.amount;
                var devolution = userDevolutions.Where(d => d.productId.Equals(detail.productId) && d.so_user_reason_devolutions.value.Equals(1)).FirstOrDefault();


                if (data.Products.ContainsKey(productId))
                {
                    amount -= data.Products[productId];
                }

                amount = devolution != null ? (amount - devolution.amount) : amount;

                if (amount > 0)
                    inventoryLlenos.Details.Add(new InventoryClassificationDetail() {
                        ItemId = Int32.Parse(detail.so_product.code),
                        Amount = amount,
                        Name = detail.so_product.name,
                        Code = detail.so_product.code,
                        SubClassifications = getSubClassifications(detail.productId, amount, userDevolutions)

                    });


            }

            var inventoryReplacements = inventory.so_inventory_replacement_detail;

            foreach (var invreplacement in inventoryReplacements)
            {
                int replacementId = invreplacement.replacementId;
                int amount = invreplacement.amount;
                var devolution = userDevolutions.Where(d => d.productId.Equals(invreplacement.replacementId) && d.so_user_reason_devolutions.value.Equals(1)).FirstOrDefault();

                if (data.Replacements.ContainsKey(replacementId)) {
                    amount -= data.Replacements[replacementId];
                }

                amount = devolution != null ? (amount - devolution.amount) : amount;

                if (amount > 0)
                    inventoryLlenos.Details.Add(new InventoryClassificationDetail()
                    {
                        ItemId = Int32.Parse(invreplacement.so_replacement.code),
                        Amount = amount,
                        Name = invreplacement.so_replacement.name,
                        Code = invreplacement.so_replacement.code,
                        SubClassifications = getSubClassifications(invreplacement.replacementId, amount, userDevolutions)
                    });

            }


            #endregion

            #region clasificando vacios
            var inventoryVacios = new InventoryClassification();

            inventoryVacios.ClassificationId = 2;
            inventoryVacios.Name = "Vacios";

            if (data.Bottles.Any()) {

                var bottleIds = data.Bottles.Select(x => x.Key).ToList();
                var bottles = db.so_product.Where(p => p.type == 2 && bottleIds.Contains(p.productId)).ToList();

                foreach (var pair in data.Bottles)
                {
                    var bottle = bottles.Where(b => b.productId == pair.Key).FirstOrDefault();
                    var amount = pair.Value;

                    if (amount > 0)
                        inventoryVacios.Details.Add(new InventoryClassificationDetail() {
                            ItemId = Int32.Parse(bottle.code),
                            Amount = amount,
                            Name = bottle.name,
                            Code = bottle.code,
                            SubClassifications = getSubClassifications(bottle.productId, amount, userDevolutions)
                        });
                }

            }
            #endregion

            #region clasificando implementos

            var implementos = inventory.so_inventory_detail_article.Where(d => d.status);
            var inventoryImplementos = new InventoryClassification();

            inventoryImplementos.ClassificationId = 3;
            inventoryImplementos.Name = "Implementos";

            foreach (var implemento in implementos)
            {
                var devolution = userDevolutions.Where(d => d.productId.Equals(implemento.articleId) && d.so_user_reason_devolutions.value.Equals(1)).FirstOrDefault();
                var amount = implemento.amount;

                amount = devolution != null ? (amount - devolution.amount) : amount;

                if (amount > 0)
                    inventoryImplementos.Details.Add(new InventoryClassificationDetail() {
                        ItemId = Int32.Parse(implemento.so_product.code),
                        Amount = amount,
                        Name = implemento.so_product.name,
                        Code = implemento.so_product.code,
                        SubClassifications = getSubClassifications(implemento.articleId, amount, userDevolutions)
                    });
            }

            #endregion


            inventoryClassification.Add(inventoryLlenos);
            inventoryClassification.Add(inventoryVacios);
            inventoryClassification.Add(inventoryImplementos);

            dto = new InventoryRevisionDto() {
                InventoryId = inventory.inventoryId,
                RouteId = routeId,
                BranchId = user.so_user.so_branch.branchId,
                Date = String.Format("{0:dd/MM/yyyy}", inventory.date),
                Viaje = inventory.order,
                Classifications = inventoryClassification
            };


            return dto;
        }

        private List<ProductState> getSubClassifications(int productId, int productAmount, IQueryable<so_user_devolutions> userDevolutions)
        {
            List<ProductState> subClassifications = new List<ProductState>();


            if (userDevolutions.Any()) {
                var productDevolution = userDevolutions.Where(p => p.productId.Equals(productId) && p.so_user_reason_devolutions.value != 1);

                foreach (var devolution in productDevolution)
                {
                    productAmount -= devolution.amount;

                    subClassifications.Add(new ProductState
                    {
                        Amount = devolution.amount,
                        Name = devolution.so_user_reason_devolutions.description,
                        ProductClassificationId = devolution.so_user_reason_devolutions.user_reason_devolutionId,
                        TipoTran = devolution.so_user_reason_devolutions.value
                    });

                }

            }

            if (productAmount > 0)
                subClassifications.Add(new ProductState
                {
                    Amount = productAmount,
                    Name = "Buen Estado",
                    ProductClassificationId = 6,
                    TipoTran = 2
                });

            return subClassifications;

        }

        private InventoryDto MapInventory(so_inventory inventory)
        {

            InventoryDto dto = new InventoryDto
            {
                InventoryId = inventory.inventoryId,
                Code = inventory.code,
                Date = String.Format("{0:dd/MM/yyyy HH:mm:ss}", inventory.date),
                IsCurrent = inventory.state == 1,
                Load = inventory.order,
                Status = inventory.status

            };



            var details = inventory.so_inventory_detail.Where(d => d.status);

            foreach (var detail in details)
            {
                dto.Details.Add(new InventoryDetailDto()
                {
                    ProductId = detail.productId,
                    Amount = detail.amount,
                    Status = detail.status,
                    base_price = detail.base_price,
                    price = detail.price,
                    discount = detail.discount,
                    discount_amount = detail.discount_amount,
                    discount_percent = detail.discount_percent,
                    ieps = detail.ieps,
                    ieps_fee = detail.ieps_fee,
                    ieps_fee_rate = detail.ieps_fee_rate,
                    ieps_rate = detail.ieps_rate,
                    ieps_snack = detail.ieps_snack,
                    ieps_snack_rate = detail.ieps_snack_rate,
                    liters = detail.liters,
                    net_price = detail.net_price,
                    price_without_taxes = detail.price_without_taxes,
                    discount_without_taxes = detail.discount_without_taxes,
                    vat = detail.vat,
                    vat_rate = detail.vat_rate
                });
            }

            var replacements = inventory.so_inventory_replacement_detail.Where(r => r.status);

            foreach (var replacement in replacements)
            {
                dto.Replacements.Add(new InventoryReplacementDto() {
                    ReplacementId = replacement.replacementId,
                    Amount = replacement.amount,
                    Status = replacement.status
                });
            }


            var articles = inventory.so_inventory_detail_article.Where(a => a.status);

            foreach (var article in articles)
            {
                dto.Details.Add(new InventoryDetailDto() {
                    ProductId = article.articleId,
                    Amount = article.amount,
                    Status = article.status
                });
            }


            return dto;

        }


        public List<int> GetInventoryProductsIds(int inventoryId)
        {
            var inventory = db.so_inventory.Where(i => i.inventoryId.Equals(inventoryId) && i.status).FirstOrDefault();

            var products = inventory.so_inventory_detail.Where(d => d.status).Select(d => d.productId).ToList();

          
            if (products.Any()) {

                // add bottles
                var bottles = db.so_product_bottle.Where(b => products.Contains(b.productId) && b.status).Select(b=> b.bottleId).ToList();

                products.AddRange(bottles);


                //add implements

                var implements = inventory.so_inventory_detail_article.Where(i => i.status).Select(i => i.articleId).ToList();

                products.AddRange(implements);
            }

            return products;

        }
    }
}