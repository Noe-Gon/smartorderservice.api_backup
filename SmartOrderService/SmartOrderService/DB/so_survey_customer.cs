using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public partial class so_survey_customer
    {
        [Key]
        public long surveyCustomerId { set; get; }

        public string route { set; get; }

        public int routeType { set; get; }

        public string code { set; get; }

        public string customer { set; get; }
        public string branch { set; get; }

        public string address { set; get; }
        public string data { set; get; }

        public string contactName { set; get; }

        public DateTime createdOn { set; get; }
        public int createdBy { set; get; }
        public DateTime modifiedOn { get; set; }
        public int modifiedBy { set; get; }
        public bool status { set; get; }

    }
}