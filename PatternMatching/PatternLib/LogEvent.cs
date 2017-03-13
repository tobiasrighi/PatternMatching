using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternLib
{
    /// <summary>
    /// Class to keep the device Id and the number of errors
    /// </summary>
    public class LogEvent
    {
        public string deviceId { get; set; }
        public int eventCount { get; set; }
    }
}
