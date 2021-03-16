using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class PointDto
    {
        public int Accuracy { get; set; }
        public string Date { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int Sequence { get; set; }
        public int LevelBattery { get; set; }
    }
}