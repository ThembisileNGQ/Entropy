using System;
using Akka.Configuration;

namespace Akka.Persistence.Performance.Runner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //replace this with your relevant config for persistence, defaults to inmem 
            //if you are using a different persistence plugin make sure to also include
            //the NuGet package of that plugin for this to work.
            var config = Config.Empty; 

            var criteria = new TestCriteria(config,1000,100);

            var testRunner = TestRunner.Init(criteria);

            var testResult  = testRunner.Run();

            Console.WriteLine(testResult);
            
            Console.ReadLine();
        }
    }
}