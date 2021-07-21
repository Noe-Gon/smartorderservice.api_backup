using SmartOrderService.DB;
using SmartOrderService.Models.Enum;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using SmartOrderService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class ConsumerService : IDisposable
    {
        public static ConsumerService Create() => new ConsumerService();

        private UoWConsumer UoWConsumer { get; set; }

        public ConsumerService()
        {
            UoWConsumer = new UoWConsumer();
        }

        public ResponseBase<InsertConsumerResponse> InsertConsumer(InsertConsumerRequest request)
        {
            try
            {
                var newCustomer = new so_customer
                {
                    name = request.Name,
                    createdby = request.UserId,
                    createdon = DateTime.Now,
                    email = request.Email,
                    latitude = request.Latitude,
                    longitude = request.Longitude
                };

                var newCustomerAdditionalData= new so_customer_additional_data
                {
                    Phone = request.Phone,
                    Phone_2 = request.Phone_2,
                    Email_2 = request.Email_2,
                    Status = (int)Consumer.STATUS.CONSUMER,
                    AcceptedTermsAndConditions = false,
                    IsMailingActive = true,
                    IsSMSActive = false,
                    CFECode = request.CFECode,
                    CodePlace = request.CodePlace,
                    CounterVisitsWithoutSales = 0,
                    Customer = newCustomer,
                    InteriorNumber = request.InteriorNumber,
                    Neighborhood = request.Neighborhood,
                    ReferenceCode = request.ReferenceCode
                };

                var newCustomerData = new so_customer_data
                {
                    address_number = request.ExternalNumber,
                    address_number_cross1 = request.Crossroads,
                    address_number_cross2 = request.Crossroads_2,
                    address_street = request.Street
                };

                UoWConsumer.CustomerRepository.Insert(newCustomer);

                //Generar el link para cancelar el envio de correo
                Guid id = Guid.NewGuid();
                var cancelEmail = new so_portal_links_log
                {
                    CustomerAdditionalDataId = newCustomer.customerId,
                    CreatedDate = DateTime.Today,
                    Id = id,
                    LimitDays = 0,
                    Status = (int)PortalLinks.STATUS.PENDING,
                    Type = (int)PortalLinks.TYPE.EMAIL_DEACTIVATION
                };
                string cancelEmailURL = ConfigurationManager.AppSettings["ApiV2Url"] + "" + id;
                //Se envia el Correo

                //Se notifica al CMR

                UoWConsumer.CustomerAdditionalDataRepository.Insert(newCustomerAdditionalData);
                UoWConsumer.CustomerDataRepository.Insert(newCustomerData);
                UoWConsumer.Save();
                return ResponseBase<InsertConsumerResponse>.Create(new InsertConsumerResponse()
                {
                    Msg = "Se guardo con exito"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<InsertConsumerResponse>
                    .Create(new List<string> { e.Message });
            }
            
        }

        public ResponseBase<UpdateConsumerResponse> UpdateConsumer(UpdateConsumerRequest request)
        {
            try
            {
                var updateCustomer = UoWConsumer.CustomerRepository.GetByID(request.CustomerId);

                updateCustomer.name = request.Name ?? updateCustomer.name;
                updateCustomer.email = request.Email ?? updateCustomer.email;
                updateCustomer.latitude = request.Latitude ?? updateCustomer.latitude;
                updateCustomer.longitude = request.Longitude ?? updateCustomer.longitude;
                updateCustomer.modifiedon = DateTime.Now;
                updateCustomer.modifiedby = request.UserId;

                var updateCustomerAdditionalData = updateCustomer.Consumers
                    .Where(x => x.Status == (int)Consumer.STATUS.CONSUMER)
                    .FirstOrDefault();


                updateCustomerAdditionalData.Email_2 = request.Email_2;
                updateCustomerAdditionalData.Phone = request.Phone;
                updateCustomerAdditionalData.Phone_2 = request.Phone_2;
                updateCustomerAdditionalData.CFECode = request.CFECode;
                updateCustomerAdditionalData.CodePlace = request.CodePlace;
                updateCustomerAdditionalData.ReferenceCode = request.ReferenceCode;
                updateCustomerAdditionalData.InteriorNumber = request.InteriorNumber;
                updateCustomerAdditionalData.Neighborhood = request.Neighborhood;

                var updateCustomerData = updateCustomer.so_customer_data
                    .Where(x => x.status)
                    .FirstOrDefault();

                updateCustomerData.address_number = request.ExternalNumber;
                updateCustomerData.address_number_cross1 = request.Crossroads;
                updateCustomerData.address_number_cross2 = request.Crossroads_2;
                updateCustomerData.address_street = request.Street;

                UoWConsumer.CustomerRepository.Update(updateCustomer);
                UoWConsumer.CustomerDataRepository.Update(updateCustomerData);
                UoWConsumer.CustomerAdditionalDataRepository.Update(updateCustomerAdditionalData);

                UoWConsumer.Save();

                //Notificar al CRM

                return ResponseBase<UpdateConsumerResponse>.Create(new UpdateConsumerResponse
                {
                    Msg = "Se ha actualizado con exito"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<UpdateConsumerResponse>.Create(new List<string>
                {
                    e.Message
                });
            }
        }

        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }
    }
}