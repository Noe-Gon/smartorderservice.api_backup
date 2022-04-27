using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Mappers;
using SmartOrderService.Models;
using SmartOrderService.Models.Enum;
using SmartOrderService.Models.Message;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
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
    public class DeliveryStatusService : IDisposable
    {
        public static DeliveryStatusService Create() => new DeliveryStatusService();

        private UoWConsumer UoWConsumer { get; set; }

        public DeliveryStatusService()
        {
            UoWConsumer = new UoWConsumer();

        }

        public ResponseBase<MsgResponseBase> UpdateDeliveryStatus(UpdateDeliveryStatus request)
        {
            var sale = UoWConsumer.SaleRepository
                .Get(x => x.saleId == request.SaleId)
                .FirstOrDefault();

            if (!sale.deliveryId.HasValue)
                return ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    "Esta venta no cuenta con delivery"
                });

            var delivery = UoWConsumer.DeliveryRepository
                .Get(x => x.deliveryId == sale.deliveryId && x.status)
                .FirstOrDefault();

            if (delivery == null)
                return ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    "No se encontró el delivery", "Delivery con id " + sale.deliveryId + " No encontradó o eliminadó"
                });

            string status = DeliveryStatus.DELIVERED;
            bool isUndelivered = true;
            foreach (var product in delivery.so_delivery_detail)
            {
                var aux = sale.so_sale_detail
                    .Where(x => x.productId == product.productId)
                    .FirstOrDefault();

                if (aux == null)
                    continue;

                if (aux.amount == 0)
                    continue;

                isUndelivered = false;
                if (aux.amount >= product.amount)
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
            .Get(x => x.deliveryId == sale.deliveryId)
            .FirstOrDefault();
            string previousStatus = "";
            //Buscar additional data y crearlo en caso contrario
            if (deliveryAD == null)
            {
                deliveryAD = new so_delivery_additional_data()
                {
                    deliveryId = sale.deliveryId.Value,
                    deliveryStatusId = statusDelivery.deliveryStatusId
                };
                previousStatus = "Indefinido";
                UoWConsumer.DeliveryAdditionalData.Insert(deliveryAD);
                UoWConsumer.Save();
            }
            else
            {
                previousStatus = deliveryAD.DeliveryStatus.Description;
                deliveryAD.deliveryStatusId = statusDelivery.deliveryStatusId;
                UoWConsumer.DeliveryAdditionalData.Update(deliveryAD);
                UoWConsumer.Save();
            }

            return ResponseBase<MsgResponseBase>.Create(new MsgResponseBase()
            {
                Msg = "El delivery ha pasado de status " + previousStatus + " a " + deliveryAD.DeliveryStatus.Description
            });
        }


        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }
    }
}