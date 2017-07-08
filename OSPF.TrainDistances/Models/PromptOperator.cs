using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSPF.TrainDistances.Models
{
    public class PromptOperator
    {
        private static TrainDistanceProcessor _trainDistanceOperator ;
        public PromptOperator()
        {
            _trainDistanceOperator = new TrainDistanceProcessor();
        }

        public string RouteType { get; set; }
        public List<char> Route { get; set; }
        public string MaxDistance { get; set; }
        public RoutesView RoutesView { get; set; }
        public void GreetingPrompt()
        {
            Console.WriteLine("Hello world");
        }


        /*
        Distance station by station: TODO: Done


        TODO: Required console prompts for following or display all information in one go
        Number of different routes end to end: TODO: display different routes
        length of shortest route end to end: TODO: display shortest rotues
        Number of different routes end to end (with max distance): TODO: Implement max distance to end to end
        */
        public string SelectRouteType()
        {
            Console.WriteLine("Select Route Type [E,e] End to End, [S,s] station by station:");
            RouteType = Console.ReadLine();
            while (!string.IsNullOrEmpty(RouteType) && (RouteType.ToLower() != "e" && RouteType.ToLower() != "s"))
            {
                Console.WriteLine("Please Enter a valid choice");
                Console.WriteLine("Please Enter Route Type [E,e] End to End, [S,s] station by station:");
                RouteType = Console.ReadLine();
            }

            
            Console.WriteLine("");
            Console.WriteLine("End to End route requires only two stations to fulfill enter [X, x] once the two entries are entered");
            Console.WriteLine("");
            Console.WriteLine("Station by station route requires less than or equal to five stations to fulfill");
            Console.WriteLine("enter [X, x] once the two entries are entered");

            return RouteType;
        }

        public void StationRoute(string routeType)
        {
            Route = new List<char>();
            string station = string.Empty;
            string[] stations = new[] {"A", "B", "C", "D", "E", "X"};
            int routeCount = 2;
            
            if (string.Compare(routeType, "e", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                routeCount = 5;
            }
            while (station != "x" && Route.Count <= routeCount)
            {
                Console.WriteLine("");
                Console.WriteLine("Select Route [A, B, C, D, E], enter [X, x] to complete route:");
                station = Console.ReadLine();
                while (!string.IsNullOrEmpty(station) && station.Length == 1 && !stations.Contains(station.ToUpper()))
                {
                    Console.WriteLine("Please Enter a valid station reference letter");
                    Console.WriteLine("Please Enter Route [A, B, C, D, E], enter [X, x] to complete route:");
                    station = Console.ReadLine();
                }

                if (!string.IsNullOrEmpty(station) && station.ToLower() == "x")
                {
                    return;
                }

                Route.Add(!string.IsNullOrEmpty(station) && station.Length > 0 ? station.ToUpper().ToCharArray().FirstOrDefault() : new char());
                if (Route.Count == routeCount)
                {
                    Console.WriteLine("Please Enter maximum distance filter, enter [A,a] for no filtering");
                    MaxDistance = Console.ReadLine();
                    Console.WriteLine("enter any key to continue to complete route:");
                    Console.ReadLine();
                    break;
                }
            }
        }

        public void Calculate()
        {
            if (RouteType.ToLower() == "e")
            {
                RoutesView = CalculateEndToEnd(Route);
            }
            else
            {
                RoutesView = CalculateStationByStation(Route);
            }
            if (!RoutesView.DifferentRoutes.Any())
            {
                Console.WriteLine("NO SUCH ROUTE");
                return;
            }
            Console.WriteLine("Different available rotues: " + RoutesView.DifferentRoutes.Aggregate((i, j) => i + " " + j));
            Console.WriteLine("Number of different routes: " + RoutesView.RoutesCount);
            Console.WriteLine("Shortest possible route: " + RoutesView.ShortestRoute);
        }

        public RoutesView CalculateEndToEnd(List<char> Route)
        {
            RoutesView routesView = _trainDistanceOperator.CalculateEndToEnd(Route, MaxDistance);
           
            return routesView;
        }

        public RoutesView CalculateStationByStation(List<char> Route)
        {
            RoutesView routesView = _trainDistanceOperator.CalculateStationByStation(Route);

            return routesView;
        }
    }
}
