using PatternLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //create a instance of the list
            List<LogEvent> logEvents = new List<LogEvent>();
            //create a instance of the matchPattern and use only one list
            MatchingPattern matchPattern = new MatchingPattern(logEvents);

            //run only one csv file
            //matchPattern.ParseEvents("HV0", new StreamReader(Path.Combine(@"C:\Users\Tobias\Desktop\", "LogFile0.csv")));
            //Console.WriteLine(matchPattern.GetEventCounts());

            //create some threads with different deviceId and log files
            List<Task> tasks = new List<Task>();
            int nro = 0;
            while (nro <= 5) {
                int x = nro;
                //get the test file in debug folder
                string path =
                    Path.Combine(@"C:\Temp", "LogFile" + x + ".csv");
                //create a new thread
                Task t = Task.Run(() => {
                    //call the parseEvent
                    matchPattern.ParseEvents(
                    //create a new deviceId for each thread
                    "HV" + x,
                    //load different csv file for each thread
                    new StreamReader(path));
                });
                //add the new task to the list so can wait all threads to finish in the end
                tasks.Add(t);
                //print the results so far
                Console.WriteLine(matchPattern.GetEventCounts());
                //add 2 seconds delay
                //Thread.Sleep(2000);
                nro += 1;
            }

            //wait for all threads to finish
            Task.WaitAll(tasks.ToArray());
            //print the final results
            Console.WriteLine(matchPattern.GetEventCounts());

            Console.ReadLine();
        }
    }
}
