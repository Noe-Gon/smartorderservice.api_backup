using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class CustomerDataDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Ftr { get; set; }
        public string BusinessName { get; set; }
        public string FiscalAddress { get; set; }
        public bool Status { get; set; }
    }
}