namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_process
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_process()
        {
            so_control_download = new HashSet<so_control_download>();
            so_log_errors = new HashSet<so_log_errors>();
            so_process_user = new HashSet<so_process_user>();
            so_process_branch = new HashSet<so_process_branch>();
            so_summary = new HashSet<so_summary>();
        }

        [Key]
        public int processId { get; set; }

        public int companyId { get; set; }

        public DateTime createdon { get; set; }

        public string description { get; set; }

        public int type { get; set; }

        public DateTime? start_process { get; set; }

        public DateTime? end_process { get; set; }

        public int percent { get; set; }

        public bool is_active { get; set; }

        public bool is_error { get; set; }

        public int etl_working { get; set; }

        public virtual so_company so_company { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_control_download> so_control_download { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_log_errors> so_log_errors { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_process_user> so_process_user { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_process_branch> so_process_branch { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_summary> so_summary { get; set; }
    }
}
