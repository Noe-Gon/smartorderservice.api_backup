using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Message
{
    #region Crew Ope20 Register
    public class CrewOpe20Request
    {
        [JsonProperty("supervisor")]
        public Supervisor Supervisor { get; set; }

        [JsonProperty("salesMan")]
        public User SalesMan { get; set; }

        [JsonProperty("assistant1")]
        public User Assistant1 { get; set; }

        [JsonProperty("assistant2")]
        public User Assistant2 { get; set; }

        [JsonProperty("assistant3")]
        public User Assistant3 { get; set; }

        [JsonProperty("assistant4")]
        public User Assistant4 { get; set; }

        [JsonProperty("businessUnitId")]
        public int? BusinessUnitId { get; set; }

        [JsonProperty("operationDate")]
        public DateTime OperationDate { get; set; }

        [JsonProperty("routeCode")]
        public string RouteCode { get; set; }

        [JsonProperty("posId")]
        public string PosId { get; set; }
    }

    public class Supervisor
    {
        [JsonProperty("collaboratorCode")]
        public int CollaboratorCode { get; set; }
    }

    public class User
    {
        [JsonProperty("collaboratorCode")]
        public string CollaboratorCode { get; set; }

        [JsonProperty("supplier")]
        public Supplier Supplier { get; set; }
    }

    public class Supplier
    {
        [JsonProperty("collaboratorCode")]
        public int CollaboratorCode { get; set; }

        [JsonProperty("supplierDateBegin")]
        public DateTime SupplierDateBegin { get; set; }

        [JsonProperty("supplierDateEnd")]
        public DateTime SupplierDateEnd { get; set; }
    }
    #endregion

    #region DistributionCenter Ope 20
    public class DistributionCenterResponse
    {
        [JsonProperty("cedisNumber")]
        public int CedisNumber { get; set; }

        [JsonProperty("branch")]
        public int Branch { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

    }
    #endregion

    #region Close Route Notification
    public class CloseRouteNotificationRequest
    {
        [JsonProperty("routeCode")]
        public string RouteCode { get; set; }

        [JsonProperty("posID")]
        public string BranchCode { get; set; }

        [JsonProperty("loadID")]
        public string LoadID { get; set; }
    }

    public class CloseRouteNotificationResponse
    {
        [JsonProperty("errorCode")]
        public int ErrorCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("affectedSales")]
        public int AffectedSaled { get; set; }
    }

    public class Ope20ErrrorResponse
    {
        public object errors { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public int status { get; set; }
        public string traceId { get; set; }
    }
    #endregion
}