namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_device
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_device()
        {
            so_work_day = new HashSet<so_work_day>();
        }

        [Key]
        public int deviceId { get; set; }

        [StringLength(50)]
        public string code { get; set; }

        public int userId { get; set; }

        public Guid token { get; set; }

        public DateTime? last_sync { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_work_day> so_work_day { get; set; }

        public virtual so_user so_user { get; set; }
    }
}
