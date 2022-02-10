namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_delivery
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_delivery()
        {
            so_delivery_detail = new HashSet<so_delivery_detail>();
            so_delivery_promotion = new HashSet<so_delivery_promotion>();
            so_delivery_replacement = new HashSet<so_delivery_replacement>();
        }

        [Key]
        public int deliveryId { get; set; }

        public int customerId { get; set; }

        public int inventoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public int? visit_order { get; set; }

        public bool status { get; set; }

        private int _sale_note = 0;

        public int? deliveryStatusId { get; set; }

        public so_delivery_status DeliveryStatus { get; set; }

        public int? sale_note {
            set
            {
                if (value == null)
                    _sale_note = 0;
                else
                    _sale_note = (int)value;
            }
            get
            {
                
                return _sale_note;
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_delivery_detail> so_delivery_detail { get; set; }

        public virtual so_delivery_devolution so_delivery_devolution { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_delivery_promotion> so_delivery_promotion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_delivery_replacement> so_delivery_replacement { get; set; }

        public virtual so_delivery_sale so_delivery_sale { get; set; }

        public virtual so_inventory so_inventory { get; set; }

        public virtual so_customer so_customer { get; set; }
    }
}
