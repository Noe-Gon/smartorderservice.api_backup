namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_user_reason_devolutions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_user_reason_devolutions()
        {
            so_user_devolutions = new HashSet<so_user_devolutions>();
        }

        [Key]
        public int user_reason_devolutionId { get; set; }

        [Required]
        [StringLength(100)]
        public string description { get; set; }

        public int value { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_user_devolutions> so_user_devolutions { get; set; }
    }
}
