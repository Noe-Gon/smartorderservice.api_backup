using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models.Enum;
using SmartOrderService.Models.Message;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using SmartOrderService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;

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
                .Get(x => x.userId == impulsorId && DbFunctions.TruncateTime(x.date) == DbFunctions.TruncateTime(request.Date.Value))
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

        public ResponseBase<List<GetEmptyBottleResponse>> GetEmptyBottle(GetEmptyBottleRequest request)
        {
            if (!request.Date.HasValue)
                request.Date = DateTime.Now;

            List<int> inventoriesId;

            if (request.InventoryId.HasValue && request.InventoryId > 0)
                inventoriesId = new List<int>() { request.InventoryId.Value };
            else
                inventoriesId = UoWConsumer.InventoryRepository
                    .Get(x => x.userId == request.UserId && DbFunctions.TruncateTime(x.date) == DbFunctions.TruncateTime(request.Date))
                    .Select(x => x.inventoryId)
                    .ToList();

            var userTeam = UoWConsumer.RouteTeamRepository
               .Get(x => x.userId == request.UserId)
               .FirstOrDefault();

            if (userTeam.roleTeamId == (int)ERolTeam.Ayudante)
                return ResponseBase<List<GetEmptyBottleResponse>>.Create(new List<string>()
                   {
                       "Solo los impulsores pueden realizar esta consulta"
                   });

            var teamIds = UoWConsumer.RouteTeamRepository
                .Get(x => x.routeId == userTeam.routeId)
                .Select(x => x.userId)
                .ToList();

            var response = UoWConsumer.SaleDetailRepository
                .Get(x => teamIds.Contains(x.so_sale.userId) && inventoriesId.Contains(x.so_sale.inventoryId.Value) 
                        && x.status && DbFunctions.TruncateTime(x.so_sale.date) == DbFunctions.TruncateTime(request.Date.Value))
                .Join(UoWConsumer.ProductBottleRepository.GetAll(), sd => sd.productId, pb => pb.productId,
                (sd, pb) => new
                {
                    Code = pb.so_product.code,
                    Id = pb.bottleId,
                    Name = pb.so_product.name,
                    Quantity = sd.amount,
                    BarCode = pb.so_product.barcode
                })
                .GroupBy(x => x.Id)
                .Select(x => new GetEmptyBottleResponse
                {
                    Id = x.Key,
                    BarCode = x.FirstOrDefault().BarCode,
                    Code = x.FirstOrDefault().Code,
                    Name = x.FirstOrDefault().Name,
                    Quantity = x.Sum(b => b.Quantity)
                })
                .ToList();

            return ResponseBase<List<GetEmptyBottleResponse>>.Create(response);
        }

        public ResponseBase<List<GetEmptyBottleResponse>> GetEmptyBottleInventory(GetEmptyBottleRequest request)
        {
            if (!request.Date.HasValue)
                request.Date = DateTime.Now;

            List<int> inventoriesId;

            if (!request.InventoryId.HasValue)
                inventoriesId = new List<int>() { request.InventoryId.Value };
            else
                inventoriesId = UoWConsumer.InventoryRepository
                    .Get(x => x.userId == request.UserId && DbFunctions.TruncateTime(x.date) == DbFunctions.TruncateTime(request.Date))
                    .Select(x => x.inventoryId)
                    .ToList();

            var userTeam = UoWConsumer.RouteTeamRepository
               .Get(x => x.userId == request.UserId)
               .FirstOrDefault();

            if (userTeam.roleTeamId == (int)ERolTeam.Ayudante)
                return ResponseBase<List<GetEmptyBottleResponse>>.Create(new List<string>()
                   {
                       "Solo los impulsores pueden realizar esta consulta"
                   });

            var bottlesIds = UoWConsumer.ProductBottleRepository
               .GetAll()
               .Select(x => x.productId)
               .ToList();

            var teamIds = UoWConsumer.RouteTeamRepository
                .Get(x => x.routeId == userTeam.routeId)
                .Select(x => x.userId)
                .ToList();

            var bottleProducts = UoWConsumer.SaleDetailRepository
                .Get(x => teamIds.Contains(x.so_sale.userId) && DbFunctions.TruncateTime(x.so_sale.date) == DbFunctions.TruncateTime(request.Date) && bottlesIds.Contains(x.productId))
                .Select(x => new GetEmptyBottleResponse
                {
                    Code = x.so_product.code,
                    Id = x.productId,
                    Name = x.so_product.name,
                    Quantity = x.amount,
                    BarCode = x.so_product.barcode
                })
                .ToList();

            return ResponseBase<List<GetEmptyBottleResponse>>.Create(bottleProducts);
        }

        public ResponseBase<SendLiquidationResponse> SendLiquidation(SendLiquidationRequest request)
        {

            if (request.Date == null)
                request.Date = DateTime.Now;

            int logStatusId = HelperLiquidationLogStatus.GetLiquidationStatusId(UoWConsumer, HelperLiquidationLogStatus.UNDEFINED);

            so_work_day workDay;
            try
            {
                workDay = UoWConsumer.GetWorkdayByUserAndDate(request.UserId, request.Date.Value);
            }
            catch (WorkdayNotFoundException e)
            {
                logStatusId = HelperLiquidationLogStatus.GetLiquidationStatusId(UoWConsumer, HelperLiquidationLogStatus.WORK_DAY_NOT_FOUND);

                var newLog = new so_liquidation_log()
                {
                    ExecutionIdAws = null,
                    BranchId = null,
                    JsonInput = null,
                    CreatedBy = request.UserId,
                    LiquidationStatusId = logStatusId,
                    RouteId = request.RouteId,
                    WorkDayId = null,
                    OutPut = e.Message
                };

                UoWConsumer.LiquidationLogRepository.Insert(newLog);
                UoWConsumer.Save();

                return ResponseBase<SendLiquidationResponse>.Create(new List<string>()
                {
                    newLog.OutPut
                });
            }

            // Verificar si existe algun viaje abierto
            var inventoriesOpen = UoWConsumer.InventoryRepository
                .Get(x => x.state == 1 && x.status && x.userId == request.UserId && DbFunctions.TruncateTime(x.date) == DbFunctions.TruncateTime(request.Date))
                .ToList();

            if (inventoriesOpen.Count() > 0) //Hay inventarios abiertos
            {
                if (request.CloseInvetories)
                {
                    foreach (var inventoryOpen in inventoriesOpen)
                    {
                        //Verificar si tiene una venta activa
                        var customerBlockedService = new CustomerBlockedService(UoWConsumer);
                        var response = customerBlockedService.GetCustomersBlockedByWorkday(workDay.work_dayId, inventoryOpen.inventoryId)
                            .Data.GroupBy(x => x.UserId)
                            .Select(x => x.Key)
                            .ToList();

                        if (response.Count() > 0 && !request.CloseSales)
                            throw new CustomerException("Clientes en proceso de venta");

                        foreach (var userId in response)
                        {
                            customerBlockedService.ClearBlockedCustomer(new ClearBlockedCustomerRequest { UserId = userId });
                        }

                        var routeTeamsTravlesEmployees = UoWConsumer.RouteTeamTravlesEmployeesRepository
                            .Get(x => x.inventoryId == inventoryOpen.inventoryId && x.work_dayId == workDay.work_dayId && x.active)
                            .ToList();

                        foreach (var rTE in routeTeamsTravlesEmployees)
                        {
                            rTE.active = false;
                        }
                        inventoryOpen.state = 2;
                        UoWConsumer.RouteTeamTravlesEmployeesRepository.UpdateByRange(routeTeamsTravlesEmployees);
                    }
                    UoWConsumer.Save();
                }
                else
                {
                    throw new InventoryNotClosedException("No se ha finalizado el viaje");
                }

                UoWConsumer.InventoryRepository.UpdateByRange(inventoriesOpen);
            }

            logStatusId = HelperLiquidationLogStatus.GetLiquidationStatusId(UoWConsumer, HelperLiquidationLogStatus.WORK_DAY_NOT_FOUND);

            var apiInfoRequest = UoWConsumer.RouteRepository
                .Get(x => x.routeId == request.RouteId)
                .Select(x => new 
                {
                    BranchId = x.branchId,
                    BranchCode = x.so_branch.code,
                    Date = request.Date.Value,
                    RouteCode = x.code
                })
                .FirstOrDefault();

            if (apiInfoRequest == null)
            {
                logStatusId = HelperLiquidationLogStatus.GetLiquidationStatusId(UoWConsumer, HelperLiquidationLogStatus.ROUTE_NOT_FOUND);

                var newLog = new so_liquidation_log()
                {
                    ExecutionIdAws = null,
                    BranchId = null,
                    JsonInput = null,
                    CreatedBy = request.UserId,
                    LiquidationStatusId = logStatusId,
                    RouteId = request.RouteId,
                    WorkDayId = workDay.work_dayId,
                    OutPut = "No se encontró la ruta con el Id: " + request.RouteId + " en WByC"
                };

                UoWConsumer.LiquidationLogRepository.Insert(newLog);
                UoWConsumer.Save();

                return ResponseBase<SendLiquidationResponse>.Create(new List<string>()
                {
                    newLog.OutPut
                });
            }

            try
            {
                var digitalizationService = new ApiDigitalizacionService();

                var response = digitalizationService.SaleSyncApi(new SaleSyncsRequest
                {
                    BranchCode = apiInfoRequest.BranchCode,
                    Date = apiInfoRequest.Date,
                    RouteCode = apiInfoRequest.RouteCode
                });

                if (response.Status)
                {
                    logStatusId = HelperLiquidationLogStatus.GetLiquidationStatusId(UoWConsumer, HelperLiquidationLogStatus.RUNNING);

                    var newLog = new so_liquidation_log()
                    {
                        ExecutionIdAws = response.Data.executionId,
                        BranchId = apiInfoRequest.BranchId,
                        JsonInput = Newtonsoft.Json.JsonConvert.SerializeObject(new SaleSyncsRequest
                        {
                            BranchCode = apiInfoRequest.BranchCode,
                            Date = apiInfoRequest.Date,
                            RouteCode = apiInfoRequest.RouteCode
                        }),
                        CreatedBy = request.UserId,
                        LiquidationStatusId = logStatusId,
                        RouteId = request.RouteId,
                        WorkDayId = workDay.work_dayId,
                        OutPut = "El proceso ha iniciado correctamente"
                    };

                    UoWConsumer.LiquidationLogRepository.Insert(newLog);
                    UoWConsumer.Save();

                    return ResponseBase<SendLiquidationResponse>.Create(new SendLiquidationResponse
                    {
                        Msg = newLog.OutPut
                    });
                }
                else
                {
                    logStatusId = HelperLiquidationLogStatus.GetLiquidationStatusId(UoWConsumer, HelperLiquidationLogStatus.UNDEFINED);

                    var newLog = new so_liquidation_log()
                    {
                        ExecutionIdAws = response.Data.executionId,
                        BranchId = apiInfoRequest.BranchId,
                        JsonInput = Newtonsoft.Json.JsonConvert.SerializeObject(new SaleSyncsRequest
                        {
                            BranchCode = apiInfoRequest.BranchCode,
                            Date = apiInfoRequest.Date,
                            RouteCode = apiInfoRequest.RouteCode
                        }),
                        CreatedBy = request.UserId,
                        LiquidationStatusId = logStatusId,
                        RouteId = request.RouteId,
                        WorkDayId = workDay.work_dayId,
                        OutPut = "Error no definido, en api Digitalización"
                    };

                    UoWConsumer.LiquidationLogRepository.Insert(newLog);
                    UoWConsumer.Save();

                    return ResponseBase<SendLiquidationResponse>.Create(new List<string>()
                    {
                        newLog.OutPut
                    });
                }
            }
            catch (UnauthorisedException e)
            {
                logStatusId = HelperLiquidationLogStatus.GetLiquidationStatusId(UoWConsumer, HelperLiquidationLogStatus.UNAUTHORISED);

                var newLog = new so_liquidation_log()
                {
                    ExecutionIdAws = null,
                    BranchId = apiInfoRequest.BranchId,
                    JsonInput = Newtonsoft.Json.JsonConvert.SerializeObject(new SaleSyncsRequest
                    {
                        BranchCode = apiInfoRequest.BranchCode,
                        Date = apiInfoRequest.Date,
                        RouteCode = apiInfoRequest.RouteCode
                    }),
                    CreatedBy = request.UserId,
                    LiquidationStatusId = logStatusId,
                    RouteId = request.RouteId,
                    WorkDayId = workDay.work_dayId,
                    OutPut = e.Message
                };

                UoWConsumer.LiquidationLogRepository.Insert(newLog);
                UoWConsumer.Save();

                return ResponseBase<SendLiquidationResponse>.Create(new List<string>()
                    {
                        newLog.OutPut
                    });
            }
            catch (ConfigurationValueNotFoundException e)
            {
                logStatusId = HelperLiquidationLogStatus.GetLiquidationStatusId(UoWConsumer, HelperLiquidationLogStatus.CONFIGURATION_VALUE_NOT_FOUND);

                var newLog = new so_liquidation_log()
                {
                    ExecutionIdAws = null,
                    BranchId = apiInfoRequest.BranchId,
                    JsonInput = Newtonsoft.Json.JsonConvert.SerializeObject(new SaleSyncsRequest
                    {
                        BranchCode = apiInfoRequest.BranchCode,
                        Date = apiInfoRequest.Date,
                        RouteCode = apiInfoRequest.RouteCode
                    }),
                    CreatedBy = request.UserId,
                    LiquidationStatusId = logStatusId,
                    RouteId = request.RouteId,
                    WorkDayId = workDay.work_dayId,
                    OutPut = e.Message
                };

                UoWConsumer.LiquidationLogRepository.Insert(newLog);
                UoWConsumer.Save();

                return ResponseBase<SendLiquidationResponse>.Create(new List<string>()
                    {
                        newLog.OutPut
                    });
            }
            catch (InternalServerException e)
            {
                logStatusId = HelperLiquidationLogStatus.GetLiquidationStatusId(UoWConsumer, HelperLiquidationLogStatus.INTERNAL_SERVER_ERROR);

                var newLog = new so_liquidation_log()
                {
                    ExecutionIdAws = null,
                    BranchId = apiInfoRequest.BranchId,
                    JsonInput = Newtonsoft.Json.JsonConvert.SerializeObject(new SaleSyncsRequest
                    {
                        BranchCode = apiInfoRequest.BranchCode,
                        Date = apiInfoRequest.Date,
                        RouteCode = apiInfoRequest.RouteCode
                    }),
                    CreatedBy = request.UserId,
                    LiquidationStatusId = logStatusId,
                    RouteId = request.RouteId,
                    WorkDayId = workDay.work_dayId,
                    OutPut = e.Message
                };

                UoWConsumer.LiquidationLogRepository.Insert(newLog);
                UoWConsumer.Save();

                return ResponseBase<SendLiquidationResponse>.Create(new List<string>()
                    {
                        newLog.OutPut
                    });
            }
            catch (Exception e)
            {
                logStatusId = HelperLiquidationLogStatus.GetLiquidationStatusId(UoWConsumer, HelperLiquidationLogStatus.UNDEFINED);

                var newLog = new so_liquidation_log()
                {
                    ExecutionIdAws = null,
                    BranchId = apiInfoRequest.BranchId,
                    JsonInput = Newtonsoft.Json.JsonConvert.SerializeObject(new SaleSyncsRequest
                    {
                        BranchCode = apiInfoRequest.BranchCode,
                        Date = apiInfoRequest.Date,
                        RouteCode = apiInfoRequest.RouteCode
                    }),
                    CreatedBy = request.UserId,
                    LiquidationStatusId = logStatusId,
                    RouteId = request.RouteId,
                    WorkDayId = workDay.work_dayId,
                    OutPut = e.Message
                };

                UoWConsumer.LiquidationLogRepository.Insert(newLog);
                UoWConsumer.Save();

                return ResponseBase<SendLiquidationResponse>.Create(new List<string>()
                    {
                        newLog.OutPut
                    });
            }
        }

        public ResponseBase<GetLiquidationStatusResponse> GetLiquidationStatus(GetLiquidationStatusRequest request)
        {

            if (request.Date == null)
                request.Date = DateTime.Now;

            so_work_day workDay;
            try
            {
                workDay = UoWConsumer.GetWorkdayByUserAndDate(request.UserId, request.Date.Value);
            }
            catch (WorkdayNotFoundException e)
            {
                return ResponseBase<GetLiquidationStatusResponse>.Create(new List<string>()
                {
                    e.Message
                });
            }

            try
            {
                var log = UoWConsumer.LiquidationLogRepository
                    .Get(x => x.WorkDayId == workDay.work_dayId && x.ExecutionIdAws != null)
                    .OrderByDescending(x => x.CreatedOn)
                    .Select(x => new { Log = x, Code = x.LiquidationStatus.Code })
                    .FirstOrDefault();

                if (log == null)
                    throw new EntityNotFoundException("No se encontró una ejecución para la Jornada");

                if (log.Code == HelperLiquidationLogStatus.SUCCEEDED)
                    return ResponseBase<GetLiquidationStatusResponse>.Create(new GetLiquidationStatusResponse
                    {
                        Code = log.Code
                    });

                var apiDigitalizationService = new ApiDigitalizacionService();
                var response = apiDigitalizationService.GetSaleSyncStatus(new GetSaleSyncStatusRequest { executionIdAws = log.Log.ExecutionIdAws });

                if (response.Status)
                {
                    var logStatus = HelperLiquidationLogStatus.GetLiquidationStatusId(UoWConsumer, response.Data.status);
                    var updateLog = log.Log;
                    updateLog.LiquidationStatusId = logStatus;
                    updateLog.ModifiedBy = request.UserId;
                    updateLog.ModifiedOn = DateTime.Now;
                    updateLog.OutPut = response.Data.output;

                    UoWConsumer.LiquidationLogRepository.Update(updateLog);
                    UoWConsumer.Save();

                    return ResponseBase<GetLiquidationStatusResponse>.Create(new GetLiquidationStatusResponse
                    {
                        Code = response.Data.status
                    });
                }

                return ResponseBase<GetLiquidationStatusResponse>.Create(new List<string>()
                {
                    "Error con la api digitalización"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<GetLiquidationStatusResponse>.Create(new List<string>()
                {
                    e.Message
                });
            }
            
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
        private class ProductInfo
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
        #endregion
    }
}