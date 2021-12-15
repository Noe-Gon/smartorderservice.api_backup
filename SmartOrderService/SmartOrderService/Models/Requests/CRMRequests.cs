using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class CRMBase
    {
        public string entityName { get; set; }
        public string entityId { get; set; }
        public Object referenceId { get; set; }
        public List<AttributeCrm> attributes { get; set; }

    }

    public class AttributeCrm
    {
        public bool isDateTime { get; set; }
        public bool isOptionSet { get; set; }
        public string entityReferenceName { get; set; }
        public string name { get; set; }
        public object value { get; set; }

        public static AttributeCrm Create(string name, string value) => new AttributeCrm
        {
            isDateTime = false,
            isOptionSet = false,
            entityReferenceName = null,
            name = name,
            value = value
        };

        public static AttributeCrm CreateDateTime(string name, string value) => new AttributeCrm
        {
            isDateTime = true,
            isOptionSet = false,
            entityReferenceName = null,
            name = name,
            value = value
        };

        public static AttributeCrm CreateBoolean(string name, bool value) => new AttributeCrm
        {
            isDateTime = true,
            isOptionSet = false,
            entityReferenceName = null,
            name = name,
            value = value
        };
    }
}