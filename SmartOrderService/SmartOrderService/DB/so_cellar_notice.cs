namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_cellar_notice
    {
        [Key]
        public int cellar_noticeId { get; set; }

        public int routeId { get; set; }

        public DateTime date { get; set; }

        public int userId { get; set; }

        public DateTime? createdOn { get; set; }

        public DateTime? modifiedOn { get; set; }

        public int? createdBy { get; set; }

        public int? modifiedBy { get; set; }

        public bool status { get; set; }

        public virtual so_route so_route { get; set; }

        public virtual so_user so_user { get; set; }
    }
}
