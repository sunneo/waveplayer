using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SoundTouch.Utility
{
    public class Logger
    {
        public static StreamWriter LogRep = null;
        public static StreamWriter Log
        {
            get
            {
                if (LogRep == null)
                {
                    String path = Environment.CurrentDirectory;
                    if (!path.EndsWith("\\") || !path.EndsWith("/"))
                    {
                        path += "\\";
                    }
                    path += "errorlog\\";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    TimeSpan tmspan = TimeSpan.FromTicks(DateTime.Now.Ticks);

                    String filename = string.Format("sndtouch_y{0:0000}m{1:00}d{2:00}h{3:00}M{4:00}S{5:00}g{6}.log",
                        DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, tmspan.Hours, tmspan.Minutes, tmspan.Seconds, Guid.NewGuid().ToString());
                    StreamWriter sw = new StreamWriter(new FileStream(path + filename, FileMode.Append));
                    LogRep = sw;
                }
                return LogRep;
            }
        }
    }
}
