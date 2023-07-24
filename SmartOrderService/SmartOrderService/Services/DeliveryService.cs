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
using System.Data.Entity.Core.Objects;
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

            var inventoryAvailable = db.so_inventory.Where(i => i.inventoryId == InventoryId && i.state != 2 && i.status).FirstOrDefault();

            if (inventoryAvailable == null)
            {
                throw new InventoryNotOpenException("El inventario se encuentra cerrado");
            }

            var so_user = inventoryAvailable.so_user;


            if ((!InventoryDeliveries.Any() || InventoryDeliveries.Count() == 0) && so_user.type != so_user.CCEH_TYPE && so_user.type != so_user.POAC_TYPE)

                throw new InventoryEmptyException();

            foreach (var delivery in InventoryDeliveries) {

                Deliveries.Add(MapDelivery(delivery));

            }

            return Deliveries;

        }

        public List<GetDeliveriesByInventoryResponse> GetDeliveriesByInventory(int InventoryId)
        {

            List<GetDeliveriesByInventoryResponse> Deliveries = new List<GetDeliveriesByInventoryResponse>();

            var InventoryDeliveries = db.so_delivery.Where(d => d.inventoryId == InventoryId && d.status && d.so_customer.status);

            var so_user = db.so_inventory.Where(i => i.inventoryId == InventoryId && i.status).FirstOrDefault().so_user;


            if ((!InventoryDeliveries.Any() || InventoryDeliveries.Count() == 0) && so_user.type != so_user.CCEH_TYPE && so_user.type != so_user.POAC_TYPE)

                throw new InventoryEmptyException();

            foreach (var delivery in InventoryDeliveries)
            {

                Deliveries.Add(MapGetDeliveriesResponse(delivery));

            }

            return Deliveries;

        }

        [Obsolete]
        public OrderDTO GetNewDeliveriesByCustomerId(int customerId)
        {
            List<OrderBodyDTO> body = new List<OrderBodyDTO>();
            var currentDate = DateTime.Now.Date;
            var orders = db.so_order.Where(x => x.customerId == customerId 
            && EntityFunctions.TruncateTime(x.createdon) == EntityFunctions.TruncateTime(currentDate) 
            && x.status).ToList();

            foreach (var order in orders)
            {
                List<OrderDetailDTO> orderDetails = new List<OrderDetailDTO>();
                foreach (var orderDetail in order.so_order_detail.ToList())
                {
                    orderDetails.Add(new OrderDetailDTO
                    {
                        productId = orderDetail.productId,
                        amount = orderDetail.amount,
                        price = orderDetail.price,
                        import = orderDetail.import
                    });
                }
                body.Add(new OrderBodyDTO
                {
                    OrderId = order.orderId,
                    CustomerId = order.customerId,
                    UserId = order.userId,
                    DeliveryDate = order.delivery,
                    OrderDetails = orderDetails,
                    TotalCash = order.total_cash
                });
            }
            OrderDTO response = new OrderDTO
            {
                Orders = body
            };

            return response;

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

                var promotiondetailarticles = promotion.so_delivery_promotion_article_detail;

                foreach(var detailarticle in promotiondetailarticles)
                {
                    DeliveryPromotion.DeliveryPromotionDetailArticle.Add(new DeliveryPromotionDetailArticleDto() {

                        ArticleId = detailarticle.articleId,
                        Amount = detailarticle.amount,
                        Status = detailarticle.status
                    });

                }

                dto.DeliveryPromotions.Add(DeliveryPromotion);

            }

            return dto;
        }

        private GetDeliveriesByInventoryResponse MapGetDeliveriesResponse(so_delivery delivery)
        {

            var deliveryStatusUndifined = db.so_delivery_status.Where(x => x.Code == DeliveryStatus.UNDEFINED)
                .FirstOrDefault();

            var deliveryStatus = db.so_delivery_additional_data.Where(x => x.deliveryId == delivery.deliveryId).Select(x => x.DeliveryStatus).FirstOrDefault();

            if (deliveryStatus == null)
                deliveryStatus = deliveryStatusUndifined ?? new so_delivery_status
                {
                    Code = "UNDEFINED",
                    Description = "Indefinido",
                    deliveryStatusId = 0
                };

            GetDeliveriesByInventoryResponse dto = new GetDeliveriesByInventoryResponse() {
                CustomerId = delivery.customerId,
                InventoryId = delivery.inventoryId,
                Status = delivery.status,
                DeliveryId = delivery.deliveryId,
                DeliveryCode = delivery.code,
                StatusId = deliveryStatus.deliveryStatusId,
                StatusName = deliveryStatus.Description,
                StatusCode = deliveryStatus.Code
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

            //Guardar en so_order
            var deliveryReference = db.so_delivery_references
                .Where(x => x.value == 80)
                .FirstOrDefault();

            if (deliveryReference == null)
                throw new Exception("No existe el delivery reference con valor 80");

            var newOrder = new so_order()
            {
                createdby = request.UserId,
                createdon = DateTime.Now,
                customerId = request.CustomerId,
                date = DateTime.Now, //Validar que valor va en date
                datesync = null,
                delivery = request.DeliveryDate, //Se cambio de string a Datetime
                delivery_reference = deliveryReference.delivery_references_Id,
                modifiedby = request.UserId,
                modifiedon = DateTime.Now,
                status = true,
                tags = "preventa",
                userId = request.UserId,
                total_cash = request.TotalCash,
                total_credit = request.TotalCredit,
                so_order_detail = new List<so_order_detail>()
            };

            foreach (var product in request.Products)
            {
                newOrder.so_order_detail.Add(new so_order_detail()
                {
                    amount = product.Quantity,
                    createdby = request.UserId,
                    createdon = DateTime.Now,
                    import = product.Import,
                    productId = product.ProductId,
                    price = product.Price,
                    modifiedby = request.UserId,
                    status = true,
                    modifiedon = DateTime.Now,
                    credit_amount = product.CreditAmount
                });
            }

            db.so_order.Add(newOrder);
            db.SaveChanges();

            var response = ResponseBase<SendOrderResponse>.Create(new SendOrderResponse
            {
                Msg = "Orden realizada con exitó"
            });

            try
            {
                string mailRespnse = SendOrderEmail(newOrder, request.RouteId);

                response.Data.Msg = response.Data.Msg + " - " + mailRespnse;
            }
            catch (Exception e)
            {
                response.Errors = new List<string>()
                {
                    e.Message
                };
            }

            return response;
        }

        public string SendOrderEmail(so_order order, int routeId)
        {
            var customer = db.so_customer
                .Where(x => x.customerId == order.customerId)
                .Select(x => new { x, x.CustomerAdditionalData })
                .FirstOrDefault();

            if (customer == null)
                throw new Exception("No se encontró al cliente");

            if (customer.CustomerAdditionalData.Count == 0)
                throw new Exception("El cliente no tiene datos adicionales");

            if (!customer.CustomerAdditionalData.FirstOrDefault().IsMailingActive)
                return "El email no se envio ya que no tiene activado la recepción de mails";

            var route = db.so_route
                .Where(x => x.routeId == routeId)
                .FirstOrDefault();

            if (route == null)
                throw new Exception("No se encontró la ruta");

            var user = db.so_user
                .Where(x => x.userId == order.userId)
                .FirstOrDefault();

            if (user == null)
                throw new Exception("No se encontró al usuario");

            SendOrderTicketRequest mailRequest = new SendOrderTicketRequest()
            {
                RouteAddress = route.code,
                CustomerFullName = customer.x.customerId + " - " + customer.x.name + " " + customer.x.address,
                CustomerMail = customer.x.email,
                CustomerName = customer.x.name,
                Date = DateTime.Now,
                DeliveryDate = order.delivery.Value,
                SallerName = user.code + " - " + user.name,
                ReferenceCode = customer.x.customerId.ToString(),
                Items = new List<SendOrderTicketItem>(),
                Status = order.status
            };

            var productIds = order.so_order_detail.Select(x => x.productId).ToList();
            var products = db.so_product
                .Where(x => productIds.Contains(x.productId))
                .ToList();

            foreach (var item in order.so_order_detail)
            {
                var product = products.Where(x => x.productId == item.productId).FirstOrDefault();

                mailRequest.Items.Add(new SendOrderTicketItem()
                {
                    Amount = item.amount,
                    ProductName = product.name,
                    TotalPrice = item.price * (double)item.amount,
                    UnitPrice = item.price
                });
            }

            var saleService = new SaleService();
            mailRequest.CancelTicketLink = saleService.GetCancelLinkByCustomerId(customer.x.customerId);

            var service = new EmailService();
            var response = service.SendOrderTicket(mailRequest);

            if (response.Status)
                return response.Data.Msg;

            throw new Exception(response.Errors.FirstOrDefault());
        }

        public ResponseBase<SendOrderResponse> UpdateOrder(NewDeliveryUpdateRequest request)
        {

            var customer = db.so_customer
                .Where(x => x.customerId == request.CustomerId && x.status)
                .FirstOrDefault();

            if (customer == null)
                return ResponseBase<SendOrderResponse>.Create(new List<string>()
                {
                    "No se encontró al cliente con id: " + request.CustomerId + " o ha dado de baja"
                });

            //Guardar en so_order
            var deliveryReference = db.so_delivery_references
                .Where(x => x.value == 80)
                .FirstOrDefault();

            if (deliveryReference == null)
                throw new Exception("No existe el delivery reference con valor 80");

            so_order orderToUpdate = db.so_order.Where(x => x.orderId == request.OrderId).FirstOrDefault();
            orderToUpdate.modifiedon = DateTime.Now;
            orderToUpdate.modifiedby = request.UserId;
            orderToUpdate.delivery = Convert.ToDateTime(request.DeliveryDate);

            db.so_order_detail.RemoveRange(orderToUpdate.so_order_detail);
            db.SaveChanges();
            orderToUpdate.so_order_detail = new List<so_order_detail>();

            foreach (var product in request.Products)
            {
                orderToUpdate.so_order_detail.Add(new so_order_detail()
                {
                    amount = product.Quantity,
                    createdby = request.UserId,
                    createdon = DateTime.Now,
                    import = product.Import,
                    productId = product.ProductId,
                    price = product.Price,
                    modifiedby = request.UserId,
                    status = true,
                    modifiedon = DateTime.Now,
                    credit_amount = product.CreditAmount
                });
            }
            db.so_order.Attach(orderToUpdate);
            db.SaveChanges();

            return ResponseBase<SendOrderResponse>.Create(new SendOrderResponse
            {
                Msg = "Orden actualizada con exitó"
            });
        }

        public ResponseBase<SendOrderResponse> CancelOrder(int orderId, int userId)
        {

            //var orderToCancel = db.so_order.Where(x => x.orderId == orderId && x.status).FirstOrDefault();
            var orderToCancel = db.so_order.SingleOrDefault(x => x.orderId == orderId);

            if (orderToCancel == null)
                return ResponseBase<SendOrderResponse>.Create(new List<string>()
                {
                    "No se encontró el predido"
                });

            orderToCancel.modifiedon = DateTime.Now;
            orderToCancel.modifiedby = userId;
            orderToCancel.status = false;
            //db.so_order.Attach(orderToCancel);

            var routeId = db.so_route_team
                .Where(x => x.userId == userId)
                .Select(x => x.routeId)
                .FirstOrDefault();

            db.SaveChanges();

            try
            {
                string mailRespnse = SendOrderEmail(orderToCancel, routeId);

                return ResponseBase<SendOrderResponse>.Create(new SendOrderResponse
                {
                    Msg = "Orden cancelada con exitó - Email: " + mailRespnse
                });
            }
            catch (Exception e)
            {
                var response = ResponseBase<SendOrderResponse>.Create(new SendOrderResponse
                {
                    Msg = "Orden cancelada con exitó"
                });

                response.Errors = new List<string>()
                {
                    "No se envió el email", e.Message
                };

                return response;
            }  
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

            var deliveryStatusUndifined = db.so_delivery_status.Where(x => x.Code == DeliveryStatus.UNDEFINED)
               .FirstOrDefault();

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

            if (deliveryStatusUndifined == null)
                deliveryStatusUndifined =  new so_delivery_status
                {
                    Code = "UNDEFINED",
                    Description = "Indefinido",
                    deliveryStatusId = 0
                };

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
                    StatusId = x.so_delivery.so_delivery_additional_data == null ? deliveryStatusUndifined.deliveryStatusId : (x.so_delivery.so_delivery_additional_data.DeliveryStatus == null ? deliveryStatusUndifined.deliveryStatusId : x.so_delivery.so_delivery_additional_data.DeliveryStatus.deliveryStatusId),
                    StatusCode = x.so_delivery.so_delivery_additional_data == null ? deliveryStatusUndifined.Code : (x.so_delivery.so_delivery_additional_data.DeliveryStatus == null ? deliveryStatusUndifined.Code : x.so_delivery.so_delivery_additional_data.DeliveryStatus.Code),
                    StatusName = x.so_delivery.so_delivery_additional_data == null ? deliveryStatusUndifined.Description : (x.so_delivery.so_delivery_additional_data.DeliveryStatus == null ? deliveryStatusUndifined.Description : x.so_delivery.so_delivery_additional_data.DeliveryStatus.Description),
                    Products = x.so_sale_detail.Select(s => new GetDeliveriesProduct
                    {
                        Id = s.productId,
                        Name = s.so_product.name,
                        Price = s.price,
                        Quantity = s.amount,
                        TotalPrice = s.price * s.amount
                    }).ToList()
                }).ToList();

            return ResponseBase<List<GetDeliveriesResponse>>.Create(response);
        }

        public ResponseBase<DeliveredResponse> UpdateDeliveryApiPreventa(DeliveredRequest request)
        {
            var delivery = db.so_delivery
                .Where(x => x.deliveryId == request.deliveryId)
                .FirstOrDefault();

            if (delivery == null)
                throw new EntityNotFoundException("No se encontró el delivery");

            // Crear la entidad
            if (delivery.so_delivery_additional_data == null)
                throw new EntityNotFoundException("No cuenta con datos adicionales");

            if(delivery.so_delivery_additional_data.DeliveryStatus == null)
                throw new EntityNotFoundException("No cuenta con el estado del delivery");

            var clientPreventaApi = new RestClient();
            clientPreventaApi.BaseUrl = new Uri(ConfigurationManager.AppSettings["PreventaAPI"]);
            var requestPreventaApiConfig = new RestRequest("api/v1/preOrder", Method.PATCH);
            requestPreventaApiConfig.AddHeader("x-api-key", ConfigurationManager.AppSettings["x-api-key"]);
            requestPreventaApiConfig.RequestFormat = DataFormat.Json;
            requestPreventaApiConfig.AddBody(new UpdateDeliveryAPIPreventaRequest
            {
                groupCode = delivery.code,
                statusCode = delivery.so_delivery_additional_data.DeliveryStatus.Code
            });

            var apiResponse = clientPreventaApi.Execute(requestPreventaApiConfig);
            string errorMsg = "Ocurrio un error al momento de procesar la información. API Preventa";

            if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                return ResponseBase<DeliveredResponse>.Create(new DeliveredResponse()
                {
                    Msg = "Actualizado con éxito"
                });

            if (apiResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
                errorMsg = "No se encontraron pedido a actualizar. API Preventa";

            if (apiResponse.StatusCode == (System.Net.HttpStatusCode)401)
                errorMsg = "Petición no autorizada, se requiere enviar el token de autorización o no cuenta con permisos. API Preventa";

            if (apiResponse.StatusCode == (System.Net.HttpStatusCode)403)
                errorMsg = "No se encuentra el header Authorization del usuario. API Preventa";

            if (apiResponse.StatusCode == (System.Net.HttpStatusCode)404)
                errorMsg = "Recurso no encontrado. API Preventa";

            if (apiResponse.StatusCode == (System.Net.HttpStatusCode)409)
                errorMsg = "El objeto ya existe en la BD de Intermedia. API Preventa";

            return ResponseBase<DeliveredResponse>.Create(new List<string>()
                    {
                        errorMsg
                    });
        }

        public ResponseBase<CancelDeliveryResponse> CancelDeliveryApiPreventa(CancelDeliveryRequest request)
        {
            var delivery = db.so_delivery
                .Where(x => x.deliveryId == request.deliveryId)
                .FirstOrDefault();

            if (delivery == null)
                throw new EntityNotFoundException("No se encontró el delivery");

            var statusDelivery = db.so_delivery_status
                    .Where(x => x.Code == DeliveryStatus.CANCELED)
                    .FirstOrDefault();

            if(statusDelivery == null)
            {
                throw new EntityNotFoundException("No se encontró el delivery status");
            }

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                // Crear la entidad
                if (delivery.so_delivery_additional_data == null)
                {
                    so_delivery_additional_data newDeliveryAdditionalData = new so_delivery_additional_data()
                    {
                        deliveryId = delivery.deliveryId,
                        deliveryStatusId = statusDelivery.deliveryStatusId
                    };
                    db.so_delivery_additional_data.Add(newDeliveryAdditionalData);
                    db.SaveChanges();

                    delivery.so_delivery_additional_data = newDeliveryAdditionalData;
                }
                //Actualizar el status
                else
                {
                    delivery.modifiedon = DateTime.Now;
                    delivery.so_delivery_additional_data.deliveryStatusId = statusDelivery.deliveryStatusId;
                    db.so_delivery.Attach(delivery);
                    db.Entry(delivery).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var clientPreventaApi = new RestClient();
                clientPreventaApi.BaseUrl = new Uri(ConfigurationManager.AppSettings["PreventaAPI"]);
                var requestPreventaApiConfig = new RestRequest("api/v1/preOrder", Method.PATCH);
                requestPreventaApiConfig.AddHeader("x-api-key", ConfigurationManager.AppSettings["x-api-key"]);
                requestPreventaApiConfig.RequestFormat = DataFormat.Json;
                requestPreventaApiConfig.AddBody(new UpdateDeliveryAPIPreventaRequest
                {
                    groupCode = delivery.code,
                    statusCode = delivery.so_delivery_additional_data.DeliveryStatus.Code
                });

                var apiResponse = clientPreventaApi.Execute(requestPreventaApiConfig);
                string errorMsg = "Ocurrio un error al momento de procesar la información. API Preventa";

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    dbContextTransaction.Commit();
                    return ResponseBase<CancelDeliveryResponse>.Create(new CancelDeliveryResponse()
                    {
                        Msg = "Cancelado con éxito"
                    });
                }
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    errorMsg = "No se encontraron pedido a actualizar. API Preventa";

                if (apiResponse.StatusCode == (System.Net.HttpStatusCode)401)
                    errorMsg = "Petición no autorizada, se requiere enviar el token de autorización o no cuenta con permisos. API Preventa";

                if (apiResponse.StatusCode == (System.Net.HttpStatusCode)403)
                    errorMsg = "No se encuentra el header Authorization del usuario. API Preventa";

                if (apiResponse.StatusCode == (System.Net.HttpStatusCode)404)
                    errorMsg = "Recurso no encontrado. API Preventa";

                if (apiResponse.StatusCode == (System.Net.HttpStatusCode)409)
                    errorMsg = "El objeto ya existe en la BD de Intermedia. API Preventa";

                dbContextTransaction.Rollback();

                return ResponseBase<CancelDeliveryResponse>.Create(new List<string>()
                    {
                        errorMsg
                    });
            }
        }
    }
}