using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OSPF.TrainDistances.Models
{
    public class TrainDistanceProcessor
    {
        //AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7
        public List<TrainStations> StationsDistance = new List<TrainStations>()
        {
            new TrainStations { Station = "AB", Distance = 5 },
            new TrainStations { Station = "BC", Distance = 4 },
            new TrainStations { Station = "CD", Distance = 8 },
            new TrainStations { Station = "DC", Distance = 8 },
            new TrainStations { Station = "DE", Distance = 6 },
            new TrainStations { Station = "AD", Distance = 5 },
            new TrainStations { Station = "CE", Distance = 2 },
            new TrainStations { Station = "EB", Distance = 3 },
            new TrainStations { Station = "AE", Distance = 7 }
        };

        //public Dictionary<int, Dictionary<string, int>> StationMapping = new Dictionary<int, Dictionary<string, int>>();
        public Dictionary<int, List<TrainStations>> StationMapping = new Dictionary<int, List<TrainStations>>();  

        public Dictionary<int, List<TrainStations>> CalculateEndToEnd(List<char> Routes)
        {
            int distance = 0;
            int x = 0;
            bool startPoint = false;
            List<TrainStations> mappings = new List<TrainStations>();
            do
            {
                x++;
                for (int stationDistanceCount = 0; stationDistanceCount < StationsDistance.Count; stationDistanceCount++)
                {
                    var stationCode = StationsDistance[stationDistanceCount].Station.ToCharArray();
                    
                    if (!startPoint && stationCode[0] == Routes[0]) {
                        if (!StationMapping.Any() || StationMapping.Any() &&
                            StationMapping.Last().Value.First().Station != StationsDistance[stationDistanceCount].Station)
                        {
                            startPoint = true;
                           // distance = distance + StationsDistance[stationDistanceCount].Distance;
                            mappings.Add(new TrainStations
                            {
                                Station = StationsDistance[stationDistanceCount].Station,
                                Distance = StationsDistance[stationDistanceCount].Distance
                            }
                        );
                            stationDistanceCount = -1;
                             
                        }
                    }
                    else if (startPoint && mappings.Any() && stationCode[0] == mappings.Last().Station.ToCharArray()[1] && stationCode[1] == Routes[1])
                    {
                        int test7 = -1;
                        foreach (var stationMap in StationMapping.Values)
                        {
                            List<String> test5 = stationMap.Select(i => i.Station)
                            .ToList();
                            test7 = test5.IndexOf(StationsDistance[stationDistanceCount].Station);

                        }

                        if (test7 != -1 
                            /*mappings[test7].Station !=
                            StationsDistance[stationDistanceCount].Station*/)
                        {
                            mappings.Add(new TrainStations
                            {
                                Station = StationsDistance[stationDistanceCount].Station,
                                Distance = StationsDistance[stationDistanceCount].Distance

                            });
                            break;
                        }
                        if ( //found &&
                            StationMapping.Values.Any(
                                y => y.Any(o => o.Station == StationsDistance[stationDistanceCount].Station)) 
                                    )
                        {
                            continue;
                        }
                        mappings.Add(new TrainStations
                        {
                            Station = StationsDistance[stationDistanceCount].Station,
                            Distance = StationsDistance[stationDistanceCount].Distance

                        });
                        break;
                    }
                    else if (startPoint && mappings.Any() && stationCode[0] == mappings.Last().Station.ToCharArray()[1] && 
                        mappings.All(q => q.Station != StationsDistance[stationDistanceCount].Station))
                    {
                        mappings.Add(new TrainStations
                        {
                            Station = StationsDistance[stationDistanceCount].Station,
                            Distance = StationsDistance[stationDistanceCount].Distance
                        });
                        stationDistanceCount = -1;
                    } 
                }
                if (startPoint && mappings.Any() && mappings.Last().Station.ToCharArray()[1] == Routes[1])
                {
                    if (!StationMapping.Any() || !CheckDistinct(StationMapping.Values.SelectMany(train => train).ToList(), mappings))
                    {
                        StationMapping.Add(x, mappings);
                        mappings = new List<TrainStations>();
                    }
                }
                startPoint = false;


            } while ( x < StationsDistance.Count);
            //TrainStationsDetails details = new TrainStationsDetails(stationMapping.Values.ElementAt(stat));
            
            StationMapping = MergeAdditionalRoutes(StationMapping);
            return StationMapping;
        }

        private bool CheckDistinct(List<TrainStations> stationMappings, List<TrainStations> mappings, bool isSingle = false)
        {
            foreach (var stationMapping in stationMappings)
            {
                if (stationMappings.Count == mappings.Count)
                {
                    for (int map = 0; map < mappings.Count; map++)
                    {
                        if (stationMappings[map].Distance == mappings[map].Distance &&
                            stationMappings[map].Station == mappings[map].Station)
                        {
                            return true;
                        }
                    }
                }

                if (isSingle && stationMappings.Any(sm => sm.Distance == mappings.First().Distance 
                    && sm.Station == mappings.First().Station))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckDistinctOnSingle(List<TrainStations> stationMappings, List<TrainStations> mappings)
        {
            return CheckDistinct(stationMappings, mappings, true);
        }

        private Dictionary<int, List<TrainStations>> AggregateStationsToRoute()
        {
            var stationMapping = new Dictionary<int, List<TrainStations>>();

            int count = 0;
            stationMapping.Add(count, new List<TrainStations>
            {

            });
            foreach (var stat in StationMapping.Values)
            {
                TrainStationsDetails details = new TrainStationsDetails(stat);
                List<TrainStations> temp = new List<TrainStations>
                {
                    new TrainStations {
                        Station = details.Route,
                        Distance = details.RouteDistance
                    }
                };

                if (!CheckDistinctOnSingle(stationMapping.Values.SelectMany(t => t).ToList(), temp))
                {
                    stationMapping.Values.First().Add(temp.First());
                }
            }

            return stationMapping;
        }

        //refactor into smaller methods 
        public Dictionary<int, List<TrainStations>> MergeAdditionalRoutes(Dictionary<int, List<TrainStations>> stationMapping)
        {
            string stationRoute = string.Empty;
            string invertStationRoute = string.Empty;
            int distance = 0;
            int count = 0;
            List<TrainStations> trains = new List<TrainStations>();
            bool started = false;
            bool completed = false;
            List<TrainStations> groupedStations = new List<TrainStations>();

            
            //foreach (var stationMap in stationMapping.Values)
            for (int stat = 0; stat < stationMapping.Values.Count; stat++)
            {
                
                TrainStationsDetails details = new TrainStationsDetails(stationMapping.Values.ElementAt(stat));
                if (distance == 0 && started == false)
                {
                    started = true;
                    stationRoute = details.Route;
                    distance = details.RouteDistance;
                    stat = -1;
                    continue;
                }
                if (details.RouteDistance <= 10)
                {
                    
                    stationRoute = details.Route + details.Route + details.Route;
                    distance = details.RouteDistance + details.RouteDistance + details.RouteDistance;

                    trains = new List<TrainStations>
                    {
                        new TrainStations { 
                            Station = stationRoute,
                            Distance = distance
                        }
                    };
                    if (!CheckDistinctOnSingle(groupedStations, trains))
                    {
                        groupedStations.Add(new TrainStations
                        {
                            Station = stationRoute,
                            Distance = distance
                        });
                    }

                    stationRoute = string.Empty;
                    distance = 0;
                    trains = new List<TrainStations>();
                }

                if (details.RouteDistance <= 15 && started)
                {
                    
                    stationRoute = details.Route + details.Route;
                    distance = details.RouteDistance + details.RouteDistance;
                    trains = new List<TrainStations>
                    {
                        new TrainStations {
                            Station = stationRoute,
                            Distance = distance
                        }
                    };
                    if (!CheckDistinctOnSingle(groupedStations, trains))
                    {
                        groupedStations.Add(new TrainStations
                        {
                            Station = stationRoute,
                            Distance = distance
                        });
                    }
                    stationRoute = string.Empty;
                    distance = 0;
                    trains = new List<TrainStations>();
                }

                if ((distance + details.RouteDistance) <= 30)
                {

                    var temp = stationRoute;
                    stationRoute = details.Route + temp;
                    invertStationRoute = temp + details.Route;
                    distance = details.RouteDistance + distance;
                    if (!trains.Any())
                    {
                        trains = new List<TrainStations>
                        {
                            new TrainStations
                            {
                                Station = stationRoute,
                                Distance = distance,
                                Merged = false
                            }
                        };
                    }
                    else
                    {
                        stationRoute = trains.First().Station + details.Route;
                        invertStationRoute = details.Route + trains.First().Station;
                        distance = trains.First().Distance + details.RouteDistance;
                        trains.First().Merged = true;
                    }


                    if (trains.First().Merged && !CheckDistinctOnSingle(groupedStations, trains)
                        && distance < 30)
                    {
                        groupedStations.Add(new TrainStations
                        {
                            Station = stationRoute,
                            Distance = distance
                        });

                        groupedStations.Add(new TrainStations
                        {
                            Station = invertStationRoute,
                            Distance = distance
                        });
                        trains = new List<TrainStations>();
                        if (completed)
                        {
                            break;
                        }
                        if (stat++ == stationMapping.Values.Count)
                        {
                            stat = -1;
                            completed = true;
                        }
                    }


                    stationRoute = string.Empty;
                    distance = 0;
                    invertStationRoute = string.Empty;
                }
                else
                {
                    for (int statMap = 0; statMap < stationMapping.Values.Count; statMap++)
                    {
                        TrainStationsDetails detailsMap = new TrainStationsDetails(stationMapping.Values.ElementAt(statMap));
                        if ((details.RouteDistance + detailsMap.RouteDistance) <= 30)
                        {
                            var temp = stationRoute;
                            stationRoute = detailsMap.Route + details.Route;
                            invertStationRoute = details.Route + detailsMap.Route;
                            distance = detailsMap.RouteDistance + details.RouteDistance;
                            if (!trains.Any())
                            {
                                trains = new List<TrainStations>
                                {
                                    new TrainStations
                                    {
                                        Station = stationRoute,
                                        Distance = distance,
                                        Merged = true
                                    }
                                };
                            }
                            

                            if (trains.First().Merged && !CheckDistinctOnSingle(groupedStations, trains)
                                && distance < 30)
                            {
                                groupedStations.Add(new TrainStations
                                {
                                    Station = stationRoute,
                                    Distance = distance
                                });
                                groupedStations.Add(new TrainStations
                                {
                                    Station = invertStationRoute,
                                    Distance = distance
                                });

                                stationRoute = string.Empty;
                                distance = 0;
                                invertStationRoute = string.Empty;

                                trains = new List<TrainStations>();
                                if (completed)
                                {
                                    break;
                                }
                                if (stat++ == stationMapping.Values.Count)
                                {
                                    stat = -1;
                                    completed = true;
                                }
                            }
                        }

                       
                    }
                }
            }

            stationMapping = AggregateStationsToRoute();
            stationMapping.Add(stationMapping.Count + 1, groupedStations);

            return stationMapping;
        }

        public int CalculateStationByStation(List<char> Routes)
        {
            int distance = 0;
            bool routeFound = false;

            for (int userEntry = 0; userEntry < Routes.Count - 1; userEntry++)
            {
                foreach (var stationDistance in StationsDistance)
                {
                    var stationCode = stationDistance.Station.ToCharArray();
                    if (stationCode[0] == Routes[userEntry] && stationCode[1] == Routes[userEntry + 1])
                    {
                        routeFound = true;
                        distance = distance + stationDistance.Distance;
                    }
                }

                if (routeFound == false)
                {
                    distance = -1;
                    break;
                }

                routeFound = false;
            }
            
            return distance;
        }
    }
}
