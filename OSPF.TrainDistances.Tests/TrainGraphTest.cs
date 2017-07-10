using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OSPF.TrainDistances.BusinessLogic;
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
            TrainDistance trainDistances = new TrainDistance();
            List<char> stationsStub = new List<char> {'C', 'C'};

            //Act
            RoutesView routesView = trainDistances.CalculateEndToEnd(stationsStub, "30");
            
            //Assert
            Assert.IsNotNull(routesView.Distance);
            Assert.That(routesView.Distance, Is.EqualTo(9));
            Assert.That(routesView.ShortestRoute, Is.EqualTo(9));
            Assert.That(routesView.DifferentRoutes.Count, Is.EqualTo(7));
        }

        [Test]
        public void ShouldReturnAllPossibleRoutes()
        {
            //Setup
            TrainDistance trainDistances = new TrainDistance();
            Dictionary<int, List<TrainStations>> stationsStub = new Dictionary<int, List<TrainStations>>
            {
                { 0, new List<TrainStations> 
                    {
                        new TrainStations {
                            Station = "CD",
                            Distance = 3,
                            Merged = false
                        },
                        new TrainStations { 
                            Station = "DC",
                            Distance = 4,
                            Merged = false
                        }
                    }
                },
                { 1, new List<TrainStations> 
                    {
                        new TrainStations {
                            Station = "CE",
                            Distance = 2,
                            Merged = false
                        },
                        new TrainStations { 
                            Station = "EB",
                            Distance = 4,
                            Merged = false
                        },
                        new TrainStations {
                            Station = "BC",
                            Distance = 2,
                            Merged = false
                        }
                    }
                },
                { 2, new List<TrainStations> 
                    {
                        new TrainStations {
                            Station = "EA",
                            Distance = 9,
                            Merged = false
                        },
                        new TrainStations { 
                            Station = "AC",
                            Distance = 5,
                            Merged = false
                        }
                    }
                }
            };

            //Act
            List<TrainStations> calulcatedDistance = trainDistances.MergeAdditionalRoutes(stationsStub, 30)
                .Where(md => md.Distance < 30).ToList();

            //Assert
            Assert.IsNotNull(calulcatedDistance);
            Assert.That(calulcatedDistance.Count, Is.EqualTo(7));
            Assert.That(calulcatedDistance.Count, Is.EqualTo(7));
            Assert.That(calulcatedDistance.Any(x => x.Station == "EAACEAAC"));
            Assert.That(calulcatedDistance.Any(x => x.Station == "CEEBBCCEEBBC"));
            Assert.That(calulcatedDistance.Any(x => x.Station == "CD"));
        }

        //AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7
        [TestCase(new[] { 'A', 'B', 'C', 'D', 'E' }, 23)]
        [TestCase(new[] { 'A', 'B', 'C' }, 9)]
        [TestCase(new[] { 'A', 'B', 'D', 'E' }, -1)]
        [TestCase(new[] { 'A', 'B', 'C', 'D' }, 17)]
        public void ShouldReturnStationDistances(char[] stationsStub, int obtained)
        {
            //Setup
            TrainDistance trainDistances = new TrainDistance();
            //List<char> stationsStub = new List<char> { 'A', 'B', 'C', 'D', 'E' };

            //Act
            RoutesView routesView = trainDistances.CalculateStationByStation(stationsStub.ToList());

            //Assert
            Assert.That(routesView.Distance, Is.EqualTo(obtained), "Real part");
        }


        [TestCase(new[] { "AB", "BC" }, new[] { "AB", "BC" }, true, false )]
        [TestCase(new[] { "DE", "ED" }, new[] { "AB", "BC" }, false, false )]
        [TestCase(new[] { "AB", "BC" }, new[] { "AB"  }, true, true)]
        [TestCase(new[] { "DE", "ED" }, new[] { "AB" }, false, true)]
        public void ShouldReturnFalseIfDistinct(string[] station, string[] stationCompare, bool expectedResult, bool isSingle)
        {
            //Setup
            TrainDistance trainDistances = new TrainDistance();
            List<TrainStations> trainStations = new List<TrainStations>
            {
                new TrainStations
                {
                    Distance = 1,
                    Station = station[0]
                },
                new TrainStations
                {
                    Distance = 1,
                    Station = station[1]
                }
            };
            List<TrainStations> trainStationsCompare = new List<TrainStations>
            {
                new TrainStations
                {
                    Distance = 1,
                    Station = stationCompare[0]
                },
                !isSingle ? new TrainStations
                {
                    Distance = 1,
                    Station = stationCompare[1]
                } : new TrainStations()
            };

            //Act
            bool isDistinct = trainDistances.CheckDistinct(trainStations, trainStationsCompare, isSingle);

            //Assert
            Assert.AreEqual(isDistinct, expectedResult);
        }

        [Test]
        public void ShouldMergeRoutes()
        {
            //Setup
            TrainDistance trainDistances = new TrainDistance();

            //Act
            Dictionary<int, List<TrainStations>> mappings = new Dictionary<int, List<TrainStations>>
            {
                { 0, new List<TrainStations>
                    {
                        new TrainStations
                        {
                          Distance = 1,
                          Station = "AB",
                        },
                        new TrainStations
                        {
                          Distance = 1,
                          Station = "BC",
                        },
                        new TrainStations
                        {
                          Distance = 1,
                          Station = "CD",
                        },
                        new TrainStations
                        {
                          Distance = 1,
                          Station = "DE",
                        }

                    }
                }
            };

            var aggregatedResult = trainDistances.AggregateStationsToRoute(mappings);

            Assert.IsNotNull(aggregatedResult);
            Assert.That(aggregatedResult.FirstOrDefault(), Is.Not.Null);
            Assert.That(aggregatedResult.FirstOrDefault().Distance, Is.EqualTo(4));
            Assert.That(aggregatedResult.FirstOrDefault().Station, Is.EqualTo("ABBCCDDE"));
        }
    }
}
