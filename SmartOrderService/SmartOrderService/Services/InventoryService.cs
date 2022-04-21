using System;
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
using RestSharp;
using System.Configuration;
using SmartOrderService.Models.Responses;

namespace SmartOrderService.Services
{
    public class InventoryService : IDisposable
    {
        public static int INVENTORY_AVAILABLE = 0;
        public static int INVENTORY_OPEN = 1;
        public static int INVENTORY_CLOSED = 2;

        private SmartOrderModel db = new SmartOrderModel();
        private RoleTeamService roleTeamService = new RoleTeamService();
        private RouteTeamService routeTeamService = new RouteTeamService();

        public so_inventory GetCurrentInventory(int userId, DateTime? Date)
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

        public bool CloseInventory(int inventoryId, int userId)
        {
            ERolTeam userTeamRole = roleTeamService.GetUserRole(userId);
            if (userTeamRole == ERolTeam.SinAsignar)
            {
                return CloseInventory(inventoryId);
            }

            int driverId = SearchDrivingId(userId);
            var workDay = routeTeamService.GetWorkdayByUserAndDate(driverId, DateTime.Today);

            //Verificar que los demas ya hayan finalizado para cerrar el viaje
            var travelInProgress = db.so_route_team_travels_employees
                .Where(x => x.userId != userId && x.active && x.inventoryId == inventoryId && x.work_dayId == workDay.work_dayId)
                .FirstOrDefault();


            if (travelInProgress == null)
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    if (CloseInventory(inventoryId))
                    {
                        //closingRouteTeamTravelStatus(userId, inventoryId, userTeamRole);
                        CloseUserTravel(inventoryId, userId, workDay);
                        dbContextTransaction.Commit();
                        return true;
                    }
                    return false;
                }
            }
            CloseUserTravel(inventoryId, userId, workDay);
            return true;
        }

        private void CloseUserTravel(int inventoryId, int userId, so_work_day workDay)
        {
            var routeTeamTravel = db.so_route_team_travels_employees
                .Where(s => s.inventoryId.Equals(inventoryId) && s.userId.Equals(userId) && s.work_dayId == workDay.work_dayId)
                .FirstOrDefault();
            routeTeamTravel.active = false;
            db.SaveChanges();
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

        public void OpenInventory(int inventoryId, int userId)
        {
            ERolTeam userTeamRole = roleTeamService.GetUserRole(userId);
            RouteTeamInventoryAvailableService routeTeamInventoryAvailable = new RouteTeamInventoryAvailableService();
            if (userTeamRole == ERolTeam.SinAsignar)
            {
                OpenInventory(inventoryId);
                return;
            }

            if (userTeamRole == ERolTeam.Impulsor)
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    OpenInventory(inventoryId);
                    RecordRouteTeamTravelStatus(userId, inventoryId);
                    RecordRouteTeamInventory(inventoryId);
                    dbContextTransaction.Commit();
                }
                return;
            }
            if (userTeamRole == ERolTeam.Ayudante)
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    RecordRouteTeamTravelStatus(userId, inventoryId);
                    var inventoryOpen = isInventoryOpen(inventoryId, userId);
                    if (!inventoryOpen.IsOpen)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        private void RecordRouteTeamInventory(int inventoryId)
        {
            var inventoryDetailList = db.so_inventory_detail.Where(s => s.inventoryId.Equals(inventoryId)).ToList();
            foreach (var inventoryDetail in inventoryDetailList)
            {
                so_route_team_inventory_available routeTeamInventory = new so_route_team_inventory_available();
                routeTeamInventory.inventoryId = inventoryDetail.inventoryId;
                routeTeamInventory.productId = inventoryDetail.productId;
                routeTeamInventory.createOn = DateTime.Now.ToUniversalTime();
                routeTeamInventory.Available_Amount = inventoryDetail.amount;
                db.so_route_team_inventory_available.Add(routeTeamInventory);
            }
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
            ERolTeam userTeamRole = roleTeamService.GetUserRole(request.UserId);
            int UserId;

            if (!request.OnlyCurrent.HasValue)
            {
                throw new BadRequestException("Falta el parametro OnlyCurrent");
            }

            if (userTeamRole != ERolTeam.SinAsignar) {
                UserId = SearchDrivingId(request.UserId);
            }
            else
            {
                UserId = request.UserId;
            }

            bool OnlyCurrent = request.OnlyCurrent.Value;

            if (OnlyCurrent)
            {
                var currentInventory = GetCurrentInventory(UserId, null);

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

        public int SearchDrivingId(int actualUserId)
        {
            so_route_team teamRoute = db.so_route_team.Where(i => i.userId == actualUserId).FirstOrDefault();
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

            var inventory = GetCurrentInventory(user.userId, Date);

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

        public void RecordRouteTeamTravelStatus(int userId, int inventoryId)
        {
            /*
            int routeId = routeTeamService.searchRouteId(userId);
            Guid workDayId = routeTeamService.GetWorkdayByUserAndDate(userId,DateTime.Today).work_dayId;
            var routeTeamTravel = new so_route_team_travels()
            {
                routeId = routeId,
                inventoryId = inventoryId,
                travelStatus = 1,
                work_dayId = workDayId
            };
            db.so_route_team_travels.Add(routeTeamTravel);
            db.SaveChanges();
            */
            var imppulsorId = SearchDrivingId(userId);
            Guid workDayId = routeTeamService.GetWorkdayByUserAndDate(imppulsorId, DateTime.Today).work_dayId;
            int routeId = routeTeamService.searchRouteId(userId);
            int lastTravelNumber = SearchLastTravelNumber(workDayId, imppulsorId);
            var routeTeamTravel = new so_route_team_travels_employees()
            {
                inventoryId = inventoryId,
                work_dayId = workDayId,
                routeId = routeId,
                travelNumber = lastTravelNumber + 1,
                active = true,
                userId = userId
            };
            db.so_route_team_travels_employees.Add(routeTeamTravel);
            db.SaveChanges();
        }

        private int SearchLastTravelNumber(Guid workDay, int userId)
        {

            var travel = db.so_route_team_travels_employees
                .Where(s => s.work_dayId.Equals(workDay) && s.userId == userId && !s.active)
                .OrderByDescending(s => s.travelNumber)
                .FirstOrDefault();
            if (travel == null)
            {
                return 0;
            }
            return travel.travelNumber;
        }

        public bool CheckInventoryAvailability(int inventoryId, int productId, int amount)
        {
            RouteTeamInventoryAvailableService routeTeamInventoryAvailableService = new RouteTeamInventoryAvailableService();
            var inventoryTeam = routeTeamInventoryAvailableService.GetInventoryTeamByInventoryId(inventoryId);
            var inventoryProduct = inventoryTeam.Where(s => s.productId.Equals(productId)).FirstOrDefault();
            if (inventoryProduct == null)
            {
                throw new ProductNotFoundBillingException("No se encontro el producto con el id " + productId);
            }
            if (amount <= inventoryProduct.Available_Amount)
            {
                return true;
            }
            return false;
        }

        public void TransferUnsoldInventory(int inventoryId, int userId)
        {
            ERolTeam userTeamRole = roleTeamService.GetUserRole(userId);

            if (userTeamRole == ERolTeam.Impulsor) {

                var unsoldProducts = GetUnsoldProducts(inventoryId);
                //si la lista esta vacia, entonces no existen productos no vendidos
                if (!unsoldProducts.Any())
                {
                    return;
                }
                var nextInventory = GetNextInventory(userId);
                //si no se encuentra un siguiente inventario, entonces ya se termino la jornada
                if (nextInventory == null)
                {
                    return;
                }
                SetNextTeamInventory(unsoldProducts, nextInventory.inventoryId, inventoryId);
            }
        }

        public void TransferUnsoldInventory(int userId)
        {
            ERolTeam userTeamRole = roleTeamService.GetUserRole(userId);

            if (userTeamRole == ERolTeam.Impulsor)
            {
                using (var context = db.Database.BeginTransaction()) {
                    try
                    {
                        var currentInventory = GetCurrentInventory(userId);

                        if (currentInventory == null)
                        {
                            return;
                        }

                        var unsoldProducts = GetUnsoldProducts(currentInventory.inventoryId);
                        //si la lista esta vacia, entonces no existen productos no vendidos
                        if (!unsoldProducts.Any())
                        {
                            return;
                        }
                        var nextInventory = GetNextInventory(userId);
                        //si no se encuentra un siguiente inventario, entonces ya se termino la jornada
                        if (nextInventory == null)
                        {
                            return;
                        }
                        SetNextTeamInventory(unsoldProducts, nextInventory.inventoryId, currentInventory.inventoryId);
                        context.Commit();
                    }
                    catch (Exception e)
                    {
                        context.Rollback();
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        public InventoryOpenResponse isInventoryOpen(int inventoryId, int userId)
        {
            ERolTeam userTeamRole = roleTeamService.GetUserRole(userId);
            RouteTeamInventoryAvailableService routeTeamInventoryAvailable = new RouteTeamInventoryAvailableService();
            if (userTeamRole == ERolTeam.Impulsor)
            {
                var inventory = db.so_inventory.Where(x => x.inventoryId == inventoryId && (x.state == INVENTORY_OPEN || x.state == INVENTORY_AVAILABLE)).FirstOrDefault();
                if (inventory != null)
                {
                    return new InventoryOpenResponse
                    {
                        InventoryId = inventoryId,
                        IsOpen = true
                    };
                }
                return new InventoryOpenResponse
                {
                    InventoryId = inventoryId,
                    IsOpen = false
                };
            }
            else//Rol de ayudante
            {
                var inventory = db.so_inventory.Where(x => x.inventoryId == inventoryId && (x.state == INVENTORY_OPEN)).FirstOrDefault();
                if (inventory != null)
                {
                    return new InventoryOpenResponse
                    {
                        InventoryId = inventoryId,
                        IsOpen = true
                    };
                }
                return new InventoryOpenResponse
                {
                    InventoryId = inventoryId,
                    IsOpen = false
                };
            }
        }

        private so_inventory GetCurrentInventory(int userId)
        {
            var currentInventory = db.so_inventory.Where(i => i.userId.Equals(userId)
                && i.state.Equals(INVENTORY_CLOSED)
                && i.status
                && DbFunctions.TruncateTime(i.date) == DbFunctions.TruncateTime(DateTime.Today)
                ).OrderByDescending(i => i.order).FirstOrDefault();
            return currentInventory;
        }

        private void SetNextTeamInventory(List<so_route_team_inventory_available> routeTeamInventory, int nextInventoryId, int currentInventory)
        {
            var inventorySummary = db.so_inventory_summary
                .Where(s => s.inventoryId.Equals(nextInventoryId)).FirstOrDefault();
            foreach (var inventoryProduct in routeTeamInventory)
            {
                var inventoryDetail = db.so_inventory_detail.Where(
                    s => s.inventoryId.Equals(nextInventoryId)
                    && s.productId.Equals(inventoryProduct.productId)).FirstOrDefault();

                if (inventoryDetail == null)
                {
                    var product = db.so_inventory_detail
                        .Where(x => x.productId == inventoryProduct.productId && x.inventoryId == currentInventory)
                        .FirstOrDefault();

                    var newInventoryDetail = new so_inventory_detail
                    {
                        productId = inventoryProduct.productId,
                        inventoryId = nextInventoryId,
                        amount = inventoryProduct.Available_Amount,
                        createdon = DateTime.Now,
                        modifiedon = DateTime.Now,
                        createdby = 2777,
                        modifiedby = 2777,
                        status = true,
                        price = product.price
                    };
                    db.so_inventory_detail.Add(newInventoryDetail);
                    inventorySummary.articles_amount = inventorySummary.articles_amount + 1;
                }
                else
                {
                    inventoryDetail.amount += inventoryProduct.Available_Amount;
                    inventoryDetail.modifiedon = DateTime.Now;
                }
                inventorySummary.products_amount = inventorySummary.products_amount + inventoryProduct.Available_Amount;
                inventoryProduct.Available_Amount = 0;
            }
            inventorySummary.modifiedon = DateTime.Now;
            db.SaveChanges();
        }

        private List<so_route_team_inventory_available> GetUnsoldProducts(int inventoryId)
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

        private so_inventory GetNextInventory(int userId)
        {
            var nextInventory = db.so_inventory.Where(i => i.userId.Equals(userId)
                && i.state.Equals(INVENTORY_AVAILABLE)
                && i.status
                && DbFunctions.TruncateTime(i.date) == DbFunctions.TruncateTime(DateTime.Today)
                ).OrderBy(i => i.order).FirstOrDefault();
            return nextInventory;
        }

        private List<ProductState> getSubClassifications(int productId, int productAmount, IQueryable<so_user_devolutions> userDevolutions)
        {
            List<ProductState> subClassifications = new List<ProductState>();


            if (userDevolutions.Any())
            {
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
                dto.Replacements.Add(new InventoryReplacementDto()
                {
                    ReplacementId = replacement.replacementId,
                    Amount = replacement.amount,
                    Status = replacement.status
                });
            }


            var articles = inventory.so_inventory_detail_article.Where(a => a.status);

            foreach (var article in articles)
            {
                dto.Details.Add(new InventoryDetailDto()
                {
                    ProductId = article.articleId,
                    Amount = article.amount,
                    Status = article.status
                });
            }


            return dto;

        }

        private void closingRouteTeamTravelStatus(int userId, int inventoryId, ERolTeam userRol)
        {
            RouteTeamTravelsService roleTeamService = new RouteTeamTravelsService();
            int routeId = routeTeamService.searchRouteId(userId);
            int driverId = SearchDrivingId(userId);
            var workDay = routeTeamService.GetWorkdayByUserAndDate(driverId, DateTime.Today);
            if (workDay == null)
            {
                throw new WorkdayNotFoundException("No se encontro una jornada relacionada con el usuario " + userId);
            }
            var routeTeamTravels = searchRoutTeamTravels(inventoryId, routeId, workDay.work_dayId);
            EInventoryTeamStatus inventoryStatus = roleTeamService.getTravelStatusByInventoryId(routeTeamTravels.inventoryId);
            if (routeTeamTravels == null)
            {
                throw new EntityNotFoundException();
            }
            if (userRol == ERolTeam.Ayudante)
            {
                if (inventoryStatus != EInventoryTeamStatus.InventarioAbiertoPorAyudante) {
                    throw new InventoryNotOpenException("El ayudante no ha comenzado el viaje");
                }
                routeTeamTravels.travelStatus = (int)EInventoryTeamStatus.InventarioCerradoPorAsistente;
            }
            if (userRol == ERolTeam.Impulsor)
            {
                if (inventoryStatus != EInventoryTeamStatus.InventarioCerradoPorAsistente) {
                    throw new InventoryNotClosedException("El ayudante no ha cerrado el viaje");
                }
                routeTeamTravels.travelStatus = (int)EInventoryTeamStatus.InventarioCerradoPorImpulsor;
            }
            db.SaveChanges();
        }

        private void OpeningRouteTeamTravelStatus(int userId, int inventoryId, ERolTeam userRol)
        {
            RouteTeamTravelsService roleTeamService = new RouteTeamTravelsService();
            int routeId = routeTeamService.searchRouteId(userId);
            int driverId = SearchDrivingId(userId);
            var workDay = routeTeamService.GetWorkdayByUserAndDate(driverId, DateTime.Today);
            if (workDay == null)
            {
                throw new WorkdayNotFoundException("No se encontro una jornada relacionada con el usuario " + userId);
            }
            var routeTeamTravels = searchRoutTeamTravels(inventoryId, routeId, workDay.work_dayId);
            EInventoryTeamStatus inventoryStatus = roleTeamService.getTravelStatusByInventoryId(routeTeamTravels.inventoryId);
            if (routeTeamTravels == null)
            {
                throw new EntityNotFoundException();
            }
            if (userRol == ERolTeam.Ayudante)
            {
                if (inventoryStatus == EInventoryTeamStatus.InventarioCerrado)
                {
                    throw new InventoryNotOpenException("El impulsor no ha comenzado el viaje");
                }
                routeTeamTravels.travelStatus = (int)EInventoryTeamStatus.InventarioAbiertoPorAyudante;
            }
            else if (userRol == ERolTeam.Impulsor)
            {
                routeTeamTravels.travelStatus = (int)EInventoryTeamStatus.InventarioAbiertoPorImpulsor;
            }
            db.SaveChanges();
        }

        private so_route_team_travels searchRoutTeamTravels(int inventoryId, int routeId, Guid workdayId)
        {
            so_route_team_travels routeTeamTravels = db.so_route_team_travels.Where(
                i => i.inventoryId.Equals(inventoryId)
                && i.routeId.Equals(routeId)
                && i.work_dayId.Equals(workdayId)).FirstOrDefault();
            return routeTeamTravels;
        }

        public void CallLoadInventoryProcess(int userId, string branchCode, string routeCode, DateTime? deliveryDate)
        {
            if (deliveryDate == null)
                deliveryDate = DateTime.Now;

            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["ApiV2Url"]);
            var request = new RestRequest("api/CargaInventario", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new
            {
                userId = userId,
                posId = branchCode,
                routeId = routeCode,
                deliveryDate = deliveryDate,
            });

            var response = client.Execute(request);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }
    }
}