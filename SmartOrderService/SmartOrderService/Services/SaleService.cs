﻿using OpeCDLib.Models;
using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Mappers;
using SmartOrderService.Models;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Enum;
using SmartOrderService.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data;
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


        public Sale Delete(int saleId) {

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



            var SaleDto =  new SaleMapper().toModel(sale);
            
            return SaleDto;

        }

        public Sale Cancel(int saleId, string PaymentMethod)
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
                    //Enviar Ticket
                    try
                    {
                        var customer = db.so_customer.Where(x => x.customerId == sale.customerId).FirstOrDefault();

                        if (customer.CustomerAdditionalData != null)
                        {
                            if (customer.CustomerAdditionalData.FirstOrDefault().IsMailingActive)
                            {
                                //Se prepara la información
                                var route = db.so_route_customer.Where(x => x.customerId == sale.customerId).FirstOrDefault();
                                var user = db.so_user.Where(x => x.userId == sale.userId).FirstOrDefault();

                                var sendTicketDigitalEmail = new SendCancelTicketDigitalEmailRequest
                                {
                                    CustomerName = customer.name,
                                    RouteAddress = Convert.ToString(route.routeId),
                                    CustomerEmail = customer.email,
                                    CustomerFullName = customer.customerId + " - " + customer.name + " " + customer.address,
                                    Date = DateTime.Now,
                                    PaymentMethod = PaymentMethod,
                                    SellerName = user.code + " - " + user.name
                                };

                                var sales = new List<SendTicketDigitalEmailSales>();
                                foreach (var detail in sale.so_sale_detail)
                                {
                                    var product = db.so_product.Where(x => x.productId == detail.productId).FirstOrDefault();
                                    if (product == null)
                                        continue;

                                    sales.Add(new SendTicketDigitalEmailSales
                                    {
                                        Amount = detail.amount,
                                        ProductName = detail.productId + " - " + product.name,
                                        TotalPrice = Convert.ToDouble(detail.amount) * Convert.ToDouble(detail.base_price_no_tax),
                                        UnitPrice = Convert.ToDouble(detail.base_price_no_tax)
                                    });
                                }
                                sendTicketDigitalEmail.Sales = sales;

                                //Se envia el ticket
                                var emailService = new EmailService();
                                var response = emailService.SendCancelTicketDigitalEmail(sendTicketDigitalEmail);
                            }
                        }
                    }
                    catch (Exception)
                    {
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
                //entitySale.so_sale_promotion = createPromotions(sale.SalePromotions, userId);
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
                    }
                    else
                    {
                        sale.TotalCash -= Decimal.ToDouble(sale.SaleDetails[i].Import);
                        sale.SaleDetails.RemoveAt(i);
                        i--;
                    }
                    saleDetailResult.AmountSold = amountSaled;
                    saleResult.SaleDetails.Add(saleDetailResult);
                }
                saleResult.TotalCash = Math.Round(sale.TotalCash, 3);
                saleResult.SaleReplacements = sale.SaleReplacements;
                saleResult.SalePromotions = sale.SalePromotions;
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
            saleResult.EmailDeliveryTicket = sale.EmailDeliveryTicket;
            saleResult.SmsDeliveryTicket = sale.SmsDeliveryTicket;
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
                }
                else
                {
                    sale.TotalCash -= Decimal.ToDouble(sale.SaleDetails[i].Import);
                    sale.SaleDetails.RemoveAt(i);
                    i--;
                }
                saleDetailResult.AmountSold = amountSaled;
                saleResult.SaleDetails.Add(saleDetailResult);
            }
            saleResult.TotalCash = Math.Round(sale.TotalCash, 3);
            saleResult.SaleReplacements = sale.SaleReplacements;
            //saleResult.SalePromotions = sale.SalePromotions;
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
            
            /*
            foreach (so_sale_promotion p in entitySale.so_sale_promotion)
                foreach (so_sale_promotion_detail pd in p.so_sale_promotion_detail)
                    SetPromotionTax(pd, branch_tax, master_price_list, price_list);*/
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
                    modifiedon = DateTime.Now
                    
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

        public void UpdateRouteTeamInventory(SaleTeam sale)
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

        public void RestoreInventoryAvailability(int saleId)
        {
            int inventoryId = db.so_sale.Where(s => s.saleId.Equals(saleId)).FirstOrDefault().inventoryId.Value;
            var saleDetail = db.so_sale_detail.Where(s => s.saleId.Equals(saleId))
                .Select(a => new
                {
                    a.amount,
                    a.productId
                }).ToList();

            var promotion = db.so_sale_promotion.Where(s => s.saleId.Equals(saleId)).FirstOrDefault();

            if (promotion == null)
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

            int promotionId = promotion.sale_promotionId;

            var promotionDetail = db.so_sale_promotion_detail.Where(s => s.sale_promotionId.Equals(promotionId))
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

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
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
                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception();
                }
            }
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
                            UnlockCreate(sale);
                            if (sale.SaleId == 0)
                            {
                                throw new BadRequestException();
                            }
                            saleResult.SaleId = sale.SaleId;
                            UpdateRouteTeamInventory(sale);
                        }

                        transaction.Commit();
                    }
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
                string sRespuesta = "";
                try
                {
                    if (sale.SaleDetails.Count() > 0)
                    {
                        if (!checkIfSaleExist(sale))
                        {
                            UnlockCreate(sale);
                            if (sale.SaleId == 0)
                            {
                                throw new BadRequestException();
                            }
                            saleResult.SaleId = sale.SaleId;
                            UpdateRouteTeamInventory(sale);
                            CreatePaymentMethod(sale);
                            sRespuesta = CreatePromotion(sale, db, transaction);
                            if (sRespuesta != string.Empty)
                                throw new Exception(sRespuesta);
                        }

                        var updateCustomerAdditionalData = db.so_customerr_additional_data
                            .Where(x => x.CustomerId == sale.CustomerId)
                            .FirstOrDefault();

                        if (updateCustomerAdditionalData != null)
                        {
                            #region Consumidores logica
                            //Actualizar contador
                            updateCustomerAdditionalData.CounterVisitsWithoutSales = 0;
                            db.SaveChanges();

                            //Envio de Ticket
                            if (sale.EmailDeliveryTicket == true)
                            {
                                var customer = db.so_customer.Where(x => x.customerId == sale.CustomerId).FirstOrDefault();

                                if (customer.CustomerAdditionalData != null)
                                {
                                    var customerAux = customer.CustomerAdditionalData.FirstOrDefault();
                                    if (customerAux.IsMailingActive && customerAux.AcceptedTermsAndConditions)
                                    {
                                        //Se prepara la información
                                        var route = db.so_route_customer.Where(x => x.customerId == sale.CustomerId).FirstOrDefault();
                                        var user = db.so_user.Where(x => x.userId == sale.UserId).FirstOrDefault();

                                        var sendTicketDigitalEmail = new SendTicketDigitalEmailRequest
                                        {
                                            CustomerName = customer.name,
                                            RouteAddress = Convert.ToString(route.routeId),
                                            CustomerEmail = customer.email,
                                            CustomerFullName = customer.customerId + " - " + customer.name + " " + customer.address,
                                            Date = DateTime.Now,
                                            PaymentMethod = sale.PaymentMethod,
                                            SellerName = user.code + " - " + user.name
                                        };

                                        var sales = new List<SendTicketDigitalEmailSales>();
                                        foreach (var detail in saleResult.SaleDetails)
                                        {
                                            var product = db.so_product.Where(x => x.productId == detail.ProductId).FirstOrDefault();
                                            if (product == null)
                                                continue;

                                            sales.Add(new SendTicketDigitalEmailSales
                                            {
                                                Amount = detail.Amount,
                                                ProductName = detail.ProductId + " - " + product.name,
                                                TotalPrice = Convert.ToDouble(detail.Amount) * Convert.ToDouble(detail.PriceValue),
                                                UnitPrice = Convert.ToDouble(detail.PriceValue)
                                            });
                                        }
                                        sendTicketDigitalEmail.Sales = sales;

                                        //Se envia el ticket
                                        if (sales != null)
                                        {
                                            var emailService = new EmailService();
                                            var response = emailService.SendTicketDigitalEmail(sendTicketDigitalEmail);
                                        }
                                    }
                                }
                            }

                            #endregion
                        }

                        transaction.Commit();
                    }
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    throw exception;
                }
                return saleResult;
            }

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

        public string CreatePromotion(SaleTeam sale, DbContext db, DbContextTransaction transaction)
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

            SqlParameter pRouteId = new SqlParameter("@RouteId", sale.RouteId);
            command.Parameters.Add(pRouteId);

            SqlParameter pBranchId = new SqlParameter("@BranchId", sale.BranchId);
            command.Parameters.Add(pBranchId);

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

            var filtroSale = db.so_sale.Where(a => a.status == true);

            if (CustomerId != 0)
            {
                filtroSale = db.so_sale.Where(a => a.customerId == CustomerId && a.status == true);
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
                                venta.createdon
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
    }
}