using SmartOrderService.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using SmartOrderService.Models.DTO;
using AutoMapper;
using SmartOrderService.Models;

namespace SmartOrderService.Services
{
    public class PriceService
    {
        private SmartOrderModel db = new SmartOrderModel();

        public List<PriceDto> getPricesByCustomerBranch(int CustomerId, int BranchId, DateTime updated, List<int> productsIds, List<PriceDto> MasterPrices, so_branch_tax branch_tax)
        {

            List<PriceDto> prices = new List<PriceDto>();

            var priceList = db.so_products_price_list
           .Join(db.so_customer_products_price_list,
               pl => pl.products_price_listId,
               plc => plc.products_price_listId,
               (pl, plc) => new { pl, plc }
          )
           .Where(
               x => x.pl.branchId.Equals(BranchId)
               && x.pl.status
               && x.plc.customerId.Equals(CustomerId)
               && x.plc.status
               && DbFunctions.TruncateTime(x.pl.validity_start) <= DbFunctions.TruncateTime(DateTime.Today)
               && DbFunctions.TruncateTime(x.pl.validity_end) >= DbFunctions.TruncateTime(DateTime.Today)
          )
          .Select(p => new { PriceListId = p.pl.products_price_listId }).FirstOrDefault();

            if (priceList != null)
            {
                var priceDetails = db.so_price_list_products_detail
                   .Where(
                        p => p.products_price_listId.Equals(priceList.PriceListId)
                        && p.status
                        && p.so_product.type == 1
                        && p.so_product.status
                        && productsIds.Contains(p.productId)
                    ).
                    ToArray();

                prices.AddRange(Mapper.Map<so_price_list_products_detail[], List<PriceDto>>(priceDetails));
            }

            prices = MergePrices(MasterPrices, prices, branch_tax);

            return prices;
        }

        public List<PriceDto> getPricesByCustomerBranchv2(int CustomerId, int routeId, int BranchId, DateTime updated, List<int> productsIds, List<PriceDto> MasterPrices, so_branch_tax branch_tax, bool vario = false)
        {
            List<PriceDto> prices = new List<PriceDto>();

            var priceList = db.so_products_price_list
           .Join(db.so_customer_products_price_list,
               pl => pl.products_price_listId,
               plc => plc.products_price_listId,
               (pl, plc) => new { pl, plc }
          )
           .Where(
               x => x.pl.branchId.Equals(BranchId)
               && x.pl.status
               && x.plc.customerId.Equals(CustomerId)
               && x.plc.status
               && DbFunctions.TruncateTime(x.pl.validity_start) <= DbFunctions.TruncateTime(DateTime.Today)
               && DbFunctions.TruncateTime(x.pl.validity_end) >= DbFunctions.TruncateTime(DateTime.Today)
          )
          .Select(p => new { PriceListId = p.pl.products_price_listId }).FirstOrDefault();

            if (priceList == null)
            {
                CustomerVarioService varioService = new CustomerVarioService();
                var customerVario = varioService.GetCustomerVarioForCreate(db, routeId);

                if (customerVario != null)
                    priceList = db.so_products_price_list
                       .Join(db.so_customer_products_price_list,
                           pl => pl.products_price_listId,
                           plc => plc.products_price_listId,
                           (pl, plc) => new { pl, plc }
                      )
                       .Where(
                           x => x.pl.branchId.Equals(BranchId)
                           && x.pl.status
                           && x.plc.customerId.Equals(customerVario.customerId)
                           && x.plc.status
                           && DbFunctions.TruncateTime(x.pl.validity_start) <= DbFunctions.TruncateTime(DateTime.Today)
                           && DbFunctions.TruncateTime(x.pl.validity_end) >= DbFunctions.TruncateTime(DateTime.Today)
                      )
                      .Select(p => new { PriceListId = p.pl.products_price_listId }).FirstOrDefault();
            }

            if (priceList != null)
            {
                if (vario)
                {
                    var priceDetails = db.so_price_list_products_detail
                   .Where(
                        p => p.products_price_listId.Equals(priceList.PriceListId)
                        && p.status
                        && p.so_product.type == 1
                        && p.so_product.status
                    ).
                    ToArray();

                    prices.AddRange(Mapper.Map<so_price_list_products_detail[], List<PriceDto>>(priceDetails));
                }
                else
                {
                    var priceDetails = db.so_price_list_products_detail
                   .Where(
                        p => p.products_price_listId.Equals(priceList.PriceListId)
                        && p.status
                        && p.so_product.type == 1
                        && p.so_product.status
                        && productsIds.Contains(p.productId)
                    ).
                    ToArray();

                    prices.AddRange(Mapper.Map<so_price_list_products_detail[], List<PriceDto>>(priceDetails));
                }
            }
            if (vario)
            {
                prices = MergePrices(MasterPrices, prices, branch_tax, true);
            }
            else
            {
                prices = MergePrices(MasterPrices, prices, branch_tax);
            }

            return prices;
        }

