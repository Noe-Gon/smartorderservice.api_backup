using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class InvoiceDataDto
    {
        public string CustomerRFC { set; get; }
        public string BussinessName { set; get; }
        public string BranchCode { set; get; }
        public string RouteCode { set; get; }
        public string UseCFDI { set; get; }
        public string DescriptionUseCFDI { set; get; }
        public bool IsBillableByWBC { set; get; }
        public string Codope { set; get; }
        public string Address { set; get; }
        public bool CreditApply { set; get; }
        public string SiteId { set; get; }
        public string InventSiteId { set; get; }
        public string SalesResponsible { set; get; }

    }
}