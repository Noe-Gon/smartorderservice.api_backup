using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public partial class so_synchronized_consumer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int synchronizedId { get; set; }
        public int registeredBy { get; set; }
        public int customerId { get; set; }
        public bool status { get; set; }
    }
}