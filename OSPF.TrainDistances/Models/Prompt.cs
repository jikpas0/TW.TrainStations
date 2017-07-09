using System;
using System.Collections.Generic;

namespace OSPF.TrainDistances.Models
{
    public class Prompt
    {
        public string RouteType { get; set; }
        public List<char> Route { get; set; }
        public string MaxDistance { get; set; }
        public RoutesView RoutesView { get; set; }
    }
}
