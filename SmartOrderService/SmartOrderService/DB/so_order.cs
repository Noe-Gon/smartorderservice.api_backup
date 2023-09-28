namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_order()
        {
            so_order_detail = new HashSet<so_order_detail>();
        }

        [Key]
        public int orderId { get; set; }

        public int customerId { get; set; }

        public int userId { get; set; }

        public int delivery_reference { get; set; }

        public DateTime date { get; set; }

        public DateTime? datesync { get; set; }

        public DateTime? delivery { get; set; }

        public double total_cash { get; set; }

        public double total_credit { get; set; }

        [Required]
        public string tags { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_order_detail> so_order_detail { get; set; }
    }
}
