using System.Collections.Generic;
using OSPF.TrainDistances.Models;

namespace OSPF.TrainDistances.BusinessLogic.Interfaces
{
    public interface ITrainDistance
    {
        RoutesView CalculateEndToEnd(List<char> routes, string maxDistance);
        bool CheckDistinct(List<TrainStations> stationMappings, List<TrainStations> mappings, bool isSingle = false);
        bool CheckDistinctOnSingle(List<TrainStations> stationMappings, List<TrainStations> mappings);
        List<TrainStations> AggregateStationsToRoute(Dictionary<int, List<TrainStations>> mapping);
        List<TrainStations> MergeAdditionalRoutes(Dictionary<int, List<TrainStations>> stationMapping, int maxDistance);
        RoutesView CalculateStationByStation(List<char> routes);
    }
}
