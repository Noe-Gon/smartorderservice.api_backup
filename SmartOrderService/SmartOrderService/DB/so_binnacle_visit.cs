namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_binnacle_visit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_binnacle_visit()
        {
            so_binnacle_reason_failed = new HashSet<so_binnacle_reason_failed>();
        }

        [Key]
        public int binnacleId { get; set; }

        public int customerId { get; set; }

        public int userId { get; set; }

        public bool is_visit { get; set; }

        public bool scanned { get; set; }

        public DateTime checkin { get; set; }

        public DateTime checkout { get; set; }

        public double latitudein { get; set; }

        public double longitudein { get; set; }

        public double longitudeout { get; set; }

        public double latitudeout { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }
        public double? accuracyin { set; get; }
        public double? accuracyout { set; get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_binnacle_reason_failed> so_binnacle_reason_failed { get; set; }

        public virtual so_customer so_customer { get; set; }

        public virtual so_user so_user { get; set; }
    }
}
