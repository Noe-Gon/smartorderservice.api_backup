namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_user_devolutions
    {
        [Key]
        public int user_devolutionId { get; set; }

        public int user_reason_devolutionId { get; set; }

        public int productId { get; set; }

        public int inventoryId { get; set; }

        public int amount { get; set; }

        public DateTime? createdOn { get; set; }

        public DateTime? modifiedOn { get; set; }

        public int? createdBy { get; set; }

        public int? modifiedBy { get; set; }

        public bool status { get; set; }

        public virtual so_inventory so_inventory { get; set; }

        public virtual so_product so_product { get; set; }

        public virtual so_user_reason_devolutions so_user_reason_devolutions { get; set; }
    }
}
