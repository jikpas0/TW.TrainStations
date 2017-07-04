using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSPF.TrainDistances.Models
{
    public class TrainStations
    {
        public string Station { get; set; }
        public int Distance { get; set; }
    }

    public class TrainStationsDetails
    {
        public string Route { get; set; }
        public int RouteDistance { get; set; }
    }
}
