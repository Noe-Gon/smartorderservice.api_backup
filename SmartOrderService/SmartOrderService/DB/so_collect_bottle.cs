namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_collect_bottle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_collect_bottle()
        {
            so_collect_bottle_detail = new HashSet<so_collect_bottle_detail>();
            so_reception_collect_bottle = new HashSet<so_reception_collect_bottle>();
        }

        [Key]
        public int collect_bottleId { get; set; }

        public int branchId { get; set; }

        public int customerId { get; set; }

        public int folio { get; set; }

        public DateTime loanedon { get; set; }

        public int validity { get; set; }

        public double total_cash { get; set; }

        public double new_cash { get; set; }

        public double previous_cash { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public int status { get; set; }

        public virtual so_branch so_branch { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_collect_bottle_detail> so_collect_bottle_detail { get; set; }

        public virtual so_customer so_customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_reception_collect_bottle> so_reception_collect_bottle { get; set; }
    }
}
