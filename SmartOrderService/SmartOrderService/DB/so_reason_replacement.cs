namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_reason_replacement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_reason_replacement()
        {
            so_branch_reason_replacement = new HashSet<so_branch_reason_replacement>();
        }

        [Key]
        public int reasonId { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string description { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_branch_reason_replacement> so_branch_reason_replacement { get; set; }
    }
}
