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
                entity.attributes.Add(AttributeCrm.CreateBoolean("ope_tipocliente", true));

                //Address
                AttributeCrm.CreateEntityReferenceValidation(entity.attributes, "ope_pais", "ope_paisid", consumer.CountryId.ToString());
                AttributeCrm.CreateEntityReferenceValidation(entity.attributes, "ope_estado", "ope_estadoid", consumer.StateId.ToString());
                AttributeCrm.CreateEntityReferenceValidation(entity.attributes, "ope_municipio", "ope_municipioid", consumer.MunicipalityId.ToString());
                AttributeCrm.CreateEntityReferenceValidation(entity.attributes, "ope_colonia", "ope_coloniaid", consumer.Neighborhood.ToString());
                AttributeCrm.CreateEntityReferenceValidation(entity.attributes, "ope_rutas", "ope_rutasid", consumer.RouteCRMId.ToString());

                entity.attributes.Add(AttributeCrm.Create("ope_estadoidname", consumer.StateIdName ?? ""));
                entity.attributes.Add(AttributeCrm.Create("ope_paisidname", consumer.CountryIdName ?? ""));
                entity.attributes.Add(AttributeCrm.Create("ope_municipioidname", consumer.MunicipalityIdName ?? ""));
                entity.attributes.Add(AttributeCrm.Create("ope_coloniaidname", consumer.NeighborhoodIdName ?? ""));


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

                //Price List
                entity.attributes.Add(AttributeCrm.CreateInteger("ope_listprec", consumer.PriceListId));


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

        public OrganizationServiceProxy GetService()
        {
            string url = ConfigurationManager.AppSettings["Discovery_CRM_URL"];
            string organization = ConfigurationManager.AppSettings["OrganizacionCRM"];
            var serviceManagement = ServiceConfigurationFactory.CreateManagement<IDiscoveryService>(new Uri(url));
            var endpointType = serviceManagement.AuthenticationType;
            var authCredentials = GetCredentials(endpointType);
            var organizationUri = String.Empty;
            using (var discoveryProxy = GetProxy<IDiscoveryService, DiscoveryServiceProxy>(serviceManagement, authCredentials))
            {
                if (discoveryProxy != null)
                {
                    var orgs = DiscoverOrganizations(discoveryProxy);
                    var orgDetails = orgs.ToArray();
                    if (String.IsNullOrWhiteSpace(organization))
                        throw new ArgumentNullException("orgUniqueName");
                    if (orgDetails == null)
                        throw new ArgumentNullException("orgDetails");
                    OrganizationDetail orgDetail = null;
                    foreach (OrganizationDetail detail in orgDetails)
                    {
                        if (String.Compare(detail.UniqueName, organization, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            orgDetail = detail;
                            break;
                        }
                    }
                    organizationUri = orgDetail.Endpoints[EndpointType.OrganizationService];
                }
            }
            var orgServiceManagement = ServiceConfigurationFactory.CreateManagement<IOrganizationService>(new Uri(organizationUri));
            var credentials = GetCredentials(endpointType);
            var organizationProxy = GetProxy<IOrganizationService, OrganizationServiceProxy>(orgServiceManagement, credentials);
            organizationProxy.EnableProxyTypes();
            return organizationProxy;
        }

        private TProxy GetProxy<TService, TProxy>(IServiceManagement<TService> serviceManagement, AuthenticationCredentials authCredentials) where TService : class where TProxy : ServiceProxy<TService>
        {
            var classType = typeof(TProxy);
            if (serviceManagement.AuthenticationType != AuthenticationProviderType.ActiveDirectory)
            {
                var tokenCredentials = serviceManagement.Authenticate(authCredentials);
                return (TProxy)classType
                    .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(SecurityTokenResponse) })
                    .Invoke(new object[] { serviceManagement, tokenCredentials.SecurityTokenResponse });
            }
            return (TProxy)classType
                .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(ClientCredentials) })
                .Invoke(new object[] { serviceManagement, authCredentials.ClientCredentials });
        }

        private AuthenticationCredentials GetCredentials(AuthenticationProviderType endpointType)
        {
            string user = ConfigurationManager.AppSettings["UsuarioCRM"];
            string domain = "";//ConfigurationManager.AppSettings["DomainCRM"];
            string password = ConfigurationManager.AppSettings["PasswordCRM"];
            var authCredentials = new AuthenticationCredentials();
            switch (endpointType)
            {
                case AuthenticationProviderType.ActiveDirectory:
                    authCredentials.ClientCredentials.Windows.ClientCredential =
                        new System.Net.NetworkCredential(user, password, domain);
                    break;
                case AuthenticationProviderType.LiveId:
                    /*
                    authCredentials.ClientCredentials.UserName.UserName = connectionData.user;
                    authCredentials.ClientCredentials.UserName.Password = connectionData.password;
                    authCredentials.SupportingCredentials = new AuthenticationCredentials();
                    authCredentials.SupportingCredentials.ClientCredentials =
                        dalTeleventa.DeviceIdManager.LoadOrRegisterDevice();*/
                    break;
                default:
                    authCredentials.ClientCredentials.UserName.UserName = user;
                    authCredentials.ClientCredentials.UserName.Password = password;
                    break;
            }
            return authCredentials;
        }

        public OrganizationDetailCollection DiscoverOrganizations(IDiscoveryService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            RetrieveOrganizationsRequest orgRequest = new RetrieveOrganizationsRequest();
            RetrieveOrganizationsResponse orgResponse =
                (RetrieveOrganizationsResponse)service.Execute(orgRequest);
            return orgResponse.Details;
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