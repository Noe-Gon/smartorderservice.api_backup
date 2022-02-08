using RestSharp;
using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Enum;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class DeliveryService
    {
        private SmartOrderModel db = new SmartOrderModel();
        public List<DeliveryDto> getDeliveriesBy(int InventoryId)
        {

            List<DeliveryDto> Deliveries = new List<DeliveryDto>();

            var InventoryDeliveries = db.so_delivery.Where(d => d.inventoryId == InventoryId && d.status);

            var so_user = db.so_inventory.Where(i => i.inventoryId == InventoryId && i.status).FirstOrDefault().so_user;


            if ((!InventoryDeliveries.Any() || InventoryDeliveries.Count() == 0) && so_user.type != so_user.CCEH_TYPE && so_user.type != so_user.POAC_TYPE)

                throw new InventoryEmptyException();

            foreach (var delivery in InventoryDeliveries) {

                Deliveries.Add(MapDelivery(delivery));

            }

            return Deliveries;

        }

        public List<so_customer> getCustomersToDeliver(int InventoryId, int UserId)
        {
            var Deliveries = db.so_delivery.Where(d => d.inventoryId == InventoryId && d.status);

            var so_user = db.so_user.Where(u => u.userId == UserId && u.status).FirstOrDefault();
            if (so_user != null && so_user.type != so_user.POAC_TYPE && so_user.type != so_user.CCEH_TYPE)
                if (!Deliveries.Any())

                    throw new InventoryEmptyException();

            var Customers = Deliveries.Select(d => d.so_customer).ToList();

            return Customers;

        }

        private DeliveryDto MapDelivery(so_delivery delivery)
        {
            DeliveryDto dto = new DeliveryDto() {
                CustomerId = delivery.customerId,
                InventoryId = delivery.inventoryId,
                Status = delivery.status,
                DeliveryId = delivery.deliveryId,
                DeliveryCode = delivery.code,
            };

            var details = delivery.so_delivery_detail.Where(d => d.status);

            foreach (var detail in details)
            {

                dto.DeliveryDetail.Add(new DeliveryDetailDto() {
                    ProductId = detail.productId,
                    Amount = detail.amount,
                    CreditAmount = detail.credit_amount,
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

            var replacments = delivery.so_delivery_replacement.Where(r => r.status);

            foreach (var replacment in replacments)
            {
                dto.DeliveryReplacements.Add(new DeliveryReplacementDto() {
                    ReplacementId = replacment.replacementId,
                    Amount = replacment.amount,
                    Status = replacment.status
                });

            }

            var deliveryPromotions = delivery.so_delivery_promotion.Where(p => p.status);

            foreach (var promotion in deliveryPromotions)
            {
                DeliveryPromotionDto DeliveryPromotion = new DeliveryPromotionDto()
                {
                    Amount = promotion.amount,
                    PromotionId = promotion.promotionId,
                    DeliveryPromotionId = promotion.delivery_promotionId,
                    Status = promotion.status
                };


                var promotiondetails = promotion.so_delivery_promotion_detail;

                foreach (var promotiondetail in promotiondetails)
                {
                    DeliveryPromotion.DeliveryPromotionDetailProduct.Add(
                        new DeliveryPromotionDetailDto() {
                            ProductId = promotiondetail.productId,
                            Amount = promotiondetail.amount,
                            Gift = promotiondetail.is_gift,
                            Status = promotiondetail.status,
                            base_price = promotiondetail.base_price,
                            price = promotiondetail.price,
                            discount = promotiondetail.discount,
                            discount_amount = promotiondetail.discount_amount,
                            discount_percent = promotiondetail.discount_percent,
                            ieps = promotiondetail.ieps,
                            ieps_fee = promotiondetail.ieps_fee,
                            ieps_fee_rate = promotiondetail.ieps_fee_rate,
                            ieps_rate = promotiondetail.ieps_rate,
                            ieps_snack = promotiondetail.ieps_snack,
                            ieps_snack_rate = promotiondetail.ieps_snack_rate,
                            liters = promotiondetail.liters,
                            net_price = promotiondetail.net_price,
                            price_without_taxes = promotiondetail.price_without_taxes,
                            discount_without_taxes = promotiondetail.discount_without_taxes,
                            vat = promotiondetail.vat,
                            vat_rate = promotiondetail.vat_rate
                        });
                }

                /*
                var promotiondetailarticles = promotion.so_delivery_promotion_article_detail;

                foreach(var detailarticle in promotiondetailarticles)
                {
                    DeliveryPromotion.DeliveryPromotionDetailArticle.Add(new DeliveryPromotionDetailArticleDto() {

                        ArticleId = detailarticle.articleId,
                        Amount = detailarticle.amount,
                        Status = detailarticle.status
                    });

                }*/

                dto.DeliveryPromotions.Add(DeliveryPromotion);

            }

            return dto;
        }

        public so_delivery_devolution CreateDevolution(int? DeliveryId, int ReasonId, int? UserId)
        {
            if (DeliveryId == null)
                throw new DeliveryNotFoundException();

            var delivery = db.so_delivery.Where(d => d.deliveryId == DeliveryId).FirstOrDefault();


            if (delivery == null) throw new DeliveryNotFoundException();

            var devolution = db.so_delivery_devolution.Where(d => d.deliveryId == DeliveryId).FirstOrDefault();


            if (devolution != null) return devolution;

            so_delivery_devolution newDevolution = new so_delivery_devolution()
            {

                deliveryId = (int)DeliveryId,
                reasonId = ReasonId,
                createdby = UserId == null ? delivery.so_inventory.userId : UserId,
                modifiedby = UserId == null ? delivery.so_inventory.userId : UserId,
                createdon = DateTime.Now,
                modifiedon = DateTime.Now,
                status = true
            };

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {


                    db.so_delivery_devolution.Add(newDevolution);

                    db.SaveChanges();
                    /*
                    var ItemToDownload = new ControlDownloadService().createControlDownload(newDevolution.deliveryId, UserId, ControlDownloadService.MODEL_TYPE_DELIVERY_DEVOLUTION);

                    db.so_control_download.Add(ItemToDownload);

                    db.SaveChanges();*/

                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    return null;
                }

            }


            return newDevolution;

        }

        public so_delivery_devolution CreateDevolution(int DeliveryId, int ReasonId)
        {

            var delivery = db.so_delivery.Where(d => d.deliveryId == DeliveryId).FirstOrDefault();


            if (delivery == null) throw new DeliveryNotFoundException();

            var devolution = db.so_delivery_devolution.Where(d => d.deliveryId == DeliveryId).FirstOrDefault();


            if (devolution != null) return devolution;

            so_delivery_devolution newDevolution = new so_delivery_devolution()
            {

                deliveryId = DeliveryId,
                reasonId = ReasonId,
                createdby = delivery.so_inventory.userId,
                modifiedby = delivery.so_inventory.userId,
                createdon = DateTime.Now,
                modifiedon = DateTime.Now,
                status = true
            };

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {


                    db.so_delivery_devolution.Add(newDevolution);

                    db.SaveChanges();
                    /*
                    var ItemToDownload = new ControlDownloadService().createControlDownload(newDevolution.deliveryId, UserId, ControlDownloadService.MODEL_TYPE_DELIVERY_DEVOLUTION);

                    db.so_control_download.Add(ItemToDownload);

                    db.SaveChanges();*/

                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    return null;
                }

            }


            return newDevolution;

        }

        public ResponseBase<SendOrderResponse> SendOrder(SendOrderRequest request)
        {
            var route = db.so_route
                .Where(x => x.routeId == request.RouteId && x.status)
                .Select(x => new { RouteCode = x.code, BranchCode = x.so_branch.code, RouteId = x.branchId, BranchId = x.branchId })
                .FirstOrDefault();

            if (route == null)
                return ResponseBase<SendOrderResponse>.Create(new List<string>()
                {
                    "No se encontró la ruta o ha sido eliminada"
                });

            var customer = db.so_customer
                .Where(x => x.customerId == request.CustomerId && x.status)
                .FirstOrDefault();

            if(customer == null)
                return ResponseBase<SendOrderResponse>.Create(new List<string>()
                {
                    "No se encontró al cliente con id: " + request.CustomerId + " o ha dado de baja"
                });

            var apiRequest = new SendDeliveryToPreventaAPIRequest()
            {
                branchId = route.BranchCode,
                routeId = route.RouteCode,
                customerId = request.CustomerId,
                deliveryDate = request.DeliveryDate,
                createdOn = DateTime.Now,
                originSystem = "wbcprev",
                customer = new SendDeliveryToPreventaAPICustomer()
                {
                    address = customer.address,
                    branchId = route.RouteId,
                    contact = customer.contact,
                    customerId = customer.customerId,
                    email = customer.email,
                    name = customer.name,
                    originSistem = "wbcprev",
                    originSistemId = customer.customerId,
                    prePaid = request.PrePaid,
                    routeId = route.RouteId
                },
                products = request.Products.Select(x => new SendDeliveryToPreventaAPIProduct
                {
                    price = x.Price,
                    productId = x.ProductId,
                    quantity = x.Quantity
                }).ToList()
            };

            var response = SendDeliveryToPreventaAPI(apiRequest);

            if (response)
                return ResponseBase<SendOrderResponse>.Create(new SendOrderResponse
                {
                    Msg = "Orden realizada con exitó"
                });

            return ResponseBase<SendOrderResponse>.Create(new List<string>()
            {
                "Ha ocurridó un error"
            });
        }

        public bool SendDeliveryToPreventaAPI(SendDeliveryToPreventaAPIRequest request)
        {
            var clientPreventaApi = new RestClient();
            clientPreventaApi.BaseUrl = new Uri(ConfigurationManager.AppSettings["PreventaAPI"]);
            var requestPreventaApiConfig = new RestRequest("/api/v1/preOrder/register", Method.POST);
            requestPreventaApiConfig.AddHeader("x-api-key", ConfigurationManager.AppSettings["x-api-key"]);
            requestPreventaApiConfig.RequestFormat = DataFormat.Json;
            var GetDeliveriesResponse = clientPreventaApi.Execute(requestPreventaApiConfig);

            if (GetDeliveriesResponse.StatusCode == System.Net.HttpStatusCode.OK)
                return true;

            if (GetDeliveriesResponse.StatusCode == System.Net.HttpStatusCode.Forbidden && GetDeliveriesResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new ApiPreventaNoAuthorizationException(GetDeliveriesResponse.Content);

            throw new ApiPreventaException(GetDeliveriesResponse.Content);
        }

        public List<so_delivery_devolution> getDevolutionsByPeriod(int UserId,DateTime Begin,DateTime End)
        {
            var devolutions = db.so_delivery_devolution.Where(
                d => d.createdby.Value.Equals(UserId)
                && d.createdon.Value.CompareTo(Begin) >= 0
                && d.createdon.Value.CompareTo(End) <= 0
                && d.status
                ).ToList();

            return devolutions;
        }

        public ResponseBase<List<GetDeliveriesResponse>> GetDeliveriesStatus(GetDeliveriesRequest request)
        {
            #region Validaciones
            var user = db.so_user
                .Where(x => x.userId == request.UserId && x.status)
                .FirstOrDefault();

            if (user == null)
                return ResponseBase<List<GetDeliveriesResponse>>.Create(new List<string>()
                {
                    "El usuario no existe o ha sido eliminado"
                });

            var driver = db.so_route_team.Where(r => r.roleTeamId == (int)ERolTeam.Impulsor && r.routeId == db.so_route_team
                .Where(x => x.userId == request.UserId)
                .Select(x => x.routeId).FirstOrDefault())
                .FirstOrDefault();

            if (driver == null)
                return ResponseBase<List<GetDeliveriesResponse>>.Create(new List<string>()
                {
                    "No se encontró al impulsor"
                });

            #endregion

            List<int> inventoryIds;

            if (request.InventoryId.HasValue)
            {
                var inventory = db.so_inventory
                    .Where(x => x.inventoryId == request.InventoryId && x.userId == driver.userId)
                    .FirstOrDefault();

                inventoryIds = new List<int>() { request.InventoryId.Value };
            }
            else
            {
                if (!request.Date.HasValue)
                    request.Date = DateTime.Today;

                inventoryIds = db.so_inventory
                    .Where(x => DbFunctions.TruncateTime(x.date) == DbFunctions.TruncateTime(request.Date) && x.status && x.userId == request.UserId)
                    .Select(x => x.inventoryId)
                    .ToList();
            }

            var response = db.so_sale
                .Where(x => inventoryIds.Contains(x.inventoryId.Value) && x.deliveryId.HasValue)
                .Select(x => new GetDeliveriesResponse
                {
                    SaleId = x.saleId,
                    Address = x.so_customer.address,
                    CustomerCode = x.so_customer.code,
                    CustomerId = x.customerId,
                    CustomerName = x.so_customer.name,
                    DeliveryId = x.deliveryId.Value,
                    State = x.state,
                    UserId = x.userId,
                    Products = x.so_sale_detail.Select(s => new GetDeliveriesProduct
                    {
                        Id = s.productId,
                        Name = s.so_product.name,
                        Price = s.base_price_no_tax.Value + s.vat.Value,
                        Quantity = s.amount,
                        TotalPrice = s.import
                    }).ToList()
                }).ToList();

            return ResponseBase<List<GetDeliveriesResponse>>.Create(response);
        }

        public ResponseBase<DeliveredResponse> Delivered(DeliveredRequest request)
        {
            var delivery = db.so_delivery
                .Where(x => x.code == request.code)
                .FirstOrDefault();

            if (delivery == null)
                throw new EntityNotFoundException("No se encontró el delivery con CODE: " + request.code);

            var clientPreventaApi = new RestClient();
            clientPreventaApi.BaseUrl = new Uri(ConfigurationManager.AppSettings["PreventaAPI"]);
            var requestPreventaApiConfig = new RestRequest("api/v1/preOrder", Method.PATCH);
            requestPreventaApiConfig.AddHeader("x-api-key", ConfigurationManager.AppSettings["x-api-key"]);
            requestPreventaApiConfig.RequestFormat = DataFormat.Json;
            requestPreventaApiConfig.AddBody(new UpdateDeliveryAPIPreventaRequest
            {
                groupCode = request.code,
                statusCode = ""
            });

            var apiResponse = clientPreventaApi.Execute(requestPreventaApiConfig);

            if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
        }

    }
}