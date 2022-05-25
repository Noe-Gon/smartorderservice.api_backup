using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_synchronized_consumer_detail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int synchronizedDetailId { get; set; }
        public int synchronizedId { get; set; }
        public int userId { get; set; }
        public bool synchronized { get; set; }
    }
}