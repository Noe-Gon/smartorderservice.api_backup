using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace SmartOrderService.Models.DTO
{
    [Serializable]
    public class RegisterCustomerDto
    {
       
        public string User { get; set; }
        public string Password { get; set; }
        public RegisterCustomerDataDto CustomerData { get; set; }

    }
}