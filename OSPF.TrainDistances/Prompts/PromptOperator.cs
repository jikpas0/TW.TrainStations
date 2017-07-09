using System;
using System.Collections.Generic;
using System.Linq;
using OSPF.TrainDistances.BusinessLogic;
using OSPF.TrainDistances.Models;
using OSPF.TrainDistances.BusinessLogic.Interfaces;

namespace OSPF.TrainDistances.Prompts
{
    public class PromptOperator
    {
        private static ITrainDistance _trainDistance;
        private static Prompt _prompt;

        public PromptOperator(ITrainDistance trainDistance, Prompt prompt)
        {
            _trainDistance = trainDistance;
            _prompt = prompt;
        }

        public PromptOperator() : this(new TrainDistance(), new Prompt())
        {

        }

        public void GreetingPrompt()
        {
            Console.WriteLine("Hello world");
        }
        
        public string SelectRouteType()
        {
            Console.WriteLine("Select Route Type [E,e] End to End, [S,s] station by station:");
            _prompt.RouteType = Console.ReadLine();
            while (!string.IsNullOrEmpty(_prompt.RouteType) && (_prompt.RouteType.ToLower() != "e" && _prompt.RouteType.ToLower() != "s"))
            {
                Console.WriteLine("Please Enter a valid choice");
                Console.WriteLine("Please Enter Route Type [E,e] End to End, [S,s] station by station:");
                _prompt.RouteType = Console.ReadLine();
            }

            
            Console.WriteLine("");
            Console.WriteLine("End to End route requires only two stations to fulfill enter [X, x] once the two entries are entered");
            Console.WriteLine("");
            Console.WriteLine("Station by station route requires less than or equal to five stations to fulfill");
            Console.WriteLine("enter [X, x] once the two entries are entered");

            return _prompt.RouteType;
        }

        public void StationRoute(string routeType)
        {
            _prompt.Route = new List<char>();
            string station = string.Empty;
            string[] stations = new[] {"A", "B", "C", "D", "E", "X"};
            int routeCount = 2;
            
            if (string.Compare(routeType, "e", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                routeCount = 5;
            }
            while (station != "x" && _prompt.Route.Count <= routeCount)
            {
                Console.WriteLine("");
                Console.WriteLine("Select Route [A, B, C, D, E], enter [X, x] to complete route:");
                station = Console.ReadLine();
                while (string.IsNullOrEmpty(station) || (station.Length == 1 && !stations.Contains(station.ToUpper())))
                {
                    Console.WriteLine("Please Enter a valid station reference letter");
                    Console.WriteLine("Please Enter Route [A, B, C, D, E], enter [X, x] to complete route:");
                    station = Console.ReadLine();
                }

                if (!string.IsNullOrEmpty(station) && station.ToLower() == "x")
                {
                    return;
                }

                _prompt.Route.Add(!string.IsNullOrEmpty(station) && station.Length > 0 ? station.ToUpper().ToCharArray().FirstOrDefault() : new char());
                if (_prompt.Route.Count == routeCount)
                {
                    Console.WriteLine("Please Enter maximum distance filter, enter [A,a] for no filtering");
                    _prompt.MaxDistance = Console.ReadLine();
                    Console.WriteLine("enter any key to continue to complete route:");
                    Console.ReadLine();
                    break;
                }
            }
        }

        public void Calculate()
        {
            if (_prompt.RouteType.ToLower() == "e")
            {
                _prompt.RoutesView = CalculateEndToEnd(_prompt.Route);
                Console.WriteLine("Different available rotues: " +
                                  _prompt.RoutesView.DifferentRoutes.Aggregate((i, j) => i + " " + j));
                Console.WriteLine("Number of different routes: " + _prompt.RoutesView.RoutesCount);
                Console.WriteLine("Shortest possible route: " + _prompt.RoutesView.ShortestRoute);
            }
            else if (_prompt.RouteType.ToLower() == "s")
            {
                _prompt.RoutesView = CalculateStationByStation(_prompt.Route);
                Console.WriteLine("Number of different routes: " +
                                _prompt.RoutesView.DifferentRoutes.Aggregate((i, j) => i + " " + j));
                Console.WriteLine("Shortest possible route: " + _prompt.RoutesView.Distance);
            }
            else
            {
                Console.WriteLine("NO SUCH ROUTE");
                return;
            }
        }

        public RoutesView CalculateEndToEnd(List<char> Route)
        {
            RoutesView routesView = _trainDistance.CalculateEndToEnd(Route, _prompt.MaxDistance);
           
            return routesView;
        }

        public RoutesView CalculateStationByStation(List<char> Route)
        {
            RoutesView routesView = _trainDistance.CalculateStationByStation(Route);
            
            return routesView;
        }
    }
}
