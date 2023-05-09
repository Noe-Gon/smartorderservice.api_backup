using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class CRMBase
    {
        public string entityName { get; set; }
        public Guid? entityId { get; set; }
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

        public static AttributeCrm CreateInteger(string name, int value) => new AttributeCrm
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

        public static AttributeCrm CreateEntityReference(string entityReferenceName, string name, string value) => new AttributeCrm
        {
            isDateTime = false,
            isOptionSet = false,
            entityReferenceName = entityReferenceName,
            name = name,
            value = value
        };

        public static void CreateEntityReferenceValidation(List<AttributeCrm> list, string entityReferenceName, string name, string value)
        {
            if (value != null && value != "")
            {
                list.Add(CreateEntityReference(entityReferenceName, name, value));
            }
        }

        public static void CreateAttribute<T>(List<AttributeCrm> list, string name, T value)
        {
            if (value == null)
                return;

            if (value is string)
                list.Add(Create(name, value.ToString()));
        }
    }
}