using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.Lyrics
{
    public class LyricItem
    {
        public int id;
        public int fileid;
        public int groupid; // line
        public int start;
        public int interval = 1000; // default: 2 second
        public String text;
        public LyricItem(int fileid, int groupid, int start, String text)
        {
            this.fileid = fileid;
            this.groupid = groupid;
            this.start = start;
            this.text = text;
        }
        public LyricItem()
        {
        }
        private string millisecondToString(int mills)
        {
            TimeSpan tm = TimeSpan.FromMilliseconds((double)mills);
            return string.Format("[{0:d2}:{1:d2}.{2:d3}]", tm.Minutes, tm.Seconds, tm.Milliseconds);
        }
        public override string ToString()
        {
            StringBuilder strb = new StringBuilder();
            strb.AppendFormat("{0}{1}" + Environment.NewLine + "{2}{3}", millisecondToString(start), text, millisecondToString(start + interval), " ");
            string ret = strb.ToString();
            strb.Clear();
            return ret;
        }
    }
}