        public List<PriceList> getPricesByInventory(int InventoryId, int BranchId, DateTime updated)
        {
            List<PriceList> PriceList = new List<PriceList>();

            var productsIds = new InventoryService().GetInventoryProductsIds(InventoryId);

            so_branch_tax branch_tax = db.so_branch_tax.FirstOrDefault(x => x.branchId == BranchId && x.status);

            var MasterPrices = getPricesByBranch(BranchId, updated, productsIds);

            var CustomerList = db.so_delivery.Where(d => d.inventoryId.Equals(InventoryId) && d.status).Select(c => c.customerId).ToList();

            var soUser = db.so_inventory.Where(i => i.inventoryId == InventoryId).FirstOrDefault().so_user;

            if(soUser.type == so_user.POAC_TYPE || soUser.type == so_user.CCEH_TYPE)
            {
                int day = (int)DateTime.Today.DayOfWeek;
                day++;

                var route = db.so_user_route.Where(ur => ur.userId == soUser.userId && ur.status).FirstOrDefault();

                var custo = db.so_route_customer.Where(rc => rc.routeId == route.routeId && rc.status && rc.day == day && !CustomerList.Contains(rc.customerId)).ToList();

                

                var routeVisits = db.so_user_route
                        .Join(db.so_route_customer,
                            userRoute => userRoute.routeId,
                            customerRoute => customerRoute.routeId,
                            (userRoute, customerRoute) => new { userRoute.userId, customerRoute.customerId, customerRoute.day, customerRouteStatus = customerRoute.status, userRouteStatus = userRoute.status }
                        )
                        .Where(
                            v => v.userId.Equals(soUser.userId)
                            && !CustomerList.Contains(v.customerId)
                            && day.Equals(v.day)
                            && v.customerRouteStatus
                            && v.userRouteStatus
                        )
                            .Select(c => c.customerId).ToList();

                CustomerList.AddRange(routeVisits);
            }
            

            foreach (var Customer in CustomerList)
            {

                List<PriceDto> prices = getPricesByCustomerBranch(Customer, BranchId, updated, productsIds, MasterPrices, branch_tax);

                PriceList.Add(new PriceList() { Prices = prices, CustomerId = Customer });

            }

            if (soUser.type == so_user.POAC_TYPE && soUser.type == so_user.CCEH_TYPE)
            {
                PriceList = PriceList.Select(pl => { pl.Prices = pl.Prices.Select(p => { p.HasDiscount = false; return p; }).ToList(); return pl; }).ToList();
            }

            return PriceList;

        }

