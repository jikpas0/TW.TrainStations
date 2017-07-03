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
        public string JourneyDistance { get; set; }
        public void GreetingPrompt()
        {
            Console.WriteLine("Hello world");
        }

        public void SelectRouteType()
        {
            Console.WriteLine("Select Route Type [E,e] End to End, [S,s] station by station");
            RouteType = Console.ReadLine();
        }

        public void StationRoute()
        {
            Route = new List<char>();
            string station = string.Empty;
            while (station != "x" && Route.Count <= 5)
            {
                Console.WriteLine("Select Route");
                station = Console.ReadLine();
                Route.Add(station.Length > 0 ? station.ToCharArray().FirstOrDefault() : new char());
            }
        }

        public void Calculate()
        {
            if (RouteType.ToLower() == "e")
            {
                JourneyDistance = CalculateEndToEnd(Route);
            }
            else
            {
                JourneyDistance = CalculateStationByStation(Route);
            }

            Console.WriteLine(JourneyDistance);
        }

        public string CalculateEndToEnd(List<char> Route)
        {
            return "0";
        }

        public string CalculateStationByStation(List<char> Route)
        {
            string calculatedJourney = _trainDistanceOperator.CalculateStationByStation(Route).ToString();
            if (calculatedJourney == "-1")
            {
                calculatedJourney = "NO SUCH ROUTE";
            }
            return calculatedJourney;
        }
    }
}
