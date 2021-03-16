using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace SmartOrderService.DB
{
    public partial class so_tracking
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_tracking() {

        }
        [Key]
        public int TrackingId{   get; set;  }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int Accuracy{ get; set; }
        public int Sequence{ get; set; }
        public int? CreatedBy{ get; set; }
	    public DateTime? CreatedOn{ get; set; }
	    public bool Status{ get; set; }
        public virtual so_user so_user { get; set; }
        public int LevelBattery { get; set; }
    }
}