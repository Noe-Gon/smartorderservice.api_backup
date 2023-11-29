﻿using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public sealed class CustomerVarioService
    {
        private SmartOrderModel db = new SmartOrderModel();
        public int GetCustomerVarioIdByRouteId(int routeId)
        {
            var customerVario = db.so_customer.Where(x => x.name == "cliente_vario" && x.status).ToList();
            var customerVarioIds = customerVario.Select(x => x.customerId).ToList();
            var routeCustomerLst = db.so_route_customer.Where(x => customerVarioIds.Contains(x.customerId)).ToList();
            var routeCustomer = routeCustomerLst.Where(x => x.routeId == routeId).FirstOrDefault();
            if (routeCustomer == null)
            {
                throw new NotFoundVarioRouteException($"La ruta con identificador {routeId} no tienen configurado un cliente vario");
            }
            return routeCustomer.customerId;
        }

        public so_route_customer GetCustomerVarioByRouteId(int routeId)
        {
            var customerVario = db.so_route_customer
                    .Where(x => x.routeId == routeId && x.so_customer.name == "cliente_vario")
                    .FirstOrDefault();
            if (customerVario == null)
            {
                throw new NotFoundVarioRouteException($"La ruta con identificador {routeId} no tienen configurado un cliente vario");
            }
            return customerVario;
        }
    }
}