        public List<PriceList> getPricesByInventoryv2(int InventoryId, int BranchId, DateTime updated)
        {
            List<PriceList> PriceList = new List<PriceList>();

            var productsIds = new InventoryService().GetInventoryProductsIds(InventoryId);

            so_branch_tax branch_tax = db.so_branch_tax.FirstOrDefault(x => x.branchId == BranchId && x.status);

            var MasterPrices = getPricesByBranch(BranchId, updated, productsIds);
            var MasterPricesAll = getPricesByBranch(BranchId, updated, productsIds, true);

            var CustomerList = db.so_delivery.Where(d => d.inventoryId.Equals(InventoryId) && d.status).Select(c => c.customerId).ToList();

            var soUser = db.so_inventory.Where(i => i.inventoryId == InventoryId).FirstOrDefault().so_user;
            var route = db.so_user_route.Where(ur => ur.userId == soUser.userId && ur.status).FirstOrDefault();

            CustomerVarioService varioService = new CustomerVarioService();
            var vario = varioService.GetCustomerVarioIdByRouteId(route.routeId);

            if (soUser.type == so_user.POAC_TYPE || soUser.type == so_user.CCEH_TYPE)
            {
                int day = (int)DateTime.Today.DayOfWeek;
                day++;

                var custo = db.so_route_customer.Where(rc => rc.routeId == route.routeId && rc.status && rc.day == day && !CustomerList.Contains(rc.customerId)).ToList();

                var routeVisits = db.so_user_route
                        .Join(db.so_route_customer,
                            userRoute => userRoute.routeId,
                            customerRoute => customerRoute.routeId,
                            (userRoute, customerRoute) => new { userRoute.userId, customerRoute.customerId, customerRoute.day, customerRouteStatus = customerRoute.status, userRouteStatus = userRoute.status }
                        )
                        .Where(
                            v => v.userId.Equals(soUser.userId)
                            && !CustomerList.Contains(v.customerId)
                            && day.Equals(v.day)
                            && v.customerRouteStatus
                            && v.userRouteStatus
                        )
                            .Select(c => c.customerId).ToList();

                CustomerList.AddRange(routeVisits);
            }

            foreach (var Customer in CustomerList)
            {
                if (Customer == vario)
                {
                    List<PriceDto> prices = getPricesByCustomerBranchv2(Customer, route.routeId, BranchId, updated, productsIds, MasterPricesAll, branch_tax, true);

                    PriceList.Add(new PriceList() { Prices = prices, CustomerId = Customer });
                }
                else
                {
                    List<PriceDto> prices = getPricesByCustomerBranchv2(Customer, route.routeId, BranchId, updated, productsIds, MasterPrices, branch_tax);

                    PriceList.Add(new PriceList() { Prices = prices, CustomerId = Customer });
                }

            }

            if (soUser.type == so_user.POAC_TYPE && soUser.type == so_user.CCEH_TYPE)
            {
                PriceList = PriceList.Select(pl => { pl.Prices = pl.Prices.Select(p => { p.HasDiscount = false; return p; }).ToList(); return pl; }).ToList();
            }

            return PriceList;

        }

        public List<PriceDto> getPricesByInventoryCustomer(int InventoryId, int BranchId, DateTime updated, int CustomerId)
        {
            List<PriceList> PriceList = new List<PriceList>();

            var productsIds = new InventoryService().GetInventoryProductsIds(InventoryId);

            so_branch_tax branch_tax = db.so_branch_tax.FirstOrDefault(x => x.branchId == BranchId && x.status);

            var MasterPrices = getPricesByBranch(BranchId, updated, productsIds);

            var CustomerList = db.so_delivery.Where(d => d.inventoryId.Equals(InventoryId) && d.status).Select(c => c.customerId).ToList();

            List<PriceDto> prices = getPricesByCustomerBranch(CustomerId, BranchId, updated, productsIds, MasterPrices, branch_tax);

            return prices;
        }

        public List<PriceDto> getPricesByInventoryCustomerv2(int InventoryId, int BranchId, DateTime updated, int CustomerId)
        {
            List<PriceList> PriceList = new List<PriceList>();

            var productsIds = new InventoryService().GetInventoryProductsIds(InventoryId);

            so_branch_tax branch_tax = db.so_branch_tax.FirstOrDefault(x => x.branchId == BranchId && x.status);

            var MasterPrices = getPricesByBranch(BranchId, updated, productsIds);

            var CustomerList = db.so_delivery.Where(d => d.inventoryId.Equals(InventoryId) && d.status).Select(c => c.customerId).ToList();

            var soUser = db.so_inventory.Where(i => i.inventoryId == InventoryId).FirstOrDefault().so_user;
            var route = db.so_user_route.Where(ur => ur.userId == soUser.userId && ur.status).FirstOrDefault();

            List<PriceDto> prices = getPricesByCustomerBranchv2(CustomerId, route.routeId, BranchId, updated, productsIds, MasterPrices, branch_tax);

            return prices;
        }

