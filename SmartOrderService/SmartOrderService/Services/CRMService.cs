using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Newtonsoft.Json;
using RestSharp;
using SmartOrderService.Models.Requests;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.ServiceModel.Description;

namespace SmartOrderService.Services
{
    public class CRMService
    {
        public static string TypeCreate = "Create";
        public static string TypeUpdate = "Update";

        public Guid? ConsumerToCRM(CRMConsumerRequest consumer, string type, Method method)
        {
            try
            {
                var entity = new CRMBase
                {
                    entityName = "ope_clientes_cancun",
                    entityId = consumer.EntityId,
                    referenceId = null,
                    attributes = new List<AttributeCrm>()
                };
                //Basic
                entity.attributes.Add(AttributeCrm.Create("ope_name", consumer.Name));
                entity.attributes.Add(AttributeCrm.Create("ope_email", consumer.Email));
                entity.attributes.Add(AttributeCrm.Create("ope_telephone", consumer.Phone));
                entity.attributes.Add(AttributeCrm.Create("ope_cfe", consumer.CFECode));

                //Address
                AttributeCrm.CreateEntityReferenceValidation(entity.attributes, "ope_pais", "ope_paisid", consumer.CountryId.ToString());
                AttributeCrm.CreateEntityReferenceValidation(entity.attributes, "ope_estado", "ope_estadoid", consumer.StateId.ToString());
                AttributeCrm.CreateEntityReferenceValidation(entity.attributes, "ope_municipio", "ope_municipioid", consumer.MunicipalityId.ToString());
                AttributeCrm.CreateEntityReferenceValidation(entity.attributes, "ope_colonia", "ope_coloniaid", consumer.Neighborhood.ToString());

                entity.attributes.Add(AttributeCrm.Create("ope_numero_interior", consumer.InteriorNumber));
                entity.attributes.Add(AttributeCrm.Create("ope_numeroexterior", consumer.ExternalNumber));
                entity.attributes.Add(AttributeCrm.Create("ope_cruzamiento1", consumer.Crossroads));
                entity.attributes.Add(AttributeCrm.Create("ope_cruzamiento2", consumer.Crossroads_2));
                entity.attributes.Add(AttributeCrm.Create("ope_calle", consumer.Street));
                entity.attributes.Add(AttributeCrm.Create("ope_latitude", consumer.Latitude.ToString()));
                entity.attributes.Add(AttributeCrm.Create("ope_longitude", consumer.Longitude.ToString()));
                entity.attributes.Add(AttributeCrm.Create("ope_direccion_fisica", consumer.Address));

                //dias
                entity.attributes.Add(AttributeCrm.CreateBoolean("ope_domingo", consumer.Days.Contains(1)));
                entity.attributes.Add(AttributeCrm.CreateBoolean("ope_lunes", consumer.Days.Contains(2)));
                entity.attributes.Add(AttributeCrm.CreateBoolean("ope_martes", consumer.Days.Contains(3)));
                entity.attributes.Add(AttributeCrm.CreateBoolean("ope_miercoles", consumer.Days.Contains(4)));
                entity.attributes.Add(AttributeCrm.CreateBoolean("ope_jueves", consumer.Days.Contains(5)));
                entity.attributes.Add(AttributeCrm.CreateBoolean("ope_viernes", consumer.Days.Contains(6)));
                entity.attributes.Add(AttributeCrm.CreateBoolean("ope_sabado", consumer.Days.Contains(7)));

                entity.attributes.Add(AttributeCrm.CreateDateTime("ope_fechaalta", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

                var entityId = SendToCRM(entity, type, method);

                return entityId;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static OrganizationServiceProxy CreateService()
        {
            OrganizationServiceProxy serviceProxy = null;
            string url = ConfigurationManager.AppSettings["Discovery_CRM_URL"];
            string user = ConfigurationManager.AppSettings["_userCRM"]; //wicaamaly";
            string domain = ConfigurationManager.AppSettings["_domainCRM"]; //"bepensa";
            string password = ConfigurationManager.AppSettings["_passwordCRM"]; //"b3p3ns4*18";
            var serviceManagement = ServiceConfigurationFactory.CreateManagement<IDiscoveryService>(new Uri(url));

            switch (serviceManagement.AuthenticationType)
            {
                case AuthenticationProviderType.Federation:
                    ClientCredentials clientCredentials = new ClientCredentials();
                    clientCredentials.UserName.UserName = domain + "\\" + user;
                    clientCredentials.UserName.Password = password;
                   // serviceProxy = new OrganizationServiceProxy(serviceManagement, clientCredentials);
                    break;
                case AuthenticationProviderType.ActiveDirectory:
                    ClientCredentials credentials = new ClientCredentials();
                    credentials.Windows.ClientCredential = new NetworkCredential(user, password, domain);
                    serviceProxy = new OrganizationServiceProxy(new Uri(url), null, credentials, null);
                    break;
            }

            return serviceProxy;
        }


        public Guid? SendToCRM(Object model, string url, Method method)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["CRM_URL"]);
            var request = new RestRequest(url, method);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(model);
            var RestResponse = client.Execute(request);
            string content = RestResponse.Content;
            var jsonObject = JsonConvert.DeserializeObject<CRMResponse>(content);

            return jsonObject.newEntityId;
        }
    }

    public class CRMResponse
    {
        public int Id { get; set; }
        public string message { get; set; }
        public Guid? newEntityId { get; set; }
        public Object referenceId { get; set; }
    }
}