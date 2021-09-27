using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_leader_authorization_code
    {
        public int Id { get; set; }

        [Column("code")]
        [MaxLength(6)]
        public string Code { get; set; }

        [Column("status")]
        public bool Status { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("modified_date")]
        public DateTime? ModifiedDate { get; set; }
    }
}