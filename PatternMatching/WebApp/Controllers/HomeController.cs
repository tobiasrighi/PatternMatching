using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using PatternLib;
using System.Threading;
using WebApp.Models;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        LogEventModel Model { get; set; }
        public ActionResult Index()
        {
            Model = new LogEventModel();
            Model.logEvents = new List<LogEvent>();
            TempData["Model"] = Model;
            return View();
        }

        [HttpPost]
        public ActionResult EventLog()
        {
            //if (TempData.Peek("Model") != null)
            //{
            //    Model = (LogEventModel)TempData.Peek("Model");
            //    MatchingPattern matchPattern = new MatchingPattern(Model.logEvents);

            //    List<Task> tasks = new List<Task>();
            //    int nro = 0;
            //    while (nro <= 5)
            //    {
            //        int x = nro;

            //        //get the test file in debug folder
            //        string path =
            //            Path.Combine(Server.MapPath("~/App_Data/"), "LogFile" + x + ".csv");
            //        //create a new thread
            //        Task t = Task.Run(() =>
            //        {
            //            //call the parseEvent
            //            matchPattern.ParseEvents(
            //            //create a new deviceId for each thread
            //            "HV" + x,
            //            //load different csv file for each thread
            //            new StreamReader(path));
            //        });
            //        //add the new task to the list so can wait all threads to finish in the end
            //        tasks.Add(t);
            //        TempData["Model"] = Model;
            //        nro += 1;
            //    }
            //}
            return View();
        }

        public JsonResult PartialEventLog()
        {
            if (TempData.Peek("Model") != null)
            {
                Model = (LogEventModel)TempData.Peek("Model");
                MatchingPattern matchPattern = new MatchingPattern(Model.logEvents);
                return Json(matchPattern.GetEventCounts(), JsonRequestBehavior.AllowGet);
            }
            else
                return Json("EMPTY", JsonRequestBehavior.AllowGet);
        }

        public JsonResult PartialCalculate(int nro)
        {
            if (TempData.Peek("Model") != null)
            {
                Model = (LogEventModel)TempData.Peek("Model");
                MatchingPattern matchPattern = new MatchingPattern(Model.logEvents);

                //List<Task> tasks = new List<Task>();
                //while (nro <= 5)
                //{
                //    int x = nro;

                    //get the test file in debug folder
                    string path =
                        Path.Combine(Server.MapPath("~/App_Data/"), "LogFile" + nro + ".csv");
                    //create a new thread
                    Task t = Task.Run(() =>
                    {
                        //call the parseEvent
                        matchPattern.ParseEvents(
                        //create a new deviceId for each thread
                        "HV" + nro,
                        //load different csv file for each thread
                        new StreamReader(path));
                    });
                    //add the new task to the list so can wait all threads to finish in the end
                    //tasks.Add(t);
                    TempData["Model"] = Model;
                    //nro += 1;
                //}
            }

            return Json("EMPTY", JsonRequestBehavior.AllowGet);
        }
    }
}