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
            // validation rules re-enter if not allowed
            Console.WriteLine("");
            Console.WriteLine("End to End route requires only two stations to fulfill enter [X, x] once the two entries are entered");
            Console.WriteLine("");
            Console.WriteLine("Station by station route requires less than or equal to five stations to fulfill");
            Console.WriteLine("enter [X, x] once the two entries are entered");
        }

        public void StationRoute()
        {
            Route = new List<char>();
            string station = string.Empty;
            while (station != "x" && Route.Count <= 5)
            {
                Console.WriteLine("");
                Console.WriteLine("Select Route, enter [X, x] to complete route");
                station = Console.ReadLine();
                // validation rules re-enter if not allowed
                if (station.ToLower() == "x")
                {
                    return;
                }
                Route.Add(station.Length > 0 ? station.ToUpper().ToCharArray().FirstOrDefault() : new char());
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
            Dictionary<int, List<TrainStations>> calculatedJourney = _trainDistanceOperator.CalculateEndToEnd(Route);
            string journeyDistance = string.Empty;
            if (!calculatedJourney.Any())
            {
                journeyDistance = "NO SUCH ROUTE";
            }

            foreach (var journey in calculatedJourney)
            {
                if (string.IsNullOrEmpty(journeyDistance) || int.Parse(journeyDistance) > journey.Value.Sum(j => j.Distance))
                {
                    journeyDistance = journey.Value.Sum(j => j.Distance).ToString();
                }
            }
            
            return journeyDistance;
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
