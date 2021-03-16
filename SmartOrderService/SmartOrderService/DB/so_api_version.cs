using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace SmartOrderService.DB
{
    public partial class so_api_version
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_api_version() {

        }
        [Key]
        public int userId { get; set; }
        public int applicationId{   get; set;  }
        public string url { get; set; }
        public DateTime? createdOn { get; set; }
	    public bool status{ get; set; }
    }
}