namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_reception_bottle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_reception_bottle()
        {
            so_reception_bottle_detail = new HashSet<so_reception_bottle_detail>();
            so_reception_collect_bottle = new HashSet<so_reception_collect_bottle>();
        }

        [Key]
        public int reception_bottleId { get; set; }

        public int userId { get; set; }

        public int customerId { get; set; }

        [Column(TypeName = "date")]
        public DateTime date { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public int inventoryId { get; set; }

        public virtual so_customer so_customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_reception_bottle_detail> so_reception_bottle_detail { get; set; }

        public virtual so_user so_user { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_reception_collect_bottle> so_reception_collect_bottle { get; set; }
    }
}
