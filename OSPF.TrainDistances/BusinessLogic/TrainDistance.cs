using System.Collections.Generic;
using System.Linq;
using OSPF.TrainDistances.BusinessLogic.Interfaces;
using OSPF.TrainDistances.Models;

namespace OSPF.TrainDistances.BusinessLogic
{ 
    public class TrainDistance : ITrainDistance
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
        
        public Dictionary<int, List<TrainStations>> StationMapping;

        public string MaxDistance = string.Empty; 

        public RoutesView CalculateEndToEnd(List<char> routes, string maxDistance)
        {
            int index = 0;
            bool startPoint = false;
            List<TrainStations> mappings = new List<TrainStations>();
            MaxDistance = maxDistance;
            int isCorrectFormatNumber;
            bool isDistanceNumber = int.TryParse(MaxDistance, out isCorrectFormatNumber);
            StationMapping = new Dictionary<int, List<TrainStations>>();
            do
            {
                index++;
                for (int stationDistanceCount = 0; stationDistanceCount < StationsDistance.Count; stationDistanceCount++)
                {
                    var stationCode = StationsDistance[stationDistanceCount].Station.ToCharArray();
                    
                    if (!startPoint && stationCode[0] == routes[0]) {
                        //CaptureFirstStation
                        if (!StationMapping.Any() || StationMapping.Any() &&
                            StationMapping.Last().Value.First().Station != StationsDistance[stationDistanceCount].Station)
                        {
                            startPoint = true;

                            //return in trainstation objecct to mappings
                            mappings.Add(new TrainStations
                            {
                                Station = StationsDistance[stationDistanceCount].Station,
                                Distance = StationsDistance[stationDistanceCount].Distance
                            });
                            stationDistanceCount = -1;
                        }
                    }
                    else if (startPoint && mappings.Any() && stationCode[0] == mappings.Last().Station.ToCharArray()[1] && stationCode[1] == routes[1])
                    {
                        //CheckMappingForExisiting
                        int foundIndex = -1;
                        foreach (var stationMap in StationMapping.Values)
                        {
                            foundIndex = stationMap.Select(i => i.Station).ToList()
                                        .IndexOf(StationsDistance[stationDistanceCount].Station);
                        }

                        if (foundIndex != -1)
                        {
                            mappings.Add(new TrainStations
                            {
                                Station = StationsDistance[stationDistanceCount].Station,
                                Distance = StationsDistance[stationDistanceCount].Distance

                            });
                            break;
                        }

                        if ( StationMapping.Values.Any(
                                y => y.Any(o => o.Station == StationsDistance[stationDistanceCount].Station)) 
                                    )
                        {
                            continue;
                        }

                        
                        //return in trainstation objecct to mappings
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

                if (startPoint && mappings.Any() && mappings.Last().Station.ToCharArray()[1] == routes[1])
                {
                    if (!StationMapping.Any() || !CheckDistinct(StationMapping.Values.SelectMany(train => train).ToList(), mappings))
                    {
                        StationMapping.Add(index, mappings);
                        mappings = new List<TrainStations>();
                    }
                }

                startPoint = false;
            } while ( index < StationsDistance.Count);
            
            var completeRoutes = MergeAdditionalRoutes(StationMapping, isCorrectFormatNumber).Where(md => md.Distance < isCorrectFormatNumber && isDistanceNumber);

            return completeRoutes.Any() ? new RoutesView
            {
                DifferentRoutes = completeRoutes.Select(cr =>  cr.Station + " " + cr.Distance ).ToList(),
                Distance = completeRoutes.Min(cr => cr.Distance),
                RoutesCount = completeRoutes.Count(),
                ShortestRoute = completeRoutes.OrderBy(cr => cr.Distance).First().Distance
            } : new RoutesView();
        }

        public bool CheckDistinct(List<TrainStations> stationMappings, List<TrainStations> mappings, bool isSingle = false)
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

            return false;
        }

        public bool CheckDistinctOnSingle(List<TrainStations> stationMappings, List<TrainStations> mappings)
        {
            return CheckDistinct(stationMappings, mappings, true);
        }

        public List<TrainStations> AggregateStationsToRoute(Dictionary<int, List<TrainStations>> mapping)
        {
            var stationMapping = new Dictionary<int, List<TrainStations>>();

            int count = 0;
            stationMapping.Add(count, new List<TrainStations>());
            foreach (var stat in mapping.Values)
            {
                TrainStationsAggregate aggregate = new TrainStationsAggregate(stat);
                List<TrainStations> temp = new List<TrainStations>
                {
                    new TrainStations {
                        Station = aggregate.Route,
                        Distance = aggregate.RouteDistance
                    }
                };

                if (!CheckDistinctOnSingle(stationMapping.Values.SelectMany(t => t).ToList(), temp))
                {
                    stationMapping.Values.First().Add(temp.First());
                }
            }

            return stationMapping.Values.First();
        }
         
        public List<TrainStations> MergeAdditionalRoutes(Dictionary<int, List<TrainStations>> stationMapping, int maxDistance)
        {
            string stationRoute = string.Empty;
            string invertStationRoute = string.Empty;
            int distance = 0;
            bool started = false;
            bool completed = false;
            List<TrainStations> trains = new List<TrainStations>();
            List<TrainStations> groupedStations = new List<TrainStations>();
            
            for (int stat = 0; stat < stationMapping.Values.Count; stat++)
            {
                
                TrainStationsAggregate aggregate = new TrainStationsAggregate(stationMapping.Values.ElementAt(stat));
                if (distance == 0 && started == false)
                {
                    started = true;
                    stationRoute = aggregate.Route;
                    distance = aggregate.RouteDistance;
                    stat = -1;
                    continue;
                }

                
                if (aggregate.RouteDistance <= maxDistance / 3)
                {
                    //CaptureRoutesLessThanThirdOfMax
                    stationRoute = aggregate.Route + aggregate.Route + aggregate.Route;
                    distance = aggregate.RouteDistance + aggregate.RouteDistance + aggregate.RouteDistance;

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

                if (aggregate.RouteDistance <= maxDistance / 2 && started)
                {
                    //CaptureRoutesLessThanHalfOfMax
                    stationRoute = aggregate.Route + aggregate.Route;
                    distance = aggregate.RouteDistance + aggregate.RouteDistance;
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

                if ((distance + aggregate.RouteDistance) <= maxDistance)
                {
                    //CaptureRoutesLessThanMax
                    var temp = stationRoute;
                    stationRoute = aggregate.Route + temp;
                    invertStationRoute = temp + aggregate.Route;
                    distance = aggregate.RouteDistance + distance;
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
                        stationRoute = trains.First().Station + aggregate.Route;
                        invertStationRoute = aggregate.Route + trains.First().Station;
                        distance = trains.First().Distance + aggregate.RouteDistance;
                        trains.First().Merged = true;
                    }


                    if (trains.First().Merged && !CheckDistinctOnSingle(groupedStations, trains)
                        && distance < maxDistance)
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
                        TrainStationsAggregate aggregateMap = new TrainStationsAggregate(stationMapping.Values.ElementAt(statMap));
                        if ((aggregate.RouteDistance + aggregateMap.RouteDistance) <= maxDistance)
                        {
                            var temp = stationRoute;
                            stationRoute = aggregateMap.Route + aggregate.Route;
                            invertStationRoute = aggregate.Route + aggregateMap.Route;
                            distance = aggregateMap.RouteDistance + aggregate.RouteDistance;
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
                                && distance < maxDistance)
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
            if (StationMapping != null)
            {
                stationMapping = new Dictionary<int, List<TrainStations>>
                {
                    {0, AggregateStationsToRoute(StationMapping)}
                };
            }
            stationMapping.Values.First().AddRange(groupedStations);
            return stationMapping.Values.First();
        }

        public RoutesView CalculateStationByStation(List<char> routes)
        {
            bool routeFound = false;
            RoutesView routesDistanceView = new RoutesView();
            List<string> routesView = new List<string>();
            for (int userEntry = 0; userEntry < routes.Count - 1; userEntry++)
            {
                foreach (var stationDistance in StationsDistance)
                {
                    var stationCode = stationDistance.Station.ToCharArray();
                    if (stationCode[0] == routes[userEntry] && stationCode[1] == routes[userEntry + 1])
                    {
                        routeFound = true;
                        routesView.Add(stationDistance.Station);
                        routesDistanceView.Distance = routesDistanceView.Distance + stationDistance.Distance;
                    }
                }

                if (routeFound == false)
                {
                    return new RoutesView { Distance = -1 };                    
                }

                routeFound = false;
            }
            routesDistanceView.DifferentRoutes = routesView;
            return routesDistanceView;
        }
    }
}
