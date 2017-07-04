using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSPF.TrainDistances.Models
{
    public class TrainDistanceProcessor
    {
        //AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7
        public Dictionary<string, int> StationsDistance = new Dictionary<string, int>
        {
            { "AB", 5 },
            { "BC", 4 },
            { "CD", 8 },
            { "DC", 8 },
            { "DE", 6 },
            { "AD", 5 },
            { "CE", 2 },
            { "EB", 3 },
            { "AE", 7 }
        };

        public Dictionary<int, Dictionary<string, int>> StationMapping = new Dictionary<int, Dictionary<string, int>>();  

        public Dictionary<int, Dictionary<string, int>> CalculateEndToEnd(List<char> Routes)
        {
            int distance = 0;
            int x = 0;
            bool startPoint = false;
            Dictionary<string, int> mapping = new Dictionary<string, int>();
            do
            {
                x++;
                for (int stationDistanceCount = 0; stationDistanceCount < StationsDistance.Count; stationDistanceCount++)
                {
                    var stationCode = StationsDistance.ElementAt(stationDistanceCount).Key.ToCharArray();
                    
                    if (!startPoint && stationCode[0] == Routes[0]) {
                        /*if (StationMapping.Any() &&
                            StationMapping.Last().Value.Keys.First() == StationsDistance.ElementAt(stationDistanceCount).Key)
                        {
                            continue;
                        } */
                        if (!StationMapping.Any() || StationMapping.Any() &&
                            StationMapping.Last().Value.Keys.First() != StationsDistance.ElementAt(stationDistanceCount).Key)
                        { 
                            startPoint = true;
                            distance = distance + StationsDistance.ElementAt(stationDistanceCount).Value;
                            mapping.Add(StationsDistance.ElementAt(stationDistanceCount).Key, distance);
                            stationDistanceCount = -1;
                             
                        }
                    }
                    else if (startPoint && mapping.Any() && stationCode[0] == mapping.Last().Key.ToCharArray()[1] && stationCode[1] == Routes[1])
                    {
                        int test7 = -1;
                        foreach (var stationMap in StationMapping.Values)
                        {
                            List<String> test5 = stationMap.Select(i => i.Key)
                            .ToList();
                            test7 = test5.IndexOf(StationsDistance.ElementAt(stationDistanceCount).Key);

                        }

                        var test6 = mapping.Values.ToList();

                        if (test7 != -1 &&
                            mapping.ElementAtOrDefault(test7).Key !=
                            StationsDistance.ElementAt(stationDistanceCount).Key)
                        {
                            mapping.Add(StationsDistance.ElementAt(stationDistanceCount).Key, distance);
                            break;
                        }
                        if ( //found &&
                            StationMapping.Values.Any(
                                y => y.ContainsKey(StationsDistance.ElementAt(stationDistanceCount).Key)) 
                                    )
                        {
                            continue;
                        }
                        mapping.Add(StationsDistance.ElementAt(stationDistanceCount).Key, distance);
                        break;
                    }
                    else if (startPoint && mapping.Any() && stationCode[0] == mapping.Last().Key.ToCharArray()[1] && 
                        !mapping.ContainsKey(StationsDistance.ElementAt(stationDistanceCount).Key))
                    {
                        mapping.Add(StationsDistance.ElementAt(stationDistanceCount).Key, distance);
                        stationDistanceCount = -1;
                    } else if (stationDistanceCount == 8)
                    {
                        stationDistanceCount = -1;
                    }
                }
                if (startPoint && mapping.Any() && mapping.Last().Key.ToCharArray()[1] == Routes[1])
                {
                    StationMapping.Add(x, mapping);
                    mapping = new Dictionary<string, int>();
                    
                }
                startPoint = false;


            } while ( x < StationsDistance.Count);
            
            StationMapping = MergeAdditionalRoutes(StationMapping);
            return StationMapping;
        }

        public Dictionary<int, Dictionary<string, int>> MergeAdditionalRoutes(Dictionary<int, Dictionary<string, int>> stationMapping)
        {
            string stationRoute = string.Empty;
            int distance = 0;
            int count = 0;
            bool found = false;
            
            foreach (var stationMap in stationMapping.Values)
            {
                for (int i = 0; i < stationMap.Count; i++)
                {
                    if ((distance + stationMap.ElementAt(i).Value) <= 30)
                    {
                        found = true;
                        stationRoute = stationRoute + stationMap.ElementAt(i).Key;
                        distance += stationMap.ElementAt(i).Value;
                    }
                }
                    
            }
            stationMapping.Add(stationMapping.Count, new Dictionary<string, int>
            {
                { stationRoute, distance }
            });

            if (found == false)
            {
                stationMapping = MergeAdditionalRoutes(StationMapping);
            }

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
                    var stationCode = stationDistance.Key.ToCharArray();
                    if (stationCode[0] == Routes[userEntry] && stationCode[1] == Routes[userEntry + 1])
                    {
                        routeFound = true;
                        distance = distance + stationDistance.Value;
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
