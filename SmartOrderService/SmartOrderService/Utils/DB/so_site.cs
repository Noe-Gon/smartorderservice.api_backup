namespace SmartOrderService.Utils.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_site
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_site()
        {
            so_branch = new HashSet<so_branch>();
        }

        [Key]
        public int siteId { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        [Required]
        [StringLength(250)]
        public string name { get; set; }

        [Required]
        [StringLength(250)]
        public string address { get; set; }

        [Required]
        [StringLength(250)]
        public string postal_code { get; set; }

        [Required]
        [StringLength(250)]
        public string city { get; set; }

        [Required]
        [StringLength(250)]
        public string town { get; set; }

        [Required]
        [StringLength(250)]
        public string state { get; set; }

        [Required]
        [StringLength(250)]
        public string country { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_branch> so_branch { get; set; }
    }
}
