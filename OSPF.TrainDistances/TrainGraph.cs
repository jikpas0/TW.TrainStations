using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSPF.TrainDistances.Models;

namespace OSPF.TrainDistances
{
    public class TrainGraph
    {
        public static void Main(string[] args)
        {
            PromptOperator promptOperator = new PromptOperator();

            promptOperator.GreetingPrompt();
            promptOperator.SelectRouteType();
            promptOperator.StationRoute();
            promptOperator.Calculate();
            Console.ReadLine();
        }
    }
}
