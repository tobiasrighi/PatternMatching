using RC.CodingChallenge;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace PatternLib
{
    public class MatchingPattern : EventCounter
    {
        List<LogEvent> eventList;
        private Object thisLock = new Object();

        /// <summary>
        /// Create one instance of Matching Pattern and keep just one instance of eventList
        /// </summary>
        /// <param name="list">instance </param>
        public MatchingPattern(List<LogEvent> list)
        {
            //use just one instance of the list
            eventList = list;
        }

        /// <summary>
        /// Gets the current count of events detected
        /// </summary>
        /// <returns>
        /// JSON document giving the total count of events detected for each device
        /// Ex:
        /// [{"deviceID":"HV1","eventCount":3},{"deviceID":"HV2","eventCount":0},...]
        /// </returns>
        public string GetEventCounts()
        {
            //convert list of errors into a JSON string
            return Newtonsoft.Json.JsonConvert.SerializeObject(eventList);
        }

        /// <summary>
        /// Parse and accumulate event information from the given log data.
        /// </summary>
        /// <param name="deviceID">The ID of the device</param>
        /// <param name="eventLog">A stream of lines representing time/value recordings.</param>
        public void ParseEvents(string deviceID, StreamReader eventLog)
        {
            //lock method so can be used multithread
            lock (thisLock)
            {
                //create a new row for each new deviceId
                LogEvent ev = eventList.Find(e => e.deviceId.Equals(deviceID));
                if (ev == null)
                {
                    ev = new LogEvent();
                    ev.deviceId = deviceID;
                    ev.eventCount = 0;
                    eventList.Add(ev);
                }

                DateTime timestamp = DateTime.MinValue;
                int value;
                bool possibleDamage = false;
                bool damage = false;

                //check if the file have data
                if (!eventLog.EndOfStream)
                    //first line are headers so skip
                    eventLog.ReadLine();

                //while the file have data
                while (!eventLog.EndOfStream)
                {
                    //ready the line
                    string aux = eventLog.ReadLine();
                    //split into columns
                    string[] cols = aux.Split(',');
                    //value have the stage
                    value = Convert.ToInt32(cols[1]);

                    //if its a damage wait until stage 0 and count +1 error;
                    if (damage)
                    {
                        if (value == 0)
                        {
                            ev.eventCount += 1;
                            damage = false;
                        }
                    }
                    else if (possibleDamage)
                    {
                        //if its a possibleDamage check if next stage its stage 2
                        if (value == 2)
                        {
                            //check if it was in stage 3 for more than 5 minutes and set as a problem
                            if (Convert.ToDateTime(cols[0]).Subtract(timestamp) >= new TimeSpan(0, 5, 0))
                                damage = true;
                            else
                                possibleDamage = false;
                        }
                        //if its a possibleDamage and next stage its not a 2, its not a damage. keep reading the file.
                        else if (value < 2)
                            possibleDamage = false;
                    }
                    //if its in stage 3 set as a possibleDamage and save the timestamp
                    else if (value == 3)
                    {
                        possibleDamage = true;
                        timestamp = Convert.ToDateTime(cols[0]);
                    }
                }
            }
        }
    }
}
