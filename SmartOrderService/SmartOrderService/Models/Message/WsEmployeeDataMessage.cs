using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Message
{
    public class SingleEmployee
    {
        public List<Employee> employee { get; set; }
    }

    public class Employee 
    {
        public string division { get; set; }
        public string idcia { get; set; }
        public string nombrecia { get; set; }
        public string zona { get; set; }
        public string idsuc { get; set; }
        public string nombresuc { get; set; }
        public string iddepto { get; set; }
        public string nombredepto { get; set; }
        public string puesto { get; set; }
        public string codemp { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string activo { get; set; }
        public string ciudad { get; set; }
        public string fechanacimiento { get; set; }
        public string genero { get; set; }
        public string imss { get; set; }
        public string rfc { get; set; }
        public string curp { get; set; }
        public string proceso { get; set; }
        public string telefono { get; set; }
        public string tiposangre { get; set; }
        public string idtiponomina { get; set; }
        public string nombretiponomina { get; set; }
        public string antig { get; set; }
    }
}