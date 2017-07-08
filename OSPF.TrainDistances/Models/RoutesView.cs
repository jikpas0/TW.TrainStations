using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSPF.TrainDistances.Models
{
    public class RoutesView
    {
        public int Distance { get; set; }
        public int RoutesCount { get; set; }
        public int ShortestRoute { get; set; }
        public List<string> DifferentRoutes { get; set; }
    }
}
