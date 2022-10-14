using OpeCDLib.Models;
using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Mappers;
using SmartOrderService.Models;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Enum;
using SmartOrderService.Models.Message;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;


namespace SmartOrderService.Services
{
    public class SaleService
    {
        private IMapper<Sale, so_sale> mapper;
        private IMapper<SaleTeam, so_sale> mapperSaleTeam;
        private IMapper<SaleDetail, so_sale_detail> mapperDetails;
        private ListMapper<Sale, so_sale> listMapper;
        private ListMapper<SaleDetail, so_sale_detail> listDetailsMapper;
        private SmartOrderModel db = new SmartOrderModel();
        private readonly object balanceLock = new object();

        public List<Sale> getAll(String branchCode,String userCode) {

            List<so_sale> salesdb =
                db.so_sale
                .Where(sale => sale.so_user.so_branch.code.Equals(branchCode) && sale.so_user.code.Equals(userCode))              
                .ToList();
            List<Sale> sales = new List<Sale>();

            if (salesdb.Any())
            {
                mapper = new SaleMapper();
                listMapper = new ListMapper<Sale, so_sale>(mapper);
                sales.AddRange(listMapper.toModelList(salesdb));

            }

            return sales;
        }

        public List<Venta> getSalesByRoute(string BranchCode, string UserCode, int Trip, DateTime Date, bool Unmodifiable)
        {
            RouteTeamService routeTeamService = new RouteTeamService();

            var user = db.so_user.Where(
                u => u.code.Equals(UserCode) 
                && u.so_branch.code.Equals(BranchCode)
            ).FirstOrDefault();

            if (user == null)
                throw new NoUserFoundException();

            List<so_sale> salesDB = new List<so_sale>();

            if (!routeTeamService.IsImpulsor(user.userId)) {
                 salesDB =
                    db.so_sale
                    .Where(
                        sale => sale.userId.Equals(user.userId)
                        && DbFunctions.TruncateTime(sale.so_inventory.date) == DbFunctions.TruncateTime(Date)
                        && sale.status
                        && sale.so_inventory.order.Equals(Trip)
                    ).ToList();
            }
            else
            {
                List<int> ids = routeTeamService.GetTeamIds(user.userId);
                 salesDB =
                    db.so_sale
                    .Where(
                        sale => ids.Contains(sale.userId)
                        && DbFunctions.TruncateTime(sale.so_inventory.date) == DbFunctions.TruncateTime(Date)
                        && sale.status
                        && sale.so_inventory.order.Equals(Trip)
                    ).ToList();
            }

            if (Unmodifiable)
                salesDB = salesDB.Where(s => s.facturas_so_sale.FirstOrDefault() != null).ToList();

            var OpeCDService = new OpeCDService();

            var Ventas = OpeCDService.CreateVentas(salesDB);

            if (Unmodifiable == false)
            {
                var salesDelivery = salesDB.Where(s => s.deliveryId.HasValue).Select(d => d.deliveryId).ToList();

                var deliveriesDB =
                    db.so_delivery
                    .Where(d =>
                       DbFunctions.TruncateTime(d.so_inventory.date) == DbFunctions.TruncateTime(Date)
                       && d.so_inventory.order.Equals(Trip)
                       && d.so_inventory.userId.Equals(user.userId)
                    ).ToList();

                var deliveriesReached = deliveriesDB.Where(d => !salesDelivery.Contains(d.deliveryId)).ToList();
                Ventas.AddRange(OpeCDService.CreateVentas(deliveriesReached));
            }

            return Ventas;
        }


        public List<so_sale> getSalesByPeriod(int UserId,DateTime Begin, DateTime End) {

            var sales = db.so_sale.Where(
                    s => s.userId.Equals(UserId)
                    && s.createdon.Value.CompareTo(Begin) >= 0
                    && s.createdon.Value.CompareTo(End) <= 0
                    && s.status 
                ).ToList();

            return sales;
        }


        public List<so_sale> getByInventory(int inventoryId)
        {
            var salesdb = db.so_sale_inventory.Where(si => si.inventoryId == inventoryId).Select(x=> x.so_sale).ToList();
            return salesdb;
        }


        public Sale Delete(int saleId)
        {

            var sale = db.so_sale.Where(s => s.saleId.Equals(saleId)).FirstOrDefault();

            if (sale == null)
                throw new EntityNotFoundException();

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    sale.status = false;
                    sale.state = 2;
                    sale.modifiedon = DateTime.Now;

                    db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception();
                }

            }

