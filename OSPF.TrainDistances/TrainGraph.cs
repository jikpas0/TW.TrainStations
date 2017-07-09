using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSPF.TrainDistances.Models;
using OSPF.TrainDistances.Prompts;

namespace OSPF.TrainDistances
{
    public class TrainGraph
    {
        public static void Main(string[] args)
        {
            PromptOperator promptOperator = new PromptOperator();
            string isExit = string.Empty;
            while (!string.IsNullOrEmpty(isExit) && isExit.ToLower() != "exit")
            {
                promptOperator.GreetingPrompt();
                var routeType = promptOperator.SelectRouteType();
                promptOperator.StationRoute(routeType);
                promptOperator.Calculate();
                Console.WriteLine("To quit enter [Exit, exit], any other entry will restart the train time check");
                isExit = Console.ReadLine();
            }
        }
    }
}
