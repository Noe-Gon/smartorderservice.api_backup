using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Mappers;
using SmartOrderService.Models;
using SmartOrderService.Models.Enum;
using SmartOrderService.Models.Message;
using SmartOrderService.Models.Requests;
using SmartOrderService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class SaleTeamService : IDisposable
    {
        public static SaleTeamService Create() => new SaleTeamService();

        private UoWConsumer UoWConsumer { get; set; }

        public SaleTeamService()
        {
            UoWConsumer = new UoWConsumer();

        }

        #region Utils

        private IMapper<SaleTeamTransactionMessage, so_sale> mapperSaleTeam;
        private IMapper<SaleDetail, so_sale_detail> mapperDetails;
        private ListMapper<SaleDetail, so_sale_detail> listDetailsMapper;

        #endregion

        public SaleTeamTransactionMessage SaleTeamTransaction(SaleTeamTransactionMessage sale)
        {
            using (var uoWConsumer = new UoWConsumer())
            {
                this.UoWConsumer = uoWConsumer;
                SaleTeamTransactionMessage saleResult = CreateSaleResultFromSale(sale);
                string sRespuesta = "";
                try
                {
                    if (sale.SaleDetails.Count() > 0)
                    {
                        if (!checkIfSaleTeamExist(sale))
                        {
                            UnlockCreate(sale);
                            if (sale.SaleId == 0)
                            {
                                throw new BadRequestException();
                            }
                            saleResult.SaleId = sale.SaleId;
                            ValidUpdateRouteTeamInventory(sale);
                            CreatePaymentMethod(sale);


                            sRespuesta = CreatePromotion(sale);
                            if (sRespuesta != string.Empty)
                                throw new Exception(sRespuesta);


                            var updateCustomerAdditionalData = UoWConsumer.CustomerAdditionalDataRepository
                                .Get(x => x.CustomerId == sale.CustomerId)
                                .FirstOrDefault();

                            //Actualizar contador
                            if (updateCustomerAdditionalData != null)
                            {
                                updateCustomerAdditionalData.CounterVisitsWithoutSales = 0;
                                UoWConsumer.CustomerAdditionalDataRepository.Update(updateCustomerAdditionalData);
                                UoWConsumer.Save();
                            }

                            SendTicketDigital(updateCustomerAdditionalData, sale, saleResult);
                        }

                        if (sale.DeliveryId != 0)
                            UpdateDeliveryStatus(sale);

                        UoWConsumer.Save();
                    }
                }
                catch (ApiPreventaException e)
                {
                    throw e;
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return saleResult;
            }
        }

        #region Helpers

        private SaleTeamTransactionMessage CreateSaleResultFromSale(SaleTeamTransactionMessage sale)
        {
            RoleTeamService roleTeamService = new RoleTeamService();
            ERolTeam userRole = roleTeamService.GetUserRole(sale.UserId);
            if (userRole == ERolTeam.SinAsignar)
            {
                return sale;
            }

            SaleTeamTransactionMessage saleResult = new SaleTeamTransactionMessage();
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
            saleResult.PaymentMethod = sale.PaymentMethod;
            for (int i = 0; i < sale.SaleDetails.Count(); i++)
            {
                int amountSaled = 0;
                SaleDetailResult saleDetailResult = new SaleDetailResult(sale.SaleDetails[i]);

                if (UoWConsumer.CheckInventoryAvailability(sale.InventoryId, sale.SaleDetails[i].ProductId, sale.SaleDetails[i].Amount))
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

        private bool checkIfSaleTeamExist(SaleTeamTransactionMessage sale)
        {
            int userId = sale.UserId;
            DateTime date = DateTime.Parse(sale.Date);
            int customerId = sale.CustomerId;

            int deliveryId = sale.DeliveryId;

            var registeredSale = UoWConsumer.SaleRepository.
                 Get(
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

        public SaleTeamTransactionMessage UnlockCreate(SaleTeamTransactionMessage sale)
        {
            int userId = sale.UserId;
            DateTime date = DateTime.Parse(sale.Date);
            int customerId = sale.CustomerId;
            int deliveryId = sale.DeliveryId;
            var registeredSale = UoWConsumer.SaleRepository.
                     Get(
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
                UoWConsumer.SaleRepository.Insert(entitySale);
                UoWConsumer.Save();
                sale.SaleId = entitySale.saleId;

            }
            else
            {
                sale.SaleId = registeredSale.saleId;
            }

            return sale;
        }

        private so_sale createSale(SaleTeamTransactionMessage sale)
        {

            mapperSaleTeam = new SaleTeamTransactionMapper();
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

            foreach (so_sale_detail detail in entityDetails)
            {

                detail.createdby = userId;
                detail.modifiedby = userId;
                detail.createdon = DateTime.Now;
                detail.modifiedon = DateTime.Now;
                detail.status = true;
            }

            return entityDetails;
        }

        private ICollection<so_sale_replacement> createReplacements(List<SaleReplacement> saleReplacements, int userId)
        {
            List<so_sale_replacement> replacements = new ListMapper<SaleReplacement, so_sale_replacement>(new SaleReplacementMapper())
                .toEntitiesList(saleReplacements);


            foreach (so_sale_replacement detail in replacements)
            {

                detail.createdby = userId;
                detail.modifiedby = userId;
                detail.createdon = DateTime.Now;
                detail.modifiedon = DateTime.Now;
                detail.status = true;
            }

            return replacements;
        }

        private ICollection<so_sale_promotion> createPromotions(List<SalePromotion> salePromotions, int userId)
        {
            List<so_sale_promotion> promotions = new List<so_sale_promotion>();

            if (salePromotions != null)
                foreach (SalePromotion promotion in salePromotions)
                {
                    so_sale_promotion entityPromotion = new so_sale_promotion()
                    {

                        promotionId = promotion.PromotionId,
                        amount = promotion.Amount,
                        status = true,
                        so_sale_promotion_detail = createPromotionDetails(promotion.DetailProduct, userId),
                        modifiedby = userId,
                        createdby = userId,
                        createdon = DateTime.Now,
                        modifiedon = DateTime.Now

                    };

                    promotions.Add(entityPromotion);
                }

            return promotions;
        }

        private ICollection<so_sale_promotion_detail> createPromotionDetails(List<SalePromotionDetailProduct> details, int userId)
        {
            List<so_sale_promotion_detail> promotionDetails =
                new ListMapper<SalePromotionDetailProduct, so_sale_promotion_detail>
                (new SalePromotionDetailProductMapper())
                .toEntitiesList(details);

            foreach (so_sale_promotion_detail detail in promotionDetails)
            {

                detail.modifiedon = DateTime.Now;
                detail.createdon = DateTime.Now;
                detail.createdby = userId;
                detail.modifiedby = userId;
                detail.status = true;
            }

            return promotionDetails;
        }

        private void SetTaxes(so_sale entitySale)
        {
            so_user user = UoWConsumer.UserRepository.Get(x => x.userId == entitySale.userId).FirstOrDefault();
            so_branch branch = user != null ? user.so_branch : null;
            int branchId = branch != null ? branch.branchId : 0;

            so_branch_tax branch_tax = UoWConsumer.BranchTaxRepository.Get(x => x.branchId == branchId && x.status).FirstOrDefault();
            so_products_price_list master_price_list = UoWConsumer.ProductPriceListRepository.Get(x => x.is_master && x.branchId == branchId && x.status).FirstOrDefault();
            so_products_price_list price_list = UoWConsumer.ProductPriceListRepository.GetAll().
                                                Join(UoWConsumer.CustomerProductPriceListRepository.GetAll(),
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

        private void SetSaleTax(so_sale_detail detail, so_branch_tax branch_tax, so_products_price_list master_price_list, so_products_price_list price_list)
        {
            decimal stps_rate = 0;
            decimal stps_fee_rate = 0;
            decimal stps_snack_rate = 0;
            decimal vat_rate = 0;
            decimal net_content = 0;
            decimal stps_fee_product = 0;

            so_product_tax product_tax = UoWConsumer.ProductTaxRepository.Get(x => x.productId == detail.productId && x.status).FirstOrDefault();

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

        private void SetPromotionTax(so_sale_promotion_detail detail, so_branch_tax branch_tax, so_products_price_list master_price_list, so_products_price_list price_list)
        {
            decimal stps_rate = 0;
            decimal stps_fee_rate = 0;
            decimal stps_snack_rate = 0;
            decimal vat_rate = 0;
            decimal net_content = 0;
            decimal stps_fee_product = 0;

            so_product_tax product_tax = UoWConsumer.ProductTaxRepository.Get(x => x.productId == detail.productId && x.status).FirstOrDefault();

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

        private void UpdateDeliveryStatus(SaleTeamTransactionMessage sale)
        {
            var deliveriesProducts = UoWConsumer.DeliveryDetailRepository
                .Get(x => x.deliveryId == sale.DeliveryId)
                .ToList();

            string status = DeliveryStatus.DELIVERED;
            bool isUndelivered = true;
            foreach (var product in deliveriesProducts)
            {
                var aux = sale.SaleDetails
                    .Where(x => x.ProductId == product.productId)
                    .FirstOrDefault();

                if (aux == null)
                    continue;

                if (aux.Amount == 0)
                    continue;

                isUndelivered = false;
                if (aux.Amount >= product.amount)
                    continue;

                status = DeliveryStatus.PARTIALLY_DELIVERED;
            }

            if (isUndelivered)
                status = DeliveryStatus.UNDELIVERED;

            var statusDelivery = UoWConsumer.DeliveryStatusRepository
                .Get(x => x.Code == status)
                .FirstOrDefault();

            if (statusDelivery == null)
                throw new EntityNotFoundException("No se puedé actualizar el estado del delivery, no se encontró el status: " + status);

            var deliveryAD = UoWConsumer.DeliveryAdditionalData
            .Get(x => x.deliveryId == sale.DeliveryId)
            .FirstOrDefault();

            //Buscar additional data y crearlo en caso contrario
            if (deliveryAD == null)
            {
                deliveryAD = new so_delivery_additional_data()
                {
                    deliveryId = sale.DeliveryId,
                    deliveryStatusId = statusDelivery.deliveryStatusId
                };

                UoWConsumer.DeliveryAdditionalData.Insert(deliveryAD);
                UoWConsumer.Save();
            }
            else
            {
                deliveryAD.deliveryStatusId = statusDelivery.deliveryStatusId;
                UoWConsumer.DeliveryAdditionalData.Update(deliveryAD);
                UoWConsumer.Save();
            }
        }

        private void ValidUpdateRouteTeamInventory(SaleTeamTransactionMessage sale)
        {
            ERolTeam userRole = UoWConsumer.GetUserRole(sale.UserId);
            if (userRole == ERolTeam.SinAsignar)
            {
                return;
            }
            UpdateRouteTeamInventory(sale);
        }

        private void UpdateRouteTeamInventory(SaleTeamTransactionMessage sale)
        {
            List<SaleDetail> salesDetail = sale.SaleDetails;
            //List<SalePromotion> salePromotion = sale.SalePromotions;
            foreach (var productInventory in salesDetail)
            {
                var product = UoWConsumer.RouteTeamInventoryAvailableRepository
                    .Get(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(productInventory.ProductId)).FirstOrDefault();
                product.Available_Amount -= productInventory.Amount;

                UoWConsumer.RouteTeamInventoryAvailableRepository.Update(product);
                UoWConsumer.Save();
            }

            
            //foreach (var Promotion in salePromotion)
            //{
            //    foreach (var producPromotion in Promotion.DetailProduct)
            //    {
            //        var product = UoWConsumer.RouteTeamInventoryAvailableRepository.Get(s => s.inventoryId.Equals(sale.InventoryId) && s.productId.Equals(producPromotion.ProductId)).FirstOrDefault();
            //        product.Available_Amount -= producPromotion.Amount;
            //        UoWConsumer.RouteTeamInventoryAvailableRepository.Update(product);
            //        UoWConsumer.Save();
            //    }
            //}
        }

        private void CreatePaymentMethod(SaleTeamTransactionMessage sale)
        {
            var findResult = UoWConsumer.SaleAdditionalDataRepository.Get(a => a.saleId == sale.SaleId && a.paymentMethod.Trim() == sale.PaymentMethod).FirstOrDefault();
            if (findResult == null)
            {
                so_sale_aditional_data entitySale = new so_sale_aditional_data();
                entitySale.saleId = sale.SaleId;
                entitySale.paymentMethod = sale.PaymentMethod;
                UoWConsumer.SaleAdditionalDataRepository.Insert(entitySale);
                UoWConsumer.Save();
            }
        }

        private string CreatePromotion(SaleTeamTransactionMessage sale)
        {
            List<DataTable> dataTables = createDataTableParameters(sale);
            DataTable dtPromotionCatalog = dataTables[0];
            DataTable dtPromotionProduct = dataTables[1];
            DataTable dtPromotionGiftProduct = dataTables[2];
            DataTable dtPromotionGiftArticle = dataTables[3];
            DataTable dtPromotionSaleArticle = dataTables[4];
            DataTable dtPromotionData = dataTables[5];
            string sRespuesta = "";

            var command = UoWConsumer.Context.Database.Connection.CreateCommand();
            command.Transaction = UoWConsumer.Context.Database.BeginTransaction().UnderlyingTransaction;
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

        private List<DataTable> createDataTableParameters(SaleTeamTransactionMessage sale)
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

        private void SendTicketDigital(so_customer_additional_data updateCustomerAdditionalData, SaleTeamTransactionMessage sale, SaleTeamTransactionMessage saleResult)
        {
            if (updateCustomerAdditionalData != null || !string.IsNullOrEmpty(sale.Email))
            {
                #region Consumidores logica

                if (sale.EmailDeliveryTicket == null)
                    sale.EmailDeliveryTicket = false;

                //Envio de Ticket
                if (sale.EmailDeliveryTicket == true)
                {
                    var customer = UoWConsumer.CustomerRepository.Get(x => x.customerId == sale.CustomerId).FirstOrDefault();

                    if (customer.CustomerAdditionalData != null || !string.IsNullOrEmpty(sale.Email))
                    {
                        var customerAux = customer.CustomerAdditionalData.FirstOrDefault();
                        if (customerAux == null)
                            customerAux = new so_customer_additional_data { IsMailingActive = false };

                        if (!string.IsNullOrEmpty(sale.Email))
                            customer.email = sale.Email;

                        if (customerAux.IsMailingActive || !string.IsNullOrEmpty(sale.Email))
                        {
                            //Se prepara la información
                            var route = UoWConsumer.RouteCustomerRepository.Get(x => x.customerId == sale.CustomerId).FirstOrDefault();
                            var user = UoWConsumer.UserRepository.Get(x => x.userId == sale.UserId).FirstOrDefault();
                            //DataTable dtTicket = GetPromotionsTicketDigital(db, sale.SaleId);

                            var sendTicketDigitalEmail = new SendTicketDigitalEmailRequest
                            {
                                CustomerName = customer.name,
                                RouteAddress = Convert.ToString(route.routeId),
                                CustomerEmail = customer.email,
                                CustomerFullName = customer.customerId + " - " + customer.name + " " + customer.address,
                                Date = DateTime.Now,
                                PaymentMethod = sale.PaymentMethod,
                                SellerName = user.code + " - " + user.name,
                                // dtTicket = dtTicket
                            };

                            var sales = new List<SendTicketDigitalEmailSales>();
                            foreach (var detail in saleResult.SaleDetails)
                            {
                                var product = UoWConsumer.ProductRepository.Get(x => x.productId == detail.ProductId).FirstOrDefault();
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

                            sendTicketDigitalEmail.CancelTicketLink = UoWConsumer.GetCancelLinkByCustomerId(customer.customerId);
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
        }

        #endregion

        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }
    }
}