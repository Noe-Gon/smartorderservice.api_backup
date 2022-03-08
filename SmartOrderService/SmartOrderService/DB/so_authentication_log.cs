using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_authentication_log
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_code")]
        [MaxLength(10)]
        public string UserCode { get; set; }

        [Column("leader_code")]
        [MaxLength(50)]
        public string LeaderCode { get; set; }

        [Column("was_leader_code_authorization")]
        public bool WasLeaderCodeAuthorization { get; set; }

        #region Relations

        [Column("leaderAuthenticationCodeId")]
        public int? LeaderAuthenticationCodeId { get; set; }
        public virtual so_leader_authorization_code LeaderAuthorizationCode { get; set; }

        [Column("routeId")]
        public int? RouteId { get; set; }
        public virtual so_route Route { get; set; }

        [Column("userId")]
        public int? UserId { get; set; }
        public virtual so_user User { get; set; }

        #endregion

        #region Audit

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("modified_date")]
        public DateTime? ModifiedDate { get; set; }

        [Column("status")]
        public bool Status { get; set; }

        #endregion
    }
}