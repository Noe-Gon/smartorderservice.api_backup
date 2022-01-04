using SmartOrderService.DB;
using SmartOrderService.Models.Enum;
using SmartOrderService.Models.Message;
using SmartOrderService.Models.Responses;
using SmartOrderService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class LiquidationService : IDisposable
    {
        public static LiquidationService Create() => new LiquidationService();

        private UoWConsumer UoWConsumer { get; set; }

        public LiquidationService()
        {
            UoWConsumer = new UoWConsumer();
        }

        public ResponseBase<GetLiquidationSalesResponse> GetLiquidationSales(GetLiquidationSalesRequest request)
        {
            if (request.Date == null)
                request.Date = DateTime.Today;

            var team = UoWConsumer.RouteTeamRepository
                .Get(x => x.routeId == request.RouteId)
                .ToList();

            if (team.Where(x => x.userId == request.UserId && x.roleTeamId == (int)ERolTeam.Ayudante).FirstOrDefault() != null)
                return ResponseBase<GetLiquidationSalesResponse>.Create(new List<string>()
                {
                    "Solo los impulsores pueden realizar esta consulta"
                });

            var teamIds = team.Select(x => x.userId).ToList();

            var sales = UoWConsumer.SaleRepository
                .Get(x => teamIds.Contains(x.userId) && DbFunctions.TruncateTime(x.date) == DbFunctions.TruncateTime(request.Date))
                .Select(x => new 
                {
                    saleId = x.saleId,
                    customerId = x.customerId,
                    so_customer = new 
                    {
                        customerId = x.customerId,
                        name = x.so_customer.name
                    },
                    TotalCash = x.total_cash,
                    TotalCredit = x.total_credit,
                    PaymentMethod =  x.so_sale_aditional_data.FirstOrDefault() == null ? "": x.so_sale_aditional_data.FirstOrDefault().paymentMethod,
                    Details = x.so_sale_detail.Select(d => new GetLiquidationSalesDetail
                    {
                        Amount = d.amount,
                        ProductPrice = (double)(d.price_without_taxes ?? 0) + (double)(d.vat ?? 0),
                        ProductId = d.productId,
                        BarCode = d.so_product.barcode,
                        ProductName = d.so_product.name,
                        TotalPrice = d.import
                    }).ToList()
                })
                .ToList();
            var response = new GetLiquidationSalesResponse();
            foreach (var sale in sales)
            {
                var customer = response.Customers.Where(x => x.CustomerId == sale.customerId).FirstOrDefault();
                //Si el cliente existe
                if(customer != null)
                {
                    var newSale = new GetLiquidationSalesSale
                    {
                        PaymentMethod = sale.PaymentMethod,
                        Details = sale.Details,
                        SaleId = sale.saleId,
                        Total = sale.Details.Sum(x => x.TotalPrice)
                    };
                    customer.Sales.Add(newSale);
                    customer.CardAmount += sale.TotalCredit;
                    customer.CashAmount += sale.TotalCash;
                    response.TotalCard += customer.CardAmount;
                    response.TotalCash += customer.CashAmount;
                }
                //Si el cliente no existe
                else
                {
                    var newCustomer = new GetLiquidationSalesCustomer()
                    {
                        CustomerId = sale.customerId,
                        Name = sale.so_customer.name,
                        CardAmount = 0,
                        CashAmount = 0,
                        Sales = new List<GetLiquidationSalesSale>()
                    };
                    var newSale = new GetLiquidationSalesSale
                    {
                        PaymentMethod = sale.PaymentMethod,
                        Details = sale.Details,
                        SaleId = sale.saleId,
                        Total = sale.Details.Sum(x => x.TotalPrice)
                    };
                    newCustomer.Sales.Add(newSale);
                    newCustomer.CardAmount = sale.TotalCredit;
                    newCustomer.CashAmount = sale.TotalCash;
                    response.Customers.Add(newCustomer);
                    response.TotalCard += newCustomer.CardAmount;
                    response.TotalCash += newCustomer.CashAmount;
                }
                
            }
            response.Total = response.TotalCard + response.TotalCash;

            return ResponseBase<GetLiquidationSalesResponse>.Create(response);
        }

        public ResponseBase<GetLiquidationPromotionsResponse> GetLiquidationPromotions(GetLiquidationPromotionsRequest request)
        {
            if (request.Date == null)
                request.Date = DateTime.Today;

            var team = UoWConsumer.RouteTeamRepository
                .Get(x => x.routeId == request.RouteId)
                .ToList();

            if (team.Where(x => x.userId == request.UserId && x.roleTeamId == (int)ERolTeam.Ayudante).FirstOrDefault() != null)
                return ResponseBase<GetLiquidationPromotionsResponse>.Create(new List<string>()
                {
                    "Solo los impulsores pueden realizar esta consulta"
                });

            var teamIds = team.Select(x => x.userId).ToList();

            var sales = UoWConsumer.SaleRepository
                .Get(x => teamIds.Contains(x.userId) && DbFunctions.TruncateTime(x.date) == DbFunctions.TruncateTime(request.Date))
                .Select(x => new
                {
                    saleId = x.saleId,
                    customerId = x.customerId,
                    so_customer = new
                    {
                        customerId = x.customerId,
                        name = x.so_customer.name
                    },
                    TotalCash = x.total_cash,
                    TotalCredit = x.total_credit,
                    Promotions = x.so_sale_promotion.Select(p => new GetLiquidationPromotionsPromotion
                    {
                        SaleId = x.saleId,
                        PaymentMethod = x.so_sale_aditional_data.FirstOrDefault() == null ? "" : x.so_sale_aditional_data.FirstOrDefault().paymentMethod,
                        PromotionType = p.so_promotion.type.ToString(),
                        Details = p.so_sale_promotion_detail.Select(d => new GetLiquidationPromotionsDetail
                        {
                            Amount = d.amount,
                            BarCode = d.so_product.barcode,
                            ProductId = d.productId,
                            ProductName = d.so_product.name,
                            ProductPrice = d.price,
                            TotalPrice = (d.price * (double)d.amount)
                        }).ToList()
                    }).ToList()
                })
                .ToList();

            var response = new GetLiquidationPromotionsResponse();

            foreach (var sale in sales)
            {
                var customer = response.Customers.Where(x => x.CustomerId == sale.customerId).FirstOrDefault();
                //Si el cliente existe
                if (customer != null)
                {
                    customer.Promotions.AddRange(sale.Promotions);
                    customer.CardAmount += sale.Promotions.Where(x => x.PaymentMethod.ToUpper() == PaymentMethod.CARD).Sum(p => p.Details.Sum(d => d.TotalPrice));
                    customer.CashAmount += sale.Promotions.Where(x => x.PaymentMethod.ToUpper() == PaymentMethod.CASH).Sum(p => p.Details.Sum(d => d.TotalPrice));
                    response.TotalCard += customer.CardAmount;
                    response.TotalCash += customer.CashAmount;
                }
                //Si el cliente no existe
                else
                {
                    var newCustomer = new GetLiquidationPromotionsCustomer()
                    {
                        CustomerId = sale.customerId,
                        Name = sale.so_customer.name,
                        CardAmount = 0,
                        CashAmount = 0,
                        Promotions = sale.Promotions
                    };

                    newCustomer.CardAmount = sale.Promotions.Where(x => x.PaymentMethod.ToUpper() == PaymentMethod.CARD).Sum(p => p.Details.Sum(d => d.TotalPrice));
                    newCustomer.CashAmount = sale.Promotions.Where(x => x.PaymentMethod.ToUpper() == PaymentMethod.CASH).Sum(p => p.Details.Sum(d => d.TotalPrice));
                    response.TotalCard += newCustomer.CardAmount;
                    response.TotalCash += newCustomer.CashAmount;
                }
            }
            response.Total = response.TotalCard + response.TotalCash;

            return ResponseBase<GetLiquidationPromotionsResponse>.Create(response);
        }

        public ResponseBase<List<GetLiquidationRepaymentsResponse>> GetLiquidationRepayments(GetLiquidationRepaymentsRequest request)
        {
            if (request.Date == null)
                request.Date = DateTime.Today;

            var team = UoWConsumer.RouteTeamRepository
                .Get(x => x.routeId == request.RouteId)
                .ToList();

            if (team.Where(x => x.userId == request.UserId && x.roleTeamId == (int)ERolTeam.Ayudante).FirstOrDefault() != null)
                return ResponseBase<List<GetLiquidationRepaymentsResponse>>.Create(new List<string>()
                {
                    "Solo los impulsores pueden realizar esta consulta"
                });

            var impulsorId = team.Where(x => x.roleTeamId == (int)ERolTeam.Impulsor).Select(x => x.userId).FirstOrDefault();

            //Obtener los inventarios
            var inventoriesIds = UoWConsumer.InventoryRepository
                .Get(x => x.userId == impulsorId && DbFunctions.TruncateTime(x.date) == DbFunctions.TruncateTime(request.Date))
                .Select(x => x.inventoryId)
                .ToList();

            var response = new List<GetLiquidationRepaymentsResponse>();

            foreach (var inventoryId in inventoriesIds)
            {
                var products = GetUnsoldProducts(inventoryId).Select(x => new GetLiquidationRepaymentsResponse
                {
                    Amount = x.Available_Amount,
                    ProductId = x.productId
                }).ToList();

                foreach (var product in products)
                {
                    var productExist = response.Where(x => x.ProductId == product.ProductId).FirstOrDefault();

                    if(productExist == null)
                    {
                        var productAux = UoWConsumer.ProductRepository
                            .Get(x => x.productId == product.ProductId)
                            .FirstOrDefault();

                        var newProduct = new GetLiquidationRepaymentsResponse
                        {
                            Amount = product.Amount,
                            Code = productAux.code,
                            Name = productAux.name,
                            ProductId = product.ProductId
                        };

                        response.Add(newProduct);
                    }
                    else
                    {
                        productExist.Amount += product.Amount;
                    }
                }
            }

            return ResponseBase<List<GetLiquidationRepaymentsResponse>>.Create(response);
        }

        private List<so_route_team_inventory_available> GetUnsoldProducts(int inventoryId)
        {
            var inventoryAvailable = UoWConsumer.RouteTeamInventoryAvailableRepository.Get(s => s.inventoryId.Equals(inventoryId)).ToList();
            var inventoryCloneObject = new List<so_route_team_inventory_available>();
            foreach (var routeProduct in inventoryAvailable)
            {
                inventoryCloneObject.Add((so_route_team_inventory_available)routeProduct.Clone());

            }
            
            return inventoryCloneObject.Where(s => s.Available_Amount > 0).ToList();
        }

        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }

        #region Models Helpers
  
        #endregion
    }
}