        public List<PriceDto> MergePrices(List<PriceDto> Master, List<PriceDto> CustomerPrices, so_branch_tax BranchTax, bool isVario = false)
        {

            foreach (var price in CustomerPrices)
            {
                price.HasDiscount = true;

            }

            var ids = CustomerPrices.Select(p => p.ProductId).ToList();

            var filtered = Master.Where(p => !ids.Contains(p.ProductId)).ToList();

            CustomerPrices = SetTaxes(BranchTax, CustomerPrices);
            if (!isVario)
            {
                CustomerPrices.AddRange(filtered);
            }

            return CustomerPrices;
        }

        public List<PriceDto> getPricesByBranch(int BranchId, DateTime updated, List<int> ProductsIds, bool all = false)
        {

            List<PriceDto> prices;

            so_branch_tax branch_tax = db.so_branch_tax.FirstOrDefault(x => x.branchId == BranchId && x.status);

            var priceList = db.so_products_price_list.Where(
                pl => pl.branchId.Equals(BranchId)
                && pl.status
                && DbFunctions.TruncateTime(pl.validity_start) <= DbFunctions.TruncateTime(DateTime.Today)
                && DbFunctions.TruncateTime(pl.validity_end) >= DbFunctions.TruncateTime(DateTime.Today)
                && pl.is_master
                ).Select(p => new { PriceListId = p.products_price_listId }).FirstOrDefault();

            if (priceList != null)
            {
                if (all)
                {
                    var priceDetails = db.so_price_list_products_detail
                   .Where(
                        p => p.products_price_listId.Equals(priceList.PriceListId)
                        && p.status
                        && p.so_product.type == 1
                        && p.so_product.status
                    ).
                    ToArray();

                    prices = Mapper.Map<so_price_list_products_detail[], List<PriceDto>>(priceDetails);

                    prices = SetTaxes(branch_tax, prices);
                }
                else
                {
                    var priceDetails = db.so_price_list_products_detail
                   .Where(
                        p => p.products_price_listId.Equals(priceList.PriceListId)
                        && p.status
                        && p.so_product.type == 1
                        && p.so_product.status
                        && ProductsIds.Contains(p.productId)
                    ).
                    ToArray();

                    prices = Mapper.Map<so_price_list_products_detail[], List<PriceDto>>(priceDetails);

                    prices = SetTaxes(branch_tax, prices);
                }
            }
            else
            {
                prices = new List<PriceDto>();
            }

            return prices;
        }


        private List<PriceDto> SetTaxes(so_branch_tax branch_tax, List<PriceDto> prices)
        {
            decimal stps_rate = 0;
            decimal stps_fee_rate = 0;
            decimal stps_snack_rate = 0;
            decimal vat_rate = 0;
            decimal net_content = 0;
            decimal stps_fee_product = 0;

            var productsIds = prices.Select(p => p.ProductId).ToList();

            List<so_product_tax> product_taxList = db.so_product_tax.Where(x => productsIds.Contains(x.productId) && x.status).ToList();

            foreach (var price in prices)
            {

                so_product_tax product_tax = product_taxList.FirstOrDefault(p => p.productId.Equals(price.ProductId));

                if (product_tax != null && branch_tax != null)
                {
                    stps_rate = product_tax.stps_apply == 1 ? branch_tax.stps : 0;
                    stps_fee_rate = product_tax.stps_fee_apply == 1 ? branch_tax.stps_fee : 0;
                    stps_snack_rate = product_tax.stps_snack_apply == 1 ? branch_tax.stps_snack : 0;
                    vat_rate = product_tax.vat_apply == 1 ? branch_tax.vat : 0;
                    net_content = Math.Round(product_tax.pieces * product_tax.trade_volume, 2, MidpointRounding.AwayFromZero);
                    stps_fee_product = net_content * stps_fee_rate;
                }

                decimal base_price_no_tax = price.PriceBaseValue;
                base_price_no_tax = base_price_no_tax / (1 + (vat_rate / 100));               //Quita IVA
                base_price_no_tax = base_price_no_tax - stps_fee_product;                                               //Quita volumen comercial
                base_price_no_tax = base_price_no_tax / (1 + (stps_rate / 100));            //Quita IEPS
                base_price_no_tax = base_price_no_tax / (1 + (stps_snack_rate / 100));        //Quita IEPS Botana
                base_price_no_tax = Math.Round(base_price_no_tax, 2);

                decimal discount_no_tax = price.PriceBaseValue - price.PriceValue;

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
                //decimal vat_total = Math.Round(((price_no_tax + stps_product + stps_snack_product + stps_fee_product) * detail.amount) * (vat_rate / 100), 2);

                price.base_price_no_tax = Math.Truncate(base_price_no_tax * 100) / 100;
                price.discount_no_tax = Math.Truncate(discount_no_tax * 100) / 100;
                price.vat = Math.Truncate(vat_product * 100) / 100;
                //price.vat_total = Math.Truncate((vat_total) * 100) / 100;
                price.stps = Math.Truncate(stps_product * 100) / 100;
                price.stps_fee = Math.Truncate(stps_fee_product * 100) / 100;
                price.stps_snack = Math.Truncate(stps_snack_product * 100) / 100;
                price.net_content = Math.Truncate(net_content * 100) / 100;
                price.vat_rate = Math.Truncate(vat_rate * 100) / 100;
                price.stps_rate = Math.Truncate(stps_rate * 100) / 100;
                price.stps_fee_rate = Math.Truncate(stps_fee_rate * 100) / 100;
                price.stps_snack_rate = Math.Truncate(stps_snack_rate * 100) / 100;
            }

            return prices;
        }
        

