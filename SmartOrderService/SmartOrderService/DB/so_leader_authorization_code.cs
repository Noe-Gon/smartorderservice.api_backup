using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_leader_authorization_code
    {
        public int Id { get; set; }
        public int? LeaderId { get; set; }

        [Column("code")]
        public int Code { get; set; }

        [Column("modified_date")]
        public DateTime CreatedDate { get; set; }

        [Column("created_date")]
        public DateTime? ModifiedDate { get; set; }
    }
}