using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public partial class so_invoice_opefactura
    {
        [Key]
        public int invoiceId { set; get; }

        public int saleId { set; get; }

        public int billing_dataId { set; get; }

        public string opecdid { set; get; }

        public string serie { set; get; }

        public int folio { set; get; }

        public int folio_interno { set; get; }

        public int tipo_mensaje { set; get; }

        public string mensaje { set; get; }

        public double total { set; get; }

        public DateTime? createdon { set; get; }

        public int createdby { set; get; }

        public DateTime? modifiedon { set; get; }

        public int modifiedby { set; get; }

        public bool status { set; get; }

        public virtual so_sale so_sale { get; set; }

        public virtual so_billing_data so_billing_data { get; set; }
    }
}