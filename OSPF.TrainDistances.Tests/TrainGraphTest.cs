using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OSPF.TrainDistances.Models;

namespace OSPF.TrainDistances.Tests
{
    [TestFixture]
    public class TrainGraphTest
    {
        [SetUp]
        public void Init()
        {
        }

        [TearDown]
        public void Dispose()
        {
        }
       
        [Test]
        public void ShouldReturnEndToEndDistance()
        {
            //Setup
            TrainDistanceProcessor trainDistances = new TrainDistanceProcessor();
            List<char> stationsStub = new List<char> {'C', 'C'};

            //Act
            Dictionary<int, List<TrainStations>> calulcatedDistance = trainDistances.CalculateEndToEnd(stationsStub);
            
            //Assert
            Assert.IsNotNull(calulcatedDistance);
        }

        [Test]
        public void ShouldReturnAllPossibleRoutes()
        {
            //Setup
            TrainDistanceProcessor trainDistances = new TrainDistanceProcessor();
            Dictionary<int, List<TrainStations>> stationsStub = new Dictionary<int, List<TrainStations>>
            {
                { 0, new List<TrainStations> 
                    {
                        new TrainStations {
                            Station = "AB",
                            Distance = 15,
                        },
                        new TrainStations { 
                            Station = "BC",
                            Distance = 13
                        }
                    }
                },
                { 1, new List<TrainStations> 
                    {
                        new TrainStations {
                            Station = "CD",
                            Distance = 9,
                        },
                        new TrainStations { 
                            Station = "DE",
                            Distance = 15
                        }
                    }
                },
                { 2, new List<TrainStations> 
                    {
                        new TrainStations {
                            Station = "EA",
                            Distance = 9,
                        },
                        new TrainStations { 
                            Station = "AC",
                            Distance = 5
                        }
                    }
                }
            };

            //Act
            Dictionary<int, List<TrainStations>> calulcatedDistance = trainDistances.MergeAdditionalRoutes(stationsStub);

            //Assert
            Assert.IsNotNull(calulcatedDistance);
        }

        //AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7
        [TestCase(new[] { 'A', 'B', 'C', 'D', 'E' }, 23)]
        [TestCase(new[] { 'A', 'B', 'C' }, 9)]
        [TestCase(new[] { 'A', 'B', 'D', 'E' }, -1)]
        [TestCase(new[] { 'A', 'B', 'C', 'D' }, 17)]
        public void ShouldReturnStationDistances(char[] stationsStub, int obtained)
        {
            //Setup
            TrainDistanceProcessor trainDistances = new TrainDistanceProcessor();
            //List<char> stationsStub = new List<char> { 'A', 'B', 'C', 'D', 'E' };

            //Act
            int calulcatedDistance = trainDistances.CalculateStationByStation(stationsStub.ToList());

            //Assert
            Assert.That(calulcatedDistance, Is.EqualTo(obtained), "Real part");
        }

        /* Basic Test Inputs: distance for a set route
        The distance of the route A-B-C.
        The distance of the route A-D.
        The distance of the route A-D-C.
        The distance of the route A-E-B-C-D.
        The distance of the route A-E-D.

        Complex Test Inputs: starting and ending at a set station
        The number of trips starting at C and ending at C with a maximum of 3 stops.
        In the sample data below, there are two such trips: C-D-C(2 stops). and C-E-B-C(3 stops).

        The number of trips starting at A and ending at C with exactly 4 stops.
        In the sample data below, there are three such trips: 
        A to C(via B, C, D); A to C(via D, C, D); and A to C(via D, E, B).

        The length of the shortest route(in terms of distance to travel) from A to C.

        The length of the shortest route (in terms of distance to travel) from B to B.

        The number of different routes from C to C with a distance of less than 30.  
        In the sample data, the trips are: CDC, CEBC, CEBCDC, CDCEBC, CDEBC, CEBCEBC, CEBCEBCEBC.*/
    }
}
