namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_inventory_revisions
    {
        [Key]
        public int inventory_revisionId { get; set; }

        public int routeId { get; set; }

        public int inventoryId { get; set; }

        public int userId { get; set; }

        public DateTime date { get; set; }

        public int revision_typeId { get; set; }

        public int revision_stateId { get; set; }

        public DateTime? createdOn { get; set; }

        public DateTime? modifiedOn { get; set; }

        public int? createdBy { get; set; }

        public int? modifiedBy { get; set; }

        public bool status { get; set; }

        public virtual so_revision_states so_revision_states { get; set; }

        public virtual so_revision_types so_revision_types { get; set; }

        public virtual so_route so_route { get; set; }

        public virtual so_user so_user { get; set; }

        public virtual so_inventory so_inventory { get; set; }
    }
}
