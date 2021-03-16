using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpeCDLib.Models
{
    public class Visita
    {
        public int BinnacleVisitId { get; set; }
        public int BranchCode { get; set; }
        public int RouteCode { get; set; }
        public int UserCode { get; set; }
        public int CustomerCode { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Scanned { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
