using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Message
{

    #region Sale Syncs

    public class SaleSyncsRequest
    {
        public string BranchCode { get; set; }
        public string RouteCode { get; set; }
        public DateTime Date { get; set; }
    }

    public class SaleSyncsApiRequest
    {
        public string SaleState { get; set; }
        public string branchId { get; set; }
        public string routeId { get; set; }
        public string date { get; set; }
    }

    public class SaleSyncsApiResponse
    {
        public string executionId { get; set; }
        public DateTime startDate { get; set; }
    }
    #endregion

    #region Get Sale Syncs Status
    public class GetSaleSyncStatusRequest
    {
        public string executionIdAws { get; set; }
    }

    public class GetSaleSyncStatusResponse
    {
        public DateTime startDate { get; set; }
        public DateTime stopDate { get; set; }
        public string status { get; set; }
        public string output { get; set; }
        public string input { get; set; }
        public string name { get; set; }
    }
    #endregion
}