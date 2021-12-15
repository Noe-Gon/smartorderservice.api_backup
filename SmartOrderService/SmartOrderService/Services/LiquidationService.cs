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
                    PaymentMethod =  x.so_sale_aditional_data.paymentMethod,
                    Details = x.so_sale_detail.Select(d => new GetLiquidationSalesDetail
                    {
                        Amount = d.amount,
                        ProductPrice = d.price,
                        ProductId = d.productId,
                        BarCode = d.so_product.barcode,
                        ProductName = d.so_product.name,
                        TotalPrice = d.price * d.amount
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
                    customer.CardAmount += sale.PaymentMethod.ToUpper() == PaymentMethod.CARD ? newSale.Total : 0;
                    customer.CashAmount += sale.PaymentMethod.ToUpper() == PaymentMethod.CASH ? newSale.Total : 0;
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
                    newCustomer.CardAmount = sale.PaymentMethod.ToUpper() == PaymentMethod.CARD ? newSale.Total : 0;
                    newCustomer.CashAmount = sale.PaymentMethod.ToUpper() == PaymentMethod.CASH ? newSale.Total : 0;
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
                    Promotions = x.so_sale_promotion.Select(p => new GetLiquidationPromotionsPromotion
                    {
                        SaleId = x.saleId,
                        PaymentMethod = x.so_sale_aditional_data.paymentMethod,
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
                    customer.CardAmount += sale.Promotions.Where(x => x.PaymentMethod.ToUpper() == PaymentMethod.CARD).Sum(x => x.Details.Sum(x => x.TotalPrice));
                    customer.CashAmount += sale.Promotions.Where(x => x.PaymentMethod.ToUpper() == PaymentMethod.CASH).Sum(x => x.Details.Sum(x => x.TotalPrice));
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

                    newCustomer.CardAmount = sale.Promotions.Where(x => x.PaymentMethod.ToUpper() == PaymentMethod.CARD).Sum(x => x.Details.Sum(x => x.TotalPrice));
                    newCustomer.CashAmount = sale.Promotions.Where(x => x.PaymentMethod.ToUpper() == PaymentMethod.CASH).Sum(x => x.Details.Sum(x => x.TotalPrice));
                    response.TotalCard += newCustomer.CardAmount;
                    response.TotalCash += newCustomer.CashAmount;
                }
            }
            response.Total = response.TotalCard + response.TotalCash;

            return ResponseBase<GetLiquidationPromotionsResponse>.Create(response);
        }

        public ResponseBase<GetLiquidationRepaymentsResponse> GetLiquidationRepayments(GetLiquidationRepaymentsRequest request)
        {
            if (request.Date == null)
                request.Date = DateTime.Today;

            var team = UoWConsumer.RouteTeamRepository
                .Get(x => x.routeId == request.RouteId)
                .ToList();

            if (team.Where(x => x.userId == request.UserId && x.roleTeamId == (int)ERolTeam.Ayudante).FirstOrDefault() != null)
                return ResponseBase<GetLiquidationRepaymentsResponse>.Create(new List<string>()
                {
                    "Solo los impulsores pueden realizar esta consulta"
                });

            var teamIds = team.Select(x => x.userId).ToList();

            var productsWithReplayments = UoWConsumer.ProductBottleRepository
                .GetAll().Select(x => new { x.bottleId, x.productId, x.so_product, x.so_product1 })
                .ToList();
            var productsWithReplaymentIds = productsWithReplayments.Select(x => x.productId).ToList();

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
                    PaymentMethod = x.so_sale_aditional_data.paymentMethod,
                    Details = x.so_sale_detail
                        .Where(x => productsWithReplaymentIds
                        .Contains(x.productId))
                        .Select(d => new GetLiquidationRepaymentsDetail
                    {
                        Amount = d.amount,
                        ProductPrice = 0,
                        ProductId = productsWithReplayments.Where(b => b.productId == d.productId).Select(x => x.bottleId).FirstOrDefault(),
                        BarCode = productsWithReplayments.Where(b => b.productId == d.productId).Select(x => x.so_product.barcode).FirstOrDefault(),
                        ProductName = productsWithReplayments.Where(b => b.productId == d.productId).Select(x => x.so_product.name).FirstOrDefault(),
                        TotalPrice = 0
                    }).ToList()
                })
                .ToList();

            var response = new GetLiquidationRepaymentsResponse();
            foreach (var sale in sales)
            {
                var customer = response.Customers.Where(x => x.CustomerId == sale.customerId).FirstOrDefault();
                //Si el cliente existe
                if (customer != null)
                {
                    var newSale = new GetLiquidationRepaymentsRepayment
                    {
                        PaymentMethod = sale.PaymentMethod,
                        Details = sale.Details,
                        SaleId = sale.saleId,
                        Total = sale.Details.Sum(x => x.TotalPrice)
                    };
                    customer.Sales.Add(newSale);
                    customer.CardAmount += sale.PaymentMethod.ToUpper() == PaymentMethod.CARD ? newSale.Total : 0;
                    customer.CashAmount += sale.PaymentMethod.ToUpper() == PaymentMethod.CASH ? newSale.Total : 0;
                    response.TotalCard += customer.CardAmount;
                    response.TotalCash += customer.CashAmount;
                }
                //Si el cliente no existe
                else
                {
                    var newCustomer = new GetLiquidationRepaymentsCustomer()
                    {
                        CustomerId = sale.customerId,
                        Name = sale.so_customer.name,
                        CardAmount = 0,
                        CashAmount = 0,
                        Sales = new List<GetLiquidationRepaymentsRepayment>()
                    };
                    var newSale = new GetLiquidationRepaymentsRepayment
                    {
                        PaymentMethod = sale.PaymentMethod,
                        Details = sale.Details,
                        SaleId = sale.saleId,
                        Total = sale.Details.Sum(x => x.TotalPrice)
                    };
                    newCustomer.Sales.Add(newSale);
                    newCustomer.CardAmount = sale.PaymentMethod.ToUpper() == PaymentMethod.CARD ? newSale.Total : 0;
                    newCustomer.CashAmount = sale.PaymentMethod.ToUpper() == PaymentMethod.CASH ? newSale.Total : 0;
                    response.TotalCard += newCustomer.CardAmount;
                    response.TotalCash += newCustomer.CashAmount;
                }

            }
            response.Total = response.TotalCard + response.TotalCash;

            return ResponseBase<GetLiquidationRepaymentsResponse>.Create(response);
        }
        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }
    }
}