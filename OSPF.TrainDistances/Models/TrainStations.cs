﻿using System.Collections.Generic;
using System.Linq;

namespace OSPF.TrainDistances.Models
{
    public class TrainStations
    {
        public string Station { get; set; }
        public int Distance { get; set; }
        public bool Merged { get; set; }
    }

    public class TrainStationsAggregate
    {
        public TrainStationsAggregate()
        {
        }

        public TrainStationsAggregate(List<TrainStations> trainStations)
        {
            string stationGrouped = string.Empty;
            var stations = trainStations.Select(x => x.Station).ToList();
            foreach (var station in stations)
            {
                stationGrouped = stationGrouped + station;
            }
            Route = stationGrouped;
            RouteDistance = trainStations.Sum(dist => dist.Distance);
        }

        public string Route { get; set; }
        public int RouteDistance { get; set; }
    }
}
