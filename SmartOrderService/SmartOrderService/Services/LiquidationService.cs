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

        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }
    }
}