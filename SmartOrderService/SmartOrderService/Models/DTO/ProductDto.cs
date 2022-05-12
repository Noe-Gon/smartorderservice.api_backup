using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class ProductDto

    {
        public int ProductId;
        public string Code;
        public string Name;
        public bool IsReturnable;
        public double Factor;
        public int Pieces;
        public int CategoryId;
        public bool Status;
        public int Type;
        public int BillingDataId;
        public int BottleId;
        public string BarCode;
        public bool IsAlcohol { get; set; }
    }
}