        public PriceDto getPriceDto(so_price_list_products_detail priceList, so_product_tax product_tax, so_branch_tax branch_tax)
        {
            PriceDto price = new PriceDto();
            decimal stps_rate = 0;
            decimal stps_fee_rate = 0;
            decimal stps_snack_rate = 0;
            decimal vat_rate = 0;
            decimal net_content = 0;
            decimal stps_fee_product = 0;

            if (product_tax != null && branch_tax != null)
            {
                stps_rate = product_tax.stps_apply == 1 ? branch_tax.stps : 0;
                stps_fee_rate = product_tax.stps_fee_apply == 1 ? branch_tax.stps_fee : 0;
                stps_snack_rate = product_tax.stps_snack_apply == 1 ? branch_tax.stps_snack : 0;
                vat_rate = product_tax.vat_apply == 1 ? branch_tax.vat : 0;
                net_content = Math.Round(product_tax.pieces * product_tax.trade_volume, 2, MidpointRounding.AwayFromZero);
                stps_fee_product = net_content * stps_fee_rate;
            }

            decimal base_price_no_tax = Convert.ToDecimal(priceList.base_price);
            base_price_no_tax = base_price_no_tax / (1 + (vat_rate / 100));               //Quita IVA
            base_price_no_tax = base_price_no_tax - stps_fee_product;                                               //Quita volumen comercial
            base_price_no_tax = base_price_no_tax / (1 + (stps_rate / 100));            //Quita IEPS
            base_price_no_tax = base_price_no_tax / (1 + (stps_snack_rate / 100));        //Quita IEPS Botana
            base_price_no_tax = Math.Round(base_price_no_tax, 2);

            decimal discount_no_tax = Convert.ToDecimal(priceList.base_price) - Convert.ToDecimal(priceList.price);

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
            //decimal vat_total = Math.Round(((price_no_tax + stps_product + stps_snack_product + stps_fee_product) * detail.amount) * (vat_rate / 100), 2);

            price.base_price_no_tax = Math.Truncate(base_price_no_tax * 100) / 100;
            price.discount_no_tax = Math.Truncate(discount_no_tax * 100) / 100;
            price.vat = Math.Truncate(vat_product * 100) / 100;
            //price.vat_total = Math.Truncate((vat_total) * 100) / 100;
            price.stps = Math.Truncate(stps_product * 100) / 100;
            price.stps_fee = Math.Truncate(stps_fee_product * 100) / 100;
            price.stps_snack = Math.Truncate(stps_snack_product * 100) / 100;
            price.net_content = Math.Truncate(net_content * 100) / 100;
            price.vat_rate = Math.Truncate(vat_rate * 100) / 100;
            price.stps_rate = Math.Truncate(stps_rate * 100) / 100;
            price.stps_fee_rate = Math.Truncate(stps_fee_rate * 100) / 100;
            price.stps_snack_rate = Math.Truncate(stps_snack_rate * 100) / 100;

            return price;
        }
    }
}