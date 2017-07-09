using System.Collections.Generic;

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