            var SaleDto = new SaleMapper().toModel(sale);
            return SaleDto;
        }

        public Sale Cancel(int saleId, string PaymentMethod)
        {
            bool lInventarioAfectado = false;
            var sale = db.so_sale.Where(s => s.saleId.Equals(saleId)).FirstOrDefault();

            if (sale == null)
                throw new EntityNotFoundException();

            if(sale.state == 2 && !sale.status)
            {
                lInventarioAfectado = true;
            }

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    sale.status = false;
                    sale.state = 2;
                    sale.modifiedon = DateTime.Now;

                    db.SaveChanges();
                    //Enviar Ticket
                    //SendCancelTicket(sale);

                    if (!lInventarioAfectado)
                    {
                        RestoreInventoryAvailability_v2(saleId);
                    }

                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception();
                }
            }

            var SaleDto = new SaleMapper().toModel(sale);

            return SaleDto;
        }

        public Sale create(Sale sale) {

            lock (this)
            {
                int userId = sale.UserId;
                DateTime date = DateTime.Parse(sale.Date);
                int customerId = sale.CustomerId;

                int deliveryId = sale.DeliveryId;

                var registeredSale = db.so_sale.
                     Where(
                    s => (DateTime.Compare(s.date, date) == 0 || deliveryId.Equals(s.deliveryId.Value))
                     && s.userId.Equals(userId)
                     && s.customerId.Equals(customerId)
                     && s.status
                     ).FirstOrDefault();


                if (registeredSale == null)
                {

                    so_sale entitySale = createSale(sale);
                    entitySale.so_sale_detail = createDetails(sale.SaleDetails, userId);
                    entitySale.so_sale_replacement = createReplacements(sale.SaleReplacements, userId);
                    entitySale.so_sale_promotion = createPromotions(sale.SalePromotions, userId);
                    SetTaxes(entitySale);
                    sale.SaleId = SaveSale(entitySale);
                }

                else
                {
                    sale.SaleId = registeredSale.saleId;
                }
            }

            return sale;
        }

        public Sale UnlockCreate(Sale sale)
        {
            int userId = sale.UserId;
            DateTime date = DateTime.Parse(sale.Date);
            int customerId = sale.CustomerId;
            int deliveryId = sale.DeliveryId;
            var registeredSale = db.so_sale.
                     Where(
                    s => (DateTime.Compare(s.date, date) == 0 || deliveryId.Equals(s.deliveryId.Value))
                     && s.userId.Equals(userId)
                     && s.customerId.Equals(customerId)
                     && s.status
                     ).FirstOrDefault();


            if (registeredSale == null)
            {
                so_sale entitySale = createSale(sale);
                entitySale.so_sale_detail = createDetails(sale.SaleDetails, userId);
                entitySale.so_sale_replacement = createReplacements(sale.SaleReplacements, userId);
                entitySale.so_sale_promotion = createPromotions(sale.SalePromotions, userId);
                SetTaxes(entitySale);
                sale.SaleId = UntransactionalSaveSale(entitySale);
            }
            else
            {
                sale.SaleId = registeredSale.saleId;
            }

            return sale;
        }

        public SaleTeam UnlockCreate(SaleTeam sale)
        {
            int userId = sale.UserId;
            DateTime date = DateTime.Parse(sale.Date);
            int customerId = sale.CustomerId;
            int deliveryId = sale.DeliveryId;

            so_sale registeredSale = null;

            if (deliveryId != 0)
                registeredSale = db.so_sale.
                     Where(
                    s => deliveryId.Equals(s.deliveryId.Value)
                     && s.userId.Equals(userId)
                     && s.customerId.Equals(customerId)
                     && s.status
                     ).FirstOrDefault();
            else
                registeredSale = db.so_sale.
                     Where(
                    s => (DateTime.Compare(s.date, date) == 0)
                     && s.userId.Equals(userId)
                     && s.customerId.Equals(customerId)
                     && s.status
                     && s.inventoryId == sale.InventoryId
                     ).FirstOrDefault();

            if (registeredSale == null)
            {
                so_sale entitySale = createSale(sale);
                entitySale.so_sale_detail = createDetails(sale.SaleDetails, userId);
                entitySale.so_sale_replacement = createReplacements(sale.SaleReplacements, userId);
                var totalPromotion = 0;
                foreach (var promotion in sale.SalePromotions)
                {
                    totalPromotion += promotion.Amount;
                }
                if (totalPromotion > 0)
                {
                    entitySale.so_sale_promotion = createPromotions(sale.SalePromotions, userId);
                }
                SetTaxes(entitySale);
                sale.SaleId = UntransactionalSaveSale(entitySale);

            }
            else
            {
                sale.SaleId = registeredSale.saleId;
            }

            return sale;
        }

        public bool checkIfSaleExist(Sale sale)
        {
                int userId = sale.UserId;
                DateTime date = DateTime.Parse(sale.Date);
                int customerId = sale.CustomerId;

                int deliveryId = sale.DeliveryId;

                var registeredSale = db.so_sale.
                     Where(
                    s => (DateTime.Compare(s.date, date) == 0 || deliveryId.Equals(s.deliveryId.Value))
                     && s.userId.Equals(userId)
                     && s.customerId.Equals(customerId)
                     && s.status
                     ).FirstOrDefault();

                if (registeredSale == null)
                {
                    return false;
                }

                return true;
        }

        public bool checkIfSaleExist(SaleTeam sale)
        {
            int userId = sale.UserId;
            DateTime date = DateTime.Parse(sale.Date);
            int customerId = sale.CustomerId;

            int deliveryId = sale.DeliveryId;
            so_sale registeredSale = null;

            if (deliveryId != 0)
                registeredSale = db.so_sale.
                     Where(
                    s => deliveryId.Equals(s.deliveryId.Value)
                     && s.userId.Equals(userId)
                     && s.customerId.Equals(customerId)
                     && s.status
                     ).FirstOrDefault();
            else
                registeredSale = db.so_sale.
                     Where(
                    s => (DateTime.Compare(s.date, date) == 0)
                     && s.userId.Equals(userId)
                     && s.customerId.Equals(customerId)
                     && s.status
                     ).FirstOrDefault();

            if (registeredSale == null)
            {
                return false;
            }

            return true;
        }

        public Sale CreateSaleResultFromSale(Sale sale)
        {
            RoleTeamService roleTeamService = new RoleTeamService();
            ERolTeam userRole = roleTeamService.GetUserRole(sale.UserId);
            if (userRole == ERolTeam.SinAsignar)
            {
                return sale;
            }
            SaleDetailResultService saleDetailResultService = new SaleDetailResultService();
            InventoryService inventoryService = new InventoryService();
            Sale saleResult = new Sale();
            saleResult.UserId = sale.UserId;
            saleResult.TotalCash = sale.TotalCash;
            saleResult.SaleId = sale.SaleId;
            saleResult.TotalCredit = sale.TotalCredit;
            saleResult.CustomerTag = sale.CustomerTag;
            saleResult.InventoryId = sale.InventoryId;
            saleResult.Date = sale.Date;
            saleResult.CustomerId = sale.CustomerId;
            saleResult.DeliveryId = sale.DeliveryId;
            saleResult.SaleDetails = new List<SaleDetail>();

            for (int i = 0; i < sale.SaleDetails.Count(); i++)
            {
                int amountSaled = 0;
                SaleDetailResult saleDetailResult = new SaleDetailResult(sale.SaleDetails[i]);

                if (inventoryService.CheckInventoryAvailability(sale.InventoryId, sale.SaleDetails[i].ProductId, sale.SaleDetails[i].Amount))
                {
                    amountSaled = sale.SaleDetails[i].Amount;
                    saleResult.SaleDetails.Add(saleDetailResult);
                }
                else
                {
                    sale.TotalCash -= Decimal.ToDouble(sale.SaleDetails[i].Import);
                    sale.SaleDetails.RemoveAt(i);
                    i--;
                }
                saleDetailResult.AmountSold = amountSaled;
            }
            //saleResult.SalePromotions = sale.SalePromotions;
            saleResult.SalePromotions = new List<SalePromotion>();
            for (int i = 0; i < sale.SalePromotions.Count(); i++)
            {
                int amountSaled = 0;
                SalePromotion salePromotionResult = new SalePromotion(sale.SalePromotions[i]);

                List<SalePromotionDetailProduct> promotionProducts = salePromotionResult.DetailProduct;

                for (int j = 0; j < sale.SalePromotions[i].DetailProduct.Count(); j++)
                {
                    if (inventoryService.CheckInventoryAvailability(sale.InventoryId, promotionProducts[j].ProductId, promotionProducts[j].Amount))
                    {
                        amountSaled += promotionProducts[j].Amount;
                    }
                    else
                    {
                        salePromotionResult.DetailProduct[j].Amount = 0;
                        sale.TotalCash -= Decimal.ToDouble(promotionProducts[j].Import);
                        sale.SalePromotions[i].DetailProduct.RemoveAt(j);
                        j--;
                    }
                }
                salePromotionResult.Amount = amountSaled;
                saleResult.SalePromotions.Add(salePromotionResult);
            }
            saleResult.TotalCash = Math.Round(sale.TotalCash, 3);
            saleResult.SaleReplacements = sale.SaleReplacements;
            return saleResult;
        }

        public SaleTeam CreateSaleResultFromSale(SaleTeam sale)
        {
            RoleTeamService roleTeamService = new RoleTeamService();
            ERolTeam userRole = roleTeamService.GetUserRole(sale.UserId);
            if (userRole == ERolTeam.SinAsignar)
            {
                return sale;
            }
            SaleDetailResultService saleDetailResultService = new SaleDetailResultService();
            InventoryService inventoryService = new InventoryService();
            SaleTeam saleResult = new SaleTeam();
            saleResult.UserId = sale.UserId;
            saleResult.TotalCash = sale.TotalCash;
            saleResult.SaleId = sale.SaleId;
            saleResult.TotalCredit = sale.TotalCredit;
            saleResult.CustomerTag = sale.CustomerTag;
            saleResult.InventoryId = sale.InventoryId;
            saleResult.Date = sale.Date;
            saleResult.CustomerId = sale.CustomerId;
            saleResult.DeliveryId = sale.DeliveryId;
            saleResult.SaleDetails = new List<SaleDetail>();
            saleResult.PaymentMethod = sale.PaymentMethod;
            saleResult.EmailDeliveryTicket = sale.EmailDeliveryTicket;
            saleResult.Email = sale.Email;

            for (int i = 0; i < sale.SaleDetails.Count(); i++)
            {
                int amountSaled = 0;
                SaleDetailResult saleDetailResult = new SaleDetailResult(sale.SaleDetails[i]);

                if (inventoryService.CheckInventoryAvailability(sale.InventoryId, sale.SaleDetails[i].ProductId, sale.SaleDetails[i].Amount))
                {
                    amountSaled = sale.SaleDetails[i].Amount;
                    saleResult.SaleDetails.Add(saleDetailResult);
                }
                else
                {
                    sale.TotalCash -= Decimal.ToDouble(sale.SaleDetails[i].Import);
                    sale.SaleDetails.RemoveAt(i);
                    i--;
                }
                saleDetailResult.AmountSold = amountSaled;
            }
            //saleResult.SalePromotions = sale.SalePromotions;
            saleResult.SalePromotions = new List<SalePromotion>();
            for (int i = 0; i < sale.SalePromotions.Count(); i++)
            {
                int amountSaled = 0;
                SalePromotion salePromotionResult = new SalePromotion(sale.SalePromotions[i]);

                List<SalePromotionDetailProduct> promotionProducts = salePromotionResult.DetailProduct;

                for (int j = 0; j < sale.SalePromotions[i].DetailProduct.Count(); j++)
                {
                    if (inventoryService.CheckInventoryAvailability(sale.InventoryId, promotionProducts[j].ProductId, promotionProducts[j].Amount))
                    {
                        amountSaled += promotionProducts[j].Amount;
                    }
                    else
                    {
                        salePromotionResult.DetailProduct[j].Amount = 0;
                        sale.TotalCash -= Decimal.ToDouble(promotionProducts[j].Import);
                        sale.SalePromotions[i].DetailProduct.RemoveAt(j);
                        j--;
                    }
                }
                salePromotionResult.Amount = amountSaled;
                saleResult.SalePromotions.Add(salePromotionResult);
            }
            saleResult.TotalCash = Math.Round(sale.TotalCash, 3);
            saleResult.SaleReplacements = sale.SaleReplacements;
            return saleResult;
        }

        private void SetTaxes(so_sale entitySale)
        {
            so_user user = db.so_user.FirstOrDefault(x => x.userId == entitySale.userId);
            so_branch branch = user != null ? user.so_branch : null;
            int branchId = branch != null ? branch.branchId : 0;

            so_branch_tax branch_tax = db.so_branch_tax.FirstOrDefault(x => x.branchId == branchId && x.status);
            so_products_price_list master_price_list = db.so_products_price_list.FirstOrDefault(x => x.is_master && x.branchId == branchId && x.status);
            so_products_price_list price_list = db.so_products_price_list.
                                                Join(db.so_customer_products_price_list,
                                                    PL => PL.products_price_listId,
                                                    CPL => CPL.products_price_listId,
                                                    (PL, CPL) => new { PL, CPL }).
                                                Where(r => r.PL.branchId == branchId && r.PL.status && r.CPL.customerId == entitySale.customerId && r.CPL.status
                                                && DbFunctions.TruncateTime(r.PL.validity_start) <= DbFunctions.TruncateTime(DateTime.Today)
                                                && DbFunctions.TruncateTime(r.PL.validity_end) >= DbFunctions.TruncateTime(DateTime.Today)).
                                                Select(x => x.PL).FirstOrDefault();
            foreach (so_sale_detail sd in entitySale.so_sale_detail)
                SetSaleTax(sd, branch_tax, master_price_list, price_list);
            
            
            foreach (so_sale_promotion p in entitySale.so_sale_promotion)
                foreach (so_sale_promotion_detail pd in p.so_sale_promotion_detail)
                    SetPromotionTax(pd, branch_tax, master_price_list, price_list);
        }

        private ICollection<so_sale_promotion> createPromotions(List<SalePromotion> salePromotions, int userId)
        {
            List<so_sale_promotion> promotions = new List<so_sale_promotion>();

            if(salePromotions!=null)
            foreach (SalePromotion promotion in salePromotions) {

                so_sale_promotion entityPromotion = new so_sale_promotion() {

                    promotionId = promotion.PromotionId,
                    amount = promotion.Amount,
                    status = true,
                    so_sale_promotion_detail = createPromotionDetails(promotion.DetailProduct,userId),
                    modifiedby = userId,
                    createdby = userId,
                    createdon = DateTime.Now,
                    modifiedon = DateTime.Now,
                    
                };

                promotions.Add(entityPromotion);

            }

            return promotions;

        }

        public SalesData split(List<so_sale> sales, int branchId, int inventoryId)
        {

            var bottlesConfig = db.so_product_bottle.Where(b => b.status).Select(x=> new { x.productId,  x.bottleId }).ToList();

            var products = new Dictionary<int,int>();
            var bottles = new Dictionary<int, int>();
            var replacements = new Dictionary<int, int>();


            foreach (var sale in sales)
            {

                #region sale_details
                var details = sale.so_sale_detail;


                foreach (var detail in details) {
                    var productId = detail.productId;
                    var amount = detail.amount;

                    if (products.ContainsKey(productId))
                    {
                        amount += products[productId];
                        products.Remove(productId);
                    }

                    products.Add(productId, amount);


                    var bottleId = bottlesConfig.Where(b => b.productId == productId).Select(x => x.bottleId).FirstOrDefault();

                    if (bottleId > 0) {
                        int bottlesAmount = amount;

                        if (bottles.ContainsKey(bottleId))
                        {
                           // bottlesAmount += bottles[bottleId];
                            bottles.Remove(bottleId);                            
                        }

                        bottles.Add(bottleId, bottlesAmount);

                    }
                    

                }
                #endregion

                #region sale_promotions                
                var promotions = sale.so_sale_promotion;
                foreach (var promotion in promotions)
                {

                    var promotionDetails = promotion.so_sale_promotion_detail;

                    foreach (var detail in promotionDetails) {
                        var productId = detail.productId;
                        var amount = detail.amount;


                        if (products.ContainsKey(productId))
                        {
                            amount += products[productId];
                            products.Remove(productId);
                            
                        }

                        products.Add(productId, amount);


                        var bottleId = bottlesConfig.Where(b => b.productId == productId).Select(x => x.bottleId).FirstOrDefault();

                        if (bottleId > 0)
                        {
                            var bottlesAmount = amount;

                            if (bottles.ContainsKey(bottleId))
                            {
                                bottlesAmount += bottles[bottleId];
                                bottles.Remove(bottleId);                                
                            }

                            bottles.Add(bottleId, bottlesAmount);

                        }

                    }                    

                }

                #endregion

                #region sale replacements

                var salereplacements = sale.so_sale_replacement;

                foreach (var replacement in salereplacements) {
                    var replacementId = replacement.replacementId;
                    var amount = replacement.amount;


                    if (replacements.ContainsKey(replacementId))
                    {                   
                        amount += replacements[replacementId];
                        replacements.Remove(replacementId);
                    }

                    replacements.Add(replacementId, amount);


                }
                
                #endregion


            }

            #region Reception Bottle
            var receptionsBottle = db.so_reception_bottle.
                Join(db.so_reception_bottle_detail, rb => rb.reception_bottleId, rbd => rbd.reception_bottleId, (rb, rbd) => new { rb, rbd }).
                Where(r => r.rb.inventoryId == inventoryId && r.rb.status && r.rbd.status).Select(r => r.rbd);

            foreach (var reception in receptionsBottle)
            {
                var productId = reception.productId;
                var amount = reception.amount;
                if (bottles.ContainsKey(productId))
                {
                    amount += bottles[productId];
                    bottles.Remove(productId);
                }
                bottles.Add(productId, amount);
            }

            #endregion

            #region remove_bottles_saled
            if (products.Any() && bottles.Any())

            foreach(var pair in products)
            {
                var productId = pair.Key;

                if (bottles.ContainsKey(productId))
                {
                    var amount = bottles[productId] - pair.Value;
                    bottles.Remove(productId);
                    bottles.Add(productId, amount);
                }

            }
            #endregion

            SalesData data = new SalesData();

            data.Products = products;
            data.Bottles = bottles;
            data.Replacements = replacements;

            return data;

        }

        public void UpdateRouteTeamInventory(Sale sale)
        {
            RoleTeamService roleTeamService = new RoleTeamService();
            ERolTeam userRole = roleTeamService.GetUserRole(sale.UserId);
            if (userRole == ERolTeam.SinAsignar)
            {
                return;
            }
            RouteTeamInventoryAvailableService routeTeamInventoryAvailable = new RouteTeamInventoryAvailableService();
            routeTeamInventoryAvailable.UpdateRouteTeamInventory(sale);
        }

        public void UpdateRouteTeamInventory(SaleTeam sale, SmartOrderModel dbAux)
        {
            RoleTeamService roleTeamService = new RoleTeamService();
            ERolTeam userRole = roleTeamService.GetUserRole(sale.UserId);
            if (userRole == ERolTeam.SinAsignar)
            {
                return;
            }
            RouteTeamInventoryAvailableService routeTeamInventoryAvailable = new RouteTeamInventoryAvailableService(dbAux);
            routeTeamInventoryAvailable.UpdateRouteTeamInventory(sale);
        }

        public void RestoreInventoryAvailability(int saleId)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    int inventoryId = db.so_sale.Where(s => s.saleId.Equals(saleId)).FirstOrDefault().inventoryId.Value;
                    var saleDetail = db.so_sale_detail.Where(s => s.saleId.Equals(saleId))
                        .Select(a => new
                        {
                            a.amount,
                            a.productId
                        }).ToList();

                    var promotions = db.so_sale_promotion.Where(s => s.saleId.Equals(saleId));

                    if (promotions.Count() == 0)
                    {
                        foreach (var sale in saleDetail)
                        {
                            int amountSold = sale.amount;
                            int saleInventory = inventoryId;
                            int saleProductId = sale.productId;
                            db.so_route_team_inventory_available
                                .Where(e => e.inventoryId.Equals(saleInventory) && e.productId.Equals(saleProductId)).FirstOrDefault()
                                .Available_Amount += amountSold;
                        }
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return;
                    }

                    List<int> promotionId = promotions.Select(x => x.sale_promotionId).ToList();

                    var promotionDetail = db.so_sale_promotion_detail.Where(s => promotionId.Contains(s.sale_promotionId))
                        .Select(a => new
                        {
                            a.amount,
                            a.productId
                        }).ToList();
                    var sales = saleDetail
                        .Concat(promotionDetail)
                        .GroupBy(a => a.productId)
                        .Select(
                            g => new
                            {
                                productId = g.Key,
                                amount = g.Sum(s => s.amount)
                            });

                    foreach (var sale in sales)
                    {
                        int amountSold = sale.amount;
                        int saleInventory = inventoryId;
                        int saleProductId = sale.productId;
                        db.so_route_team_inventory_available
                            .Where(e => e.inventoryId.Equals(saleInventory) && e.productId.Equals(saleProductId)).FirstOrDefault()
                            .Available_Amount += amountSold;
                    }
                    db.SaveChanges();

                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception();
                }
                dbContextTransaction.Commit();
            }
        }

        public void RestoreInventoryAvailability_v2(int saleId)
        {
            int inventoryId = db.so_sale.Where(s => s.saleId.Equals(saleId)).FirstOrDefault().inventoryId.Value;
            var saleDetail = db.so_sale_detail.Where(s => s.saleId.Equals(saleId))
                .Select(a => new
                {
                    a.amount,
                    a.productId
                }).ToList();

            var promotions = db.so_sale_promotion.Where(s => s.saleId.Equals(saleId));

            if (promotions.Count() == 0)
            {
                foreach (var sale in saleDetail)
                {
                    int amountSold = sale.amount;
                    int saleInventory = inventoryId;
                    int saleProductId = sale.productId;
                    db.so_route_team_inventory_available
                        .Where(e => e.inventoryId.Equals(saleInventory) && e.productId.Equals(saleProductId)).FirstOrDefault()
                        .Available_Amount += amountSold;
                }
                db.SaveChanges();
                return;
            }

            List<int> promotionId = promotions.Select(x => x.sale_promotionId).ToList();

            var promotionDetail = db.so_sale_promotion_detail.Where(s => promotionId.Contains(s.sale_promotionId))
                .Select(a => new
                {
                    a.amount,
                    a.productId
                }).ToList();
            var sales = saleDetail
                .Concat(promotionDetail)
                .GroupBy(a => a.productId)
                .Select(
                    g => new
                    {
                        productId = g.Key,
                        amount = g.Sum(s => s.amount)
                    });

            foreach (var sale in sales)
            {
                int amountSold = sale.amount;
                int saleInventory = inventoryId;
                int saleProductId = sale.productId;
                db.so_route_team_inventory_available
                    .Where(e => e.inventoryId.Equals(saleInventory) && e.productId.Equals(saleProductId)).FirstOrDefault()
                    .Available_Amount += amountSold;
            }
            db.SaveChanges();
        }

        public void RestoreInventoryAvailability_promocion(int saleId)
        {
            int inventoryId = 0;
            int routeId = 0;
            int branchId = 0;

            var sale = db.so_sale.Where(s => s.saleId.Equals(saleId)).FirstOrDefault();

            if(sale != null  && sale.inventoryId.HasValue)
            {
                inventoryId = sale.inventoryId.Value;
            } 
            else
            {
                throw new Exception("No se encontró un inventario");
            }

            var viaje = db.so_route_team_travels_employees.Where(a => a.inventoryId == inventoryId).ToList();

            if (viaje.Count >= 1)
            {
                routeId = viaje.FirstOrDefault().routeId;
            } else
            {
                throw new Exception("Debe existir un viaje para el inventario de la venta");
            }

            var ruta = db.so_route.Where(a => a.routeId == routeId).ToList();

            if (ruta.Count == 1)
            {
                branchId = ruta.FirstOrDefault().branchId;
            }
            else
            {
                throw new Exception("Debe existir un branch configurado para la ruta del viaje donde se realizó venta");
            }


            var amountProduct = (from it in (from promo in db.so_sale_promotion
                                             join detpromo in db.so_sale_promotion_detail
                                              on promo.sale_promotionId equals detpromo.sale_promotionId
                                             where promo.saleId == saleId
                                             select new
                                             {
                                                 detpromo.productId,
                                                 amount = detpromo.amount * promo.amount
                                             }).ToList()
                             .Union((from detsale in db.so_sale_detail
                                     where detsale.saleId == saleId
                                     select new
                                     {
                                         detsale.productId,
                                         detsale.amount
                                     }).ToList()
                              ).ToList()
                                 group it by it.productId
                              into g
                                 select new
                                 {
                                     productId = g.Key,
                                     amount = g.Sum(a => a.amount)
                                 }).ToList();

            //var amountArticle = (from it in (from promo in db.so_sale_promotion
            //                                 join art in db.so_sale_promotion_detail_article
            //                                 on promo.sale_promotionId equals art.sale_promotionId
            //                                 where promo.saleId == saleId
            //                                 select new
            //                                 {
            //                                     art.article_promotionalId,
            //                                     amount = art.amount * promo.amount
            //                                 }).ToList().Union((from art in db.so_sale_detail_article
            //                                                    where art.saleId == saleId
            //                                                    select new
            //                                                    {
            //                                                        art.article_promotionalId,
            //                                                        art.amount
            //                                                    }
            //                                       ).ToList())
            //                     group it by it.article_promotionalId
            //                     into g
            //                     select new
            //                     {
            //                         article_promotionalId = g.Key,
            //                         amount = g.Sum(a => a.amount)
            //                     }).ToList();

            foreach (var product in amountProduct)
            {
                var inventarioProduct = db.so_route_team_inventory_available
                    .Where(e => e.inventoryId.Equals(inventoryId) && e.productId.Equals(product.productId)).FirstOrDefault();

                if (inventarioProduct != null)
                {
                    inventarioProduct.Available_Amount += product.amount;
                }
                else
                {
                    throw new Exception("No se encontró el Producto con ID" + product.productId + " por lo tanto no se pudo incrementar el inventario");
                }
            }

            //foreach (var article in amountArticle)
            //{
            //    var inventarioArt = db.so_article_promotional_route
            //        .Where(e => e.branchId.Equals(branchId) && e.routeId.Equals(routeId) && e.article_promotionalId.Equals(article.article_promotionalId) && e.status == true).FirstOrDefault();

            //    if(inventarioArt != null)
            //    {
            //        inventarioArt.amount += article.amount;
            //        inventarioArt.modifiedon = DateTime.Now;
            //    }
            //    else
            //    {
            //        throw new Exception("No se encontró el Articulo con ID: " + article.article_promotionalId + " por lo tanto no se pudo incrementar el inventario");
            //    }

            //    db.SaveChanges();
            //}
        }

        private ICollection<so_sale_promotion_detail> createPromotionDetails(List<SalePromotionDetailProduct> details, int userId)
        {
            List<so_sale_promotion_detail> promotionDetails = 
                new ListMapper<SalePromotionDetailProduct, so_sale_promotion_detail>
                (new SalePromotionDetailProductMapper())
                .toEntitiesList(details);

            foreach (so_sale_promotion_detail detail in promotionDetails) {

                detail.modifiedon = DateTime.Now;
                detail.createdon = DateTime.Now;
                detail.createdby = userId;
                detail.modifiedby = userId;
                detail.status = true;
            }

            return promotionDetails;
        }

        private ICollection<so_sale_replacement> createReplacements(List<SaleReplacement> saleReplacements, int userId)
        {
            List<so_sale_replacement> replacements =  new ListMapper<SaleReplacement, so_sale_replacement>(new SaleReplacementMapper())
                .toEntitiesList(saleReplacements);


            foreach (so_sale_replacement detail in replacements) {

                detail.createdby = userId;
                detail.modifiedby = userId;
                detail.createdon = DateTime.Now;
                detail.modifiedon = DateTime.Now;
                detail.status = true;
            }

            return replacements;
        }

        private so_sale_inventory createInventorySale(int inventoryId,int userId)
        {
            so_sale_inventory saleInventory = new so_sale_inventory();
            saleInventory.inventoryId = inventoryId;
            saleInventory.createdby = userId;
            saleInventory.modifiedby = userId;
            saleInventory.createdon = DateTime.Now;
            saleInventory.modifiedon = DateTime.Now;
            saleInventory.status = true; 

            return saleInventory;
        }

        private so_delivery_sale createDeliverySale(int? deliveryId, int userId)
        {
            so_delivery_sale deliverySale = new so_delivery_sale();
            deliverySale.deliveryId = deliveryId.Value;
            deliverySale.createdby = userId;
            deliverySale.createdon = DateTime.Now;
            deliverySale.modifiedon = DateTime.Now;
            deliverySale.modifiedby = userId;
            deliverySale.status = true;

            return deliverySale;
        }

        private int SaveSale(so_sale sale)
        {
            int saleId = 0;
            

          using (var dbContextTransaction = db.Database.BeginTransaction())
           {
                try
                {
                    db.so_sale.Add(sale);
                    db.SaveChanges();
                    saleId = sale.saleId;

                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                }

            }
           
            return saleId;
        }

        private int UntransactionalSaveSale(so_sale sale)
        {
            int saleId = 0;
            db.so_sale.Add(sale);
            db.SaveChanges();
            saleId = sale.saleId;
            return saleId;
        }


        private so_control_download createControlDownload(int saleId,int userId) {
            return   new so_control_download() {
                userId = userId,
                modelId = saleId,
                model_type = 1,
                process_type = 0,
                closed = true
            };
        }

        private so_sale createSale(Sale sale) {

            mapper = new SaleMapper();
            so_sale entitySale = mapper.toEntity(sale);

            entitySale.modifiedby = sale.UserId;
            entitySale.createdby = sale.UserId;
            entitySale.modifiedon = DateTime.Now;
            entitySale.createdon = DateTime.Now;
            entitySale.status = true;

            return entitySale;
        }

        private so_sale createSale(SaleTeam sale)
        {

            mapperSaleTeam = new SaleTeamMapper();
            so_sale entitySale = mapperSaleTeam.toEntity(sale);

            entitySale.modifiedby = sale.UserId;
            entitySale.createdby = sale.UserId;
            entitySale.modifiedon = DateTime.Now;
            entitySale.createdon = DateTime.Now;
            entitySale.status = true;

            return entitySale;
        }

        private List<so_sale_detail> createDetails(List<SaleDetail> details, int userId)
        {
            mapperDetails = new SaleDetailMapper();

            listDetailsMapper = new ListMapper<SaleDetail, so_sale_detail>(mapperDetails);

            List<so_sale_detail> entityDetails = listDetailsMapper.toEntitiesList(details);

            foreach (so_sale_detail detail in entityDetails) {
                
                detail.createdby = userId;
                detail.modifiedby = userId;
                detail.createdon = DateTime.Now;
                detail.modifiedon = DateTime.Now;
                detail.status = true;
            }

            return entityDetails;
        }

        public void SetSaleTax(so_sale_detail detail, so_branch_tax branch_tax, so_products_price_list master_price_list, so_products_price_list price_list)
        {
            decimal stps_rate = 0;
            decimal stps_fee_rate = 0;
            decimal stps_snack_rate = 0;
            decimal vat_rate = 0;
            decimal net_content = 0;
            decimal stps_fee_product = 0;

            so_product_tax product_tax = db.so_product_tax.FirstOrDefault(x => x.productId == detail.productId && x.status);

            if (product_tax != null && branch_tax != null)
            {
                stps_rate = product_tax.stps_apply == 1 ? branch_tax.stps : 0;
                stps_fee_rate = product_tax.stps_fee_apply == 1 ? branch_tax.stps_fee : 0;
                stps_snack_rate = product_tax.stps_snack_apply == 1 ? branch_tax.stps_snack : 0;
                vat_rate = product_tax.vat_apply == 1 ? branch_tax.vat : 0;
                net_content = Math.Round(product_tax.pieces * product_tax.trade_volume, 2, MidpointRounding.AwayFromZero);
                stps_fee_product = net_content * stps_fee_rate;
            }

            so_price_list_products_detail price_detail = price_list != null ? price_list.so_price_list_products_detail.FirstOrDefault(x => x.productId == detail.productId && x.status) : null;
            if (price_detail == null && master_price_list != null)
                price_detail = master_price_list.so_price_list_products_detail.FirstOrDefault(x => x.productId == detail.productId && x.status);

            decimal base_price_no_tax = price_detail != null ? (decimal)price_detail.base_price.Value : 0;
            base_price_no_tax = base_price_no_tax / (1 + (vat_rate / 100));               //Quita IVA
            base_price_no_tax = base_price_no_tax - stps_fee_product;                                               //Quita volumen comercial
            base_price_no_tax = base_price_no_tax / (1 + (stps_rate / 100));            //Quita IEPS
            base_price_no_tax = base_price_no_tax / (1 + (stps_snack_rate / 100));        //Quita IEPS Botana
            base_price_no_tax = Math.Round(base_price_no_tax, 2);

            decimal discount_no_tax = 0;
            if (price_detail != null)
                discount_no_tax = (decimal)price_detail.base_price - (decimal)price_detail.price;

            discount_no_tax = discount_no_tax * 100;
            decimal _decimal = discount_no_tax - Math.Floor(discount_no_tax);
            if (_decimal > (decimal)0.5)
                discount_no_tax = discount_no_tax + 1;

            discount_no_tax = Math.Truncate(discount_no_tax);
            discount_no_tax = discount_no_tax / 100;
            discount_no_tax = discount_no_tax / (1 + (vat_rate / 100));
            discount_no_tax = Math.Truncate(discount_no_tax * 100) / 100;
            discount_no_tax = discount_no_tax / (1 + (stps_rate / 100));
            discount_no_tax = Math.Round(discount_no_tax, 2);

            decimal price_no_tax = base_price_no_tax - discount_no_tax;

            decimal stps_product = base_price_no_tax * (stps_rate / 100);
            decimal stps_snack_product = base_price_no_tax * (stps_snack_rate / 100);

            decimal vat_product = Math.Round((price_no_tax + stps_product + stps_snack_product + stps_fee_product) * (vat_rate / 100), 2);
            decimal vat_total = Math.Round(((price_no_tax + stps_product + stps_snack_product + stps_fee_product) * detail.amount) * (vat_rate / 100), 2);

            detail.base_price_no_tax = Math.Truncate(base_price_no_tax * 100) / 100;
            detail.discount_no_tax = Math.Truncate(discount_no_tax * 100) / 100;
            detail.vat = Math.Truncate(vat_product * 100) / 100;
            detail.vat_total = Math.Truncate((vat_total) * 100) / 100;
            detail.stps = Math.Truncate(stps_product * 100) / 100;
            detail.stps_fee = Math.Truncate(stps_fee_product * 100) / 100;
            detail.stps_snack = Math.Truncate(stps_snack_product * 100) / 100;
            detail.net_content = Math.Truncate(net_content * 100) / 100;
            detail.vat_rate = Math.Truncate(vat_rate * 100) / 100;
            detail.stps_rate = Math.Truncate(stps_rate * 100) / 100;
            detail.stps_fee_rate = Math.Truncate(stps_fee_rate * 100) / 100;
            detail.stps_snack_rate = Math.Truncate(stps_snack_rate * 100) / 100;
        }

        public Sale SaleTeamTransaction(Sale sale)
        {
            using (var transaction = db.Database.BeginTransaction()) {
                Sale saleResult = CreateSaleResultFromSale(sale);
                try
                {
                    if (sale.SaleDetails.Count() > 0)
                    {
                        if (!checkIfSaleExist(sale))
                        {
                            //UnlockCreate(sale);
                            //if (sale.SaleId == 0)
                            //{
                            //    throw new BadRequestException();
                            //}
                            saleResult.SaleId = sale.SaleId;
                            UpdateRouteTeamInventory(sale);
                            saleResult.SalePromotions = sale.SalePromotions;

                            UnlockCreate(saleResult);
                            if (saleResult.SaleId == 0)
                            {
                                throw new BadRequestException();
                            }
                        }

                        transaction.Commit();
                    }
                    else
                    {
                        throw new EmptySaleException("La venta no se ha podido realizar porque no hay productos disponibles");
                    }
                }
                catch (EmptySaleException exception)
                {
                    transaction.Rollback();
                    sale.SaleDetails = new List<SaleDetail>();
                    sale.SalePromotions = new List<SalePromotion>();
                    sale.TotalCash = 0.00;
                    return sale;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    throw exception;
                }
                return saleResult;
            }
        }

        public SaleTeam SaleTeamTransaction(SaleTeam sale)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                SaleTeam saleResult = CreateSaleResultFromSale(sale);
                try
                {
                    if (sale.SaleDetails.Count() > 0)
                    {
                        if (!checkIfSaleExist(sale))
                        {
                            UpdateRouteTeamInventory(saleResult, db);
                            UnlockCreate(saleResult);
                            if(!string.IsNullOrEmpty(saleResult.PaymentMethod))
                                CreatePaymentMethod(saleResult);
                            if (saleResult.SaleId == 0)
                                throw new BadRequestException();
                            
                        }

                        transaction.Commit();
                    }
                    else
                    {
                        throw new EmptySaleException();
                    }
                }
                catch (EmptySaleException)
                {
                    transaction.Rollback();
                    sale.SaleDetails = new List<SaleDetail>();
                    sale.SalePromotions = new List<SalePromotion>();
                    sale.TotalCash = 0.00;
                    return sale;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    throw exception;
                }
                return saleResult;
            }
        }

        public void CancelDeliveryStatus(int deliveryId, SmartOrderModel db)
        {
            var statusDelivery = db.so_delivery_status
                    .Where(x => x.Code == DeliveryStatus.CANCELED)
                    .FirstOrDefault();

            var deliveryAD = db.so_delivery_additional_data
                .Where(x => x.deliveryId == deliveryId)
                .FirstOrDefault();

            if (statusDelivery == null)
                throw new EntityNotFoundException("No se puedé actualizar el estado del delivery, no se encontró el status: " + DeliveryStatus.CANCELED);

            //Buscar additional data y crearlo en caso contrario
            if (deliveryAD == null)
            {
                deliveryAD = new so_delivery_additional_data()
                {
                    deliveryId = deliveryId,
                    deliveryStatusId = statusDelivery.deliveryStatusId
                };

                db.so_delivery_additional_data.Add(deliveryAD);
                db.SaveChanges();
            }
            else
            {
                deliveryAD.deliveryStatusId = statusDelivery.deliveryStatusId;
                deliveryAD.modifiedon = DateTime.Now;
                db.so_delivery_additional_data.Attach(deliveryAD);
                db.Entry(deliveryAD).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void UpdateDeliveryStatus(SaleTeam sale, SmartOrderModel db)
        {
            var deliveriesProducts = db.so_delivery_detail
                .Where(x => x.deliveryId == sale.DeliveryId)
                .ToList();

            string status = DeliveryStatus.DELIVERED;
            bool isUndelivered = true;
            foreach (var product in deliveriesProducts)
            {
                var aux = sale.SaleDetails
                    .Where(x => x.ProductId == product.productId)
                    .FirstOrDefault();

                if (aux == null)
                {
                    status = DeliveryStatus.PARTIALLY_DELIVERED;
                    continue;
                }

                if (aux.Amount == 0)
                {
                    status = DeliveryStatus.PARTIALLY_DELIVERED;
                    continue;
                }

                isUndelivered = false;
                if (aux.Amount >= product.amount)
                    continue;

                status = DeliveryStatus.PARTIALLY_DELIVERED;
            }

            if(isUndelivered)
                status = DeliveryStatus.UNDELIVERED;

            var statusDelivery = db.so_delivery_status
                .Where(x => x.Code == status)
                .FirstOrDefault();

            if (statusDelivery == null)
                throw new EntityNotFoundException("No se puedé actualizar el estado del delivery, no se encontró el status: " + status);

            var deliveryAD = db.so_delivery_additional_data
            .Where(x => x.deliveryId == sale.DeliveryId)
            .FirstOrDefault();

            //Buscar additional data y crearlo en caso contrario
            if (deliveryAD == null)
            {
                deliveryAD = new so_delivery_additional_data()
                {
                    deliveryId = sale.DeliveryId,
                    deliveryStatusId = statusDelivery.deliveryStatusId
                };

                db.so_delivery_additional_data.Add(deliveryAD);
                db.SaveChanges();
            }
            else
            {
                deliveryAD.deliveryStatusId = statusDelivery.deliveryStatusId;
                deliveryAD.modifiedon = DateTime.Now;
                db.so_delivery_additional_data.Attach(deliveryAD);
                db.Entry(deliveryAD).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public DataTable GetPromotionsTicketDigital(DbContext db, int SaleId)
        {
            DataTable dt = new DataTable();
            DbDataAdapter adapter;
            DataSet dataset = new DataSet();

            DbCommand command = db.Database.Connection.CreateCommand();
            command.Transaction = db.Database.CurrentTransaction.UnderlyingTransaction;
            command.CommandText = "sp_getPromotionsTicketDigital";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter pSaleId = new SqlParameter("@SaleId", SaleId);
            command.Parameters.Add(pSaleId);

            adapter = new System.Data.SqlClient.SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(dataset);

            return dataset.Tables[0];
        }

        public string GetCancelLinkByCustomerId(int customerId)
        {
            //Ver si al usuario Tiene activado la recepción del ticket
            bool ticketIsActive = db.so_customerr_additional_data
                .Where(x => x.CustomerId == customerId && x.Status == 1).FirstOrDefault() != null ? db.so_customerr_additional_data
                .Where(x => x.CustomerId == customerId && x.Status == 1).FirstOrDefault().IsMailingActive : false;

            so_portal_links_log portalLinkLogs; 
            if(ticketIsActive)
                portalLinkLogs = db.so_portal_links_logs
                    .Where(x => x.CustomerId == customerId && x.Status == (int)PortalLinks.STATUS.PENDING && x.Type == (int)PortalLinks.TYPE.EMAIL_DEACTIVATION)
                    .FirstOrDefault();
            else
                portalLinkLogs = db.so_portal_links_logs
                        .Where(x => x.CustomerId == customerId && x.Status == (int)PortalLinks.STATUS.PENDING || x.Status == (int)PortalLinks.STATUS.ACTIVATED || x.Status == (int)PortalLinks.STATUS.CANCELED && x.Type == (int)PortalLinks.TYPE.EMAIL_DEACTIVATION)
                        .FirstOrDefault();

            if (portalLinkLogs == null)
            {
                //Generar el link para cancelar el envio de correo
                Guid id = Guid.NewGuid();
                var cancelEmail = new so_portal_links_log
                {
                    CustomerId = customerId,
                    CreatedDate = DateTime.Today,
                    Id = id,
                    LimitDays = 0,
                    Status = (int)PortalLinks.STATUS.PENDING,
                    Type = (int)PortalLinks.TYPE.EMAIL_DEACTIVATION
                };

                db.so_portal_links_logs.Add(cancelEmail);
                db.SaveChanges();

                return ConfigurationManager.AppSettings["PortalUrl"] + "Consumer/CancelTicketDigital/" + id;
            }
            
            return ConfigurationManager.AppSettings["PortalUrl"] + "Consumer/CancelTicketDigital/" + portalLinkLogs.Id;
        }        
        
        public void CreatePaymentMethod(SaleTeam sale)
        {
            var findResult = db.so_sale_aditional_data.Where(a => a.saleId == sale.SaleId && a.paymentMethod.Trim() == sale.PaymentMethod).FirstOrDefault();
            if (findResult == null)
            {
                so_sale_aditional_data entitySale = new so_sale_aditional_data();
                entitySale.saleId = sale.SaleId;
                entitySale.paymentMethod = sale.PaymentMethod;
                db.so_sale_aditional_data.Add(entitySale);
                db.SaveChanges();
            }
        }

        public string CreatePromotion(SaleTeam sale, DbContext db)
        {
            List<DataTable> dataTables = createDataTableParameters(sale);
            DataTable dtPromotionCatalog = dataTables[0];
            DataTable dtPromotionProduct = dataTables[1];
            DataTable dtPromotionGiftProduct = dataTables[2];
            DataTable dtPromotionGiftArticle = dataTables[3];
            DataTable dtPromotionSaleArticle = dataTables[4];
            DataTable dtPromotionData = dataTables[5];
            string sRespuesta = "";

            var command = db.Database.Connection.CreateCommand();
            command.Transaction = db.Database.CurrentTransaction.UnderlyingTransaction;
            command.CommandText = "sp_createPromotions";
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter pSaleId = new SqlParameter("@SaleId", sale.SaleId);
            command.Parameters.Add(pSaleId);

            SqlParameter pPromotionCatalog = new SqlParameter("@PromotionCatalog", SqlDbType.Structured);
            pPromotionCatalog.TypeName = "dbo.PromotionCatalog";
            pPromotionCatalog.Value = dtPromotionCatalog;
            command.Parameters.Add(pPromotionCatalog);

            SqlParameter pPromotionProduct = new SqlParameter("@PromotionProduct", SqlDbType.Structured);
            pPromotionProduct.TypeName = "dbo.PromotionProduct";
            pPromotionProduct.Value = dtPromotionProduct;
            command.Parameters.Add(pPromotionProduct);

            SqlParameter pPromotionGiftProduct = new SqlParameter("@PromotionGiftProduct", SqlDbType.Structured);
            pPromotionGiftProduct.TypeName = "dbo.PromotionProduct";
            pPromotionGiftProduct.Value = dtPromotionGiftProduct;
            command.Parameters.Add(pPromotionGiftProduct);

            SqlParameter pPromotionGiftArticle = new SqlParameter("@PromotionGiftArticle", SqlDbType.Structured);
            pPromotionGiftArticle.TypeName = "dbo.PromotionGiftArticle";
            pPromotionGiftArticle.Value = dtPromotionGiftArticle;
            command.Parameters.Add(pPromotionGiftArticle);

            SqlParameter pSaleDetailArticle = new SqlParameter("@SaleDetailArticle", SqlDbType.Structured);
            pSaleDetailArticle.TypeName = "dbo.SaleDetailArticle";
            pSaleDetailArticle.Value = dtPromotionSaleArticle;
            command.Parameters.Add(pSaleDetailArticle);

            SqlParameter pPromotionData = new SqlParameter("@PromotionData", SqlDbType.Structured);
            pPromotionData.TypeName = "dbo.PromocionData";
            pPromotionData.Value = dtPromotionData;
            command.Parameters.Add(pPromotionData);

            SqlParameter pMensaje = new SqlParameter();
            pMensaje.ParameterName = "@Mensaje";
            pMensaje.DbType = DbType.String;
            pMensaje.Direction = ParameterDirection.Output;
            pMensaje.Size = 1000;
            command.Parameters.Add(pMensaje);

            command.ExecuteNonQuery();
            sRespuesta = Convert.ToString(command.Parameters["@Mensaje"].Value);

            return sRespuesta;
        }

        public List<DataTable> createDataTableParameters(SaleTeam sale)
        {
            List<DataTable> dataTables = new List<DataTable>();
            //Crear DataTables a enviar
            DataTable dtPromotionCatalog = new DataTable();
            DataTable dtPromotionProduct = new DataTable();
            DataTable dtPromotionGiftProduct = new DataTable();
            DataTable dtPromotionGiftArticle = new DataTable();
            DataTable dtPromotionData = new DataTable();
            DataTable dtPromotionSaleArticle = new DataTable();

            #region Creación de Cabeceros

            DataColumn column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "promotion_catalogId";
            dtPromotionCatalog.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Decimal");
            column.ColumnName = "additional_cost";
            dtPromotionCatalog.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "amountSale";
            dtPromotionCatalog.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "promotion_catalogId";
            dtPromotionProduct.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "productId";
            dtPromotionProduct.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "amount";
            dtPromotionProduct.Columns.Add(column);

            dtPromotionGiftProduct = dtPromotionProduct.Clone();

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "promotion_catalogId";
            dtPromotionGiftArticle.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "article_promotionalId";
            dtPromotionGiftArticle.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "amount";
            dtPromotionGiftArticle.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "firma";
            dtPromotionData.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "motivoNoFirma";
            dtPromotionData.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Location";
            dtPromotionData.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "article_promotionalId";
            dtPromotionSaleArticle.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "amount";
            dtPromotionSaleArticle.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Decimal");
            column.ColumnName = "import";
            dtPromotionSaleArticle.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Decimal");
            column.ColumnName = "priceValue";
            dtPromotionSaleArticle.Columns.Add(column);

            #endregion

            foreach (var item in sale.PromotionCatalog)
            {
                DataRow row = dtPromotionCatalog.NewRow();
                row["promotion_catalogId"] = item.promotion_catalogId;
                row["additional_cost"] = item.additional_cost;
                row["amountSale"] = item.amountSale;
                dtPromotionCatalog.Rows.Add(row);
            }

            foreach (var item in sale.PromotionProductDto)
            {
                DataRow row = dtPromotionProduct.NewRow();
                row["promotion_catalogId"] = item.promotion_catalogId;
                row["productId"] = item.productId;
                row["amount"] = item.amount;
                dtPromotionProduct.Rows.Add(row);
            }

            foreach (var item in sale.PromotionGiftProductDto)
            {
                DataRow row = dtPromotionGiftProduct.NewRow();
                row["promotion_catalogId"] = item.promotion_catalogId;
                row["productId"] = item.productId;
                row["amount"] = item.amount;
                dtPromotionGiftProduct.Rows.Add(row);
            }

            foreach (var item in sale.PromotionGiftArticleDto)
            {
                DataRow row = dtPromotionGiftArticle.NewRow();
                row["promotion_catalogId"] = item.promotion_catalogId;
                row["article_promotionalId"] = item.article_promotionalId;
                row["amount"] = item.amount;
                dtPromotionGiftArticle.Rows.Add(row);
            }

            foreach (var item in sale.SaleDetailsArticles)
            {
                DataRow row = dtPromotionSaleArticle.NewRow();
                row["article_promotionalId"] = item.article_promotionalId;
                row["amount"] = item.amount;
                row["import"] = item.import;
                row["priceValue"] = item.priceValue;
                dtPromotionSaleArticle.Rows.Add(row);
            }

            DataRow rowData = dtPromotionData.NewRow();
            rowData["firma"] = sale.PromocionData.firma;
            rowData["motivoNoFirma"] = sale.PromocionData.motivoNoFirma;
            rowData["location"] = sale.PromocionData.location;
            dtPromotionData.Rows.Add(rowData);

            dataTables.Add(dtPromotionCatalog);
            dataTables.Add(dtPromotionProduct);
            dataTables.Add(dtPromotionGiftProduct);
            dataTables.Add(dtPromotionGiftArticle);
            dataTables.Add(dtPromotionSaleArticle);
            dataTables.Add(dtPromotionData);

            return dataTables;
        }

        private void SetPromotionTax(so_sale_promotion_detail detail, so_branch_tax branch_tax, so_products_price_list master_price_list, so_products_price_list price_list)
        {
            decimal stps_rate = 0;
            decimal stps_fee_rate = 0;
            decimal stps_snack_rate = 0;
            decimal vat_rate = 0;
            decimal net_content = 0;
            decimal stps_fee_product = 0;

            so_product_tax product_tax = db.so_product_tax.FirstOrDefault(x => x.productId == detail.productId && x.status);

            if (product_tax != null && branch_tax != null)
            {
                stps_rate = product_tax.stps_apply == 1 ? branch_tax.stps : 0;
                stps_fee_rate = product_tax.stps_fee_apply == 1 ? branch_tax.stps_fee : 0;
                stps_snack_rate = product_tax.stps_snack_apply == 1 ? branch_tax.stps_snack : 0;
                vat_rate = product_tax.vat_apply == 1 ? branch_tax.vat : 0;
                net_content = Math.Round(product_tax.pieces * product_tax.trade_volume, 2, MidpointRounding.AwayFromZero);
                stps_fee_product = net_content * stps_fee_rate;
            }

            so_price_list_products_detail price_detail = price_list != null ? price_list.so_price_list_products_detail.FirstOrDefault(x => x.productId == detail.productId && x.status) : null;
            if (price_detail == null && master_price_list != null)
                price_detail = master_price_list.so_price_list_products_detail.FirstOrDefault(x => x.productId == detail.productId && x.status);

            decimal base_price_no_tax = price_detail != null ? (decimal)price_detail.base_price.Value : 0;
            base_price_no_tax = base_price_no_tax / (1 + (vat_rate / 100));               //Quita IVA
            base_price_no_tax = base_price_no_tax - stps_fee_product;                                               //Quita volumen comercial
            base_price_no_tax = base_price_no_tax / (1 + (stps_rate / 100));            //Quita IEPS
            base_price_no_tax = base_price_no_tax / (1 + (stps_snack_rate / 100));        //Quita IEPS Botana
            base_price_no_tax = Math.Round(base_price_no_tax, 2);

            decimal discount_no_tax = 0;
            if (price_detail != null)
                discount_no_tax = (decimal)price_detail.base_price - (decimal)price_detail.price;

            discount_no_tax = discount_no_tax * 100;
            decimal _decimal = discount_no_tax - Math.Floor(discount_no_tax);
            if (_decimal > (decimal)0.5)
                discount_no_tax = discount_no_tax + 1;

            discount_no_tax = Math.Truncate(discount_no_tax);
            discount_no_tax = discount_no_tax / 100;
            discount_no_tax = discount_no_tax / (1 + (vat_rate / 100));
            discount_no_tax = Math.Truncate(discount_no_tax * 100) / 100;
            discount_no_tax = discount_no_tax / (1 + (stps_rate / 100));
            discount_no_tax = Math.Round(discount_no_tax, 2);

            decimal price_no_tax = base_price_no_tax - discount_no_tax;

            decimal stps_product = base_price_no_tax * (stps_rate / 100);
            decimal stps_snack_product = base_price_no_tax * (stps_snack_rate / 100);

            decimal vat_product = Math.Round((price_no_tax + stps_product + stps_snack_product + stps_fee_product) * (vat_rate / 100), 2);
            decimal vat_total = Math.Round(((price_no_tax + stps_product + stps_snack_product + stps_fee_product) * detail.amount) * (vat_rate / 100), 2);

            detail.base_price_no_tax = Math.Truncate(base_price_no_tax * 100) / 100;
            detail.discount_no_tax = Math.Truncate(discount_no_tax * 100) / 100;
            detail.vat = Math.Truncate(vat_product * 100) / 100;
            detail.vat_total = Math.Truncate((vat_total) * 100) / 100;
            detail.stps = Math.Truncate(stps_product * 100) / 100;
            detail.stps_fee = Math.Truncate(stps_fee_product * 100) / 100;
            detail.stps_snack = Math.Truncate(stps_snack_product * 100) / 100;
            detail.net_content = Math.Truncate(net_content * 100) / 100;
            detail.vat_rate = Math.Truncate(vat_rate * 100) / 100;
            detail.stps_rate = Math.Truncate(stps_rate * 100) / 100;
            detail.stps_fee_rate = Math.Truncate(stps_fee_rate * 100) / 100;
            detail.stps_snack_rate = Math.Truncate(stps_snack_rate * 100) / 100;
        }

        public List<SaleDto> GetSaleTeam(int UserId, int InventoryId, int CustomerId)
        {
            DateTime fechaAct = DateTime.Now;
            RouteTeamService routeTeamService = new RouteTeamService();

            so_work_day diaTrabajo = new so_work_day();

            if (routeTeamService.IsImpulsor(UserId))
            {
                diaTrabajo = routeTeamService.GetWorkdayByUserAndDate(UserId, fechaAct);
            } 
            else
            {
                int UserIdImpulsor = routeTeamService.SearchDrivingId(UserId);
                diaTrabajo = routeTeamService.GetWorkdayByUserAndDate(UserIdImpulsor, fechaAct);
            }

            var usuarios = from ruta in (from r in db.so_route_team
                                               select r.routeId)
                                 join r in db.so_route_team
                                   on ruta equals r.routeId
                                 join rd in db.so_route_team_travels_employees.Where(a => a.work_dayId == diaTrabajo.work_dayId)
                                    on new { r.routeId, r.userId } equals new { rd.routeId, rd.userId }
                                 group rd by rd.userId into g
                                 select new { userId = g.Key };


            var filtroRouteTeam = db.so_route_team_travels_employees.Where(a => a.work_dayId == diaTrabajo.work_dayId);

            var inventarios = from item in (from rdt in filtroRouteTeam
                                            group rdt by rdt.inventoryId into g
                                            select new { inventoryId = g.Key })
                              select item.inventoryId;

            IQueryable<so_sale> filtroSale;

            if (CustomerId != 0)
            {
                filtroSale = db.so_sale.Where(a => a.customerId == CustomerId);
            }
            else
            {
                filtroSale = db.so_sale.Where(x => x.status == true || x.status == false);
            }

            var qsaleDto = (from venta in filtroSale
                           join user in usuarios
                                on new { a = venta.userId } equals new { a = user.userId }
                           join inv in inventarios
                                on new { inventoryId = (venta.inventoryId.HasValue ? venta.inventoryId.Value : 0) } equals new { inventoryId = inv }
                            join ad in db.so_sale_aditional_data on venta.saleId equals ad.saleId into ventaSaleAD
                            from result in ventaSaleAD.DefaultIfEmpty()
                            orderby venta.createdon descending
                            select new
                            {
                                venta.userId,
                                venta.total_cash,
                                venta.saleId,
                                venta.total_credit,
                                venta.tag,
                                venta.inventoryId,
                                venta.date,
                                venta.customerId,
                                venta.deliveryId,
                                venta.so_sale_detail,
                                venta.so_sale_replacement,
                                venta.so_sale_promotion,
                                result.paymentMethod,
                                venta.createdon,
                                venta.state,
                                venta.status
                            }).ToList();

            var saleDto = (from item in qsaleDto
                           where item.date.Date == fechaAct.Date
                           select new SaleDto 
                                        {
                                         UserId = item.userId,
                                         TotalCash = item.total_cash,
                                         SaleId = item.saleId,
                                         TotalCredit = item.total_credit,
                                         CustomerTag = item.tag ?? "",
                                         InventoryId = item.inventoryId ?? 0,
                                         Date = item.date.ToString("dd/MM/yyyy HH:m:s"),
                                         CustomerId = item.customerId,
                                         DeliveryId = item.deliveryId ?? 0,
                                         PaymentMethod = item.paymentMethod,
                                         CreateDate = item.createdon.HasValue ? item.createdon.Value.ToString("dd/MM/yyyy HH:m") : "",
                                         State = item.state,
                                         Status = item.status,
                                         SaleDetails = (from g in item.so_sale_detail
                                                        orderby g.createdon descending
                                                        select new SaleDetailResponse 
                                                        {
                                                         Price = g.price,
                                                         Amount = g.amount,
                                                         CreditAmount = g.credit_amount,
                                                         Import = g.import,
                                                         ProductId = g.productId,
                                                         AmountSold = g.amount
                                                        }).ToList(),
                                         SaleReplacements = (from g in item.so_sale_replacement
                                                             orderby g.createdon descending
                                                                select new SaleReplacement 
                                                             {
                                                                 ReplacementId = g.replacementId,
                                                                 Amount = g.amount
                                                             }).ToList(),
                                         SalePromotion = (from g in item.so_sale_promotion
                                                          where g.status == true
                                                          orderby g.createdon descending
                                                         select new SalePromotionResponse 
                                                         {
                                                             PromotionId = g.promotionId,
                                                             Amount = g.amount,
                                                             DetailProduct = (from gg in g.so_sale_promotion_detail
                                                                              where gg.status == true
                                                                              orderby gg.createdon descending
                                                                             select new SalePromotionDetailProductResponse 
                                                                             {
                                                                                 ProductId = gg.productId,
                                                                                 Amount = gg.amount,
                                                                                 PriceValue = gg.price_without_taxes ?? 0,
                                                                                 Import = gg.import
                                                                             }).ToList()
                                                         }).ToList()
                                        }).ToList();

            if(InventoryId != 0){
                saleDto = saleDto.Where(x => x.InventoryId != InventoryId || x.UserId != UserId).ToList();
            }
            
            return saleDto;
        }

        public ResponseBase<MsgResponseBase> SenTicketDigital(SendTicketDigitalRequest request)
        {
            if(request.SaleId == 0)
                return ResponseBase<MsgResponseBase>.Create(new List<string>()
                        {
                            "No se puede enviar el email", "No cuenta con datos suficientes"
                        });

            using (var transaction = db.Database.BeginTransaction())
            {
                var sale = db.so_sale.Where(s => s.saleId.Equals(request.SaleId)).FirstOrDefault();

                if (sale == null)
                    throw new EntityNotFoundException("No se encontró la venta");

                var customer = db.so_customer.Where(x => x.customerId == sale.customerId).FirstOrDefault();

                if (request.Email == null)
                {
                    if (customer.CustomerAdditionalData == null)
                        return ResponseBase<MsgResponseBase>.Create(new List<string>()
                        {
                            "No se puede enviar el email", "No cuenta con datos suficientes (Datos adicionales.)"
                        });

                    if (customer.CustomerAdditionalData.Count() == 0)
                        return ResponseBase<MsgResponseBase>.Create(new List<string>()
                        {
                            "No se puede enviar el email", "No cuenta con datos suficientes (Datos adicionales.)"
                        });

                    if (!customer.CustomerAdditionalData.FirstOrDefault().IsMailingActive)
                        return ResponseBase<MsgResponseBase>.Create(new List<string>()
                        {
                            "No se puede enviar el email", "El cliente no tiene activado el envio de Emails"
                        });

                    request.Email = customer.email;
                }

                if (sale.state == 2)
                {
                    var saleAD = db.so_sale_aditional_data.Where(x => x.saleId == sale.saleId).FirstOrDefault();
                    string PaymentMethod = saleAD == null ? null : saleAD.paymentMethod;
                    //Se prepara la información
                    var route = db.so_route_customer.Where(x => x.customerId == sale.customerId).Select(x => x.so_route.code).FirstOrDefault();
                    var user = db.so_user.Where(x => x.userId == sale.userId).FirstOrDefault();
                    DataTable dtTicket = GetPromotionsTicketDigital(db, sale.saleId);

                    var sendTicketDigitalEmail = new SendCancelTicketDigitalEmailRequest
                    {
                        CustomerName = customer.name,
                        RouteAddress = route,
                        CustomerEmail = request.Email,
                        CustomerFullName = customer.customerId + " - " + customer.name + " " + customer.address,
                        Date = sale.date,
                        PaymentMethod = PaymentMethod,
                        SellerName = user.code + " - " + user.name,
                        dtTicket = dtTicket,
                        ReferenceCode = customer.customerId.ToString()
                    };

                    //Preparar Order
                    List<so_delivery_detail> delivery = null;
                    if (sale.deliveryId == 0)
                        sendTicketDigitalEmail.Order = null;
                    else
                    {
                        delivery = db.so_delivery_detail
                            .Where(x => x.deliveryId == sale.deliveryId)
                            .ToList();

                        sendTicketDigitalEmail.Order = new SendCancelTicketDigitalEmailOrder()
                        {
                            OrderDetail = new List<SendCancelTicketDigitalEmailOrderDetail>(),
                            DeliveryDate = sale.date
                        };
                    }

                    var sales = new List<SendTicketDigitalEmailSales>();
                    foreach (var detail in sale.so_sale_detail)
                    {
                        var product = db.so_product.Where(x => x.productId == detail.productId).FirstOrDefault();
                        if (product == null)
                            continue;

                        if (sendTicketDigitalEmail.Order != null)
                        {
                            var productOrder = delivery.Where(x => x.productId == detail.productId).FirstOrDefault();
                            //Si el producto esta dentro de la preventa
                            if (productOrder != null)
                            {
                                //Verificar si la cantidad es menor o igual a la preventa
                                if (detail.amount <= productOrder.amount)
                                {
                                    //Si lo que se esta vendiendo es menor o igual a lo solicitado Agregar en Order y pasar al siguiente
                                    sendTicketDigitalEmail.Order.OrderDetail.Add(new SendCancelTicketDigitalEmailOrderDetail
                                    {
                                        Amount = detail.amount, //Se usa el detail porque el amount puede ser menor
                                        ProductName = product.code + " - " + product.name,
                                        //TotalPrice = (double)detail.amount * productOrder.price.Value,
                                        //UnitPrice = productOrder.price.Value,
                                        TotalPrice = (double)detail.amount * detail.price,
                                        UnitPrice = detail.price
                                    });
                                }
                                else
                                {
                                    //Si es mayor hacer la resta y agregar a sale y preventa
                                    sendTicketDigitalEmail.Order.OrderDetail.Add(new SendCancelTicketDigitalEmailOrderDetail
                                    {
                                        Amount = productOrder.amount,
                                        ProductName = product.code + " - " + product.name,
                                        //TotalPrice = (double)productOrder.amount * productOrder.price.Value,
                                        //UnitPrice = productOrder.price.Value,
                                        TotalPrice = (double)productOrder.amount * detail.price,
                                        UnitPrice = detail.price
                                    });

                                    sales.Add(new SendTicketDigitalEmailSales
                                    {
                                        Amount = detail.amount - productOrder.amount,
                                        ProductName = product.code + " - " + product.name,
                                        TotalPrice = Convert.ToDouble(detail.amount - productOrder.amount) * Convert.ToDouble(detail.price),
                                        UnitPrice = Convert.ToDouble(detail.price)
                                    });

                                }
                                continue;
                            }
                        }

                        sales.Add(new SendTicketDigitalEmailSales
                        {
                            Amount = detail.amount,
                            ProductName = product.code + " - " + product.name,
                            TotalPrice = Convert.ToDouble(detail.amount) * Convert.ToDouble(detail.price),
                            UnitPrice = Convert.ToDouble(detail.price)
                        });
                    }
                    sendTicketDigitalEmail.Sales = sales;

                    //Se envia el ticket
                    var emailService = new EmailService();
                    var response = emailService.SendCancelTicketDigitalEmail(sendTicketDigitalEmail);
                    if (!response.Status)
                        return ResponseBase<MsgResponseBase>.Create(new List<string>(response.Errors));
                }
                else
                {
                    var saleAD = db.so_sale_aditional_data.Where(x => x.saleId == sale.saleId).FirstOrDefault();
                    string PaymentMethod = saleAD == null ? null : saleAD.paymentMethod;
                    //Se prepara la información
                    var route = db.so_route_customer.Where(x => x.customerId == sale.customerId).Select(x => x.so_route.code).FirstOrDefault();
                    var user = db.so_user.Where(x => x.userId == sale.userId).FirstOrDefault();
                    DataTable dtTicket = GetPromotionsTicketDigital(db, sale.saleId);

                    var sendTicketDigitalEmail = new SendTicketDigitalEmailRequest
                    {
                        CustomerName = customer.name,
                        RouteAddress = route,
                        CustomerEmail = request.Email,
                        CustomerFullName = customer.customerId + " - " + customer.name + " " + customer.address,
                        Date = sale.date,
                        PaymentMethod = PaymentMethod,
                        SellerName = user.code + " - " + user.name,
                        dtTicket = dtTicket,
                        ReferenceCode = customer.customerId.ToString()
                    };

                    //Preparar Order
                    List<so_delivery_detail> delivery = null;
                    if (sale.deliveryId == 0)
                        sendTicketDigitalEmail.Order = null;
                    else
                    {
                        delivery = db.so_delivery_detail
                            .Where(x => x.deliveryId == sale.deliveryId)
                            .ToList();

                        sendTicketDigitalEmail.Order = new SendTicketDigitalEmailOrder()
                        {
                            OrderDetail = new List<SendTicketDigitalEmailOrderDetail>(),
                            DeliveryDate = sale.date
                        };
                    }

                    var sales = new List<SendTicketDigitalEmailSales>();
                    foreach (var detail in sale.so_sale_detail)
                    {
                        var product = db.so_product.Where(x => x.productId == detail.productId).FirstOrDefault();
                        if (product == null)
                            continue;

                        if (sendTicketDigitalEmail.Order != null)
                        {
                            var productOrder = delivery.Where(x => x.productId == detail.productId).FirstOrDefault();
                            //Si el producto esta dentro de la preventa
                            if (productOrder != null)
                            {
                                //Verificar si la cantidad es menor o igual a la preventa
                                if (detail.amount <= productOrder.amount)
                                {
                                    //Si lo que se esta vendiendo es menor o igual a lo solicitado Agregar en Order y pasar al siguiente
                                    sendTicketDigitalEmail.Order.OrderDetail.Add(new SendTicketDigitalEmailOrderDetail
                                    {
                                        Amount = detail.amount, //Se usa el detail porque el amount puede ser menor
                                        ProductName = product.code + " - " + product.name,
                                        //TotalPrice = (double)detail.amount * productOrder.price.Value,
                                        //UnitPrice = productOrder.price.Value,
                                        TotalPrice = (double)detail.amount * detail.price,
                                        UnitPrice = detail.price
                                    });
                                }
                                else
                                {
                                    //Si es mayor hacer la resta y agregar a sale y preventa
                                    sendTicketDigitalEmail.Order.OrderDetail.Add(new SendTicketDigitalEmailOrderDetail
                                    {
                                        Amount = productOrder.amount,
                                        ProductName = product.code + " - " + product.name,
                                        //TotalPrice = (double)productOrder.amount * productOrder.price.Value,
                                        //UnitPrice = productOrder.price.Value,
                                        TotalPrice = (double)productOrder.amount * detail.price,
                                        UnitPrice = detail.price
                                    });

                                    sales.Add(new SendTicketDigitalEmailSales
                                    {
                                        Amount = detail.amount - productOrder.amount,
                                        ProductName = product.code + " - " + product.name,
                                        TotalPrice = Convert.ToDouble(detail.amount - productOrder.amount) * Convert.ToDouble(detail.price),
                                        UnitPrice = Convert.ToDouble(detail.price)
                                    });

                                }
                                continue;
                            }
                        }

                        sales.Add(new SendTicketDigitalEmailSales
                        {
                            Amount = detail.amount,
                            ProductName = product.code + " - " + product.name,
                            TotalPrice = Convert.ToDouble(detail.amount) * Convert.ToDouble(detail.price),
                            UnitPrice = Convert.ToDouble(detail.price)
                        });
                    }

                    sendTicketDigitalEmail.CancelTicketLink = GetCancelLinkByCustomerId(customer.customerId);
                    sendTicketDigitalEmail.Sales = sales;

                    //Se envia el ticket
                    var emailService = new EmailService();
                    var response = emailService.SendTicketDigitalEmail(sendTicketDigitalEmail);
                    if (!response.Status)
                        return ResponseBase<MsgResponseBase>.Create(new List<string>(response.Errors));

                    transaction.Commit();
                }

                return ResponseBase<MsgResponseBase>.Create(new MsgResponseBase()
                {
                    Msg = "Se ha enviadó con éxito"
                });
            }
        }
    }
}