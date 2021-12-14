using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class SaleWithPromotion
    {

        public int saleId { get; set; }
        public int userId { get; set; }
        public DateTime fecha { get; set; }
        public int customerId { get; set; }
        public string contact { get; set; }
        public string name { get; set; }
        public Boolean lsignature { get; set; }
        public String cause_no_signature { get; set; }
    }
}