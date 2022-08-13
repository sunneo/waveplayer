using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer
{
    public class Utility
    {
        public static String TimeSpanToString(TimeSpan delta)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", delta.Hours, delta.Minutes, delta.Seconds);
        }
    }
    public static class TimeSpanExtension
    {
        public static String ToSimpleString(this TimeSpan delta)
        {
            return Utility.TimeSpanToString(delta);
        }
    }
}
