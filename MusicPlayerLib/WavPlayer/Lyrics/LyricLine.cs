using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.Lyrics
{
    public class LyricLine
    {
        public IList<LyricItem> lyrics = new List<LyricItem>();
        public void Add(LyricItem item)
        {
            lyrics.Add(item);
        }
        public LyricItem this[int idx]
        {
            get
            {
                return lyrics[idx];
            }
            set
            {
                lyrics[idx] = value;
            }
        }
        private long LyricLine_Length_Getter(LyricLine line)
        {
            if (line.lyrics.Count > 1)
            {
                LyricItem last = line.lyrics.Last();
                return (last.start + last.interval - line.lyrics.First().start);
            }
            else
            {
                LyricItem last = line.lyrics[0];
                return last.interval;
            }
        }
        private long LyricLine_Begin_Getter(LyricLine line)
        {
            return line.lyrics.First().start;
        }
        public long Start
        {
            get
            {
                if (lyrics.Count == 0)
                {
                    return 0;
                }
                return LyricLine_Begin_Getter(this);
            }
        }
        public long Interval
        {
            get
            {
                if (lyrics.Count == 0)
                {
                    return 0;
                }
                return LyricLine_Length_Getter(this);
            }
        }
        public long End
        {
            get
            {
                if (lyrics.Count == 0)
                {
                    return 0;
                }
                LyricItem end = lyrics.Last();
                return end.start + end.interval;
            }
        }
        public String Text
        {
            get
            {
                if (lyrics.Count == 0)
                {
                    return "";
                }
                return lyrics[0].text;
            }
            set
            {
                if (lyrics.Count != 0)
                {
                    lyrics[0].text = value;
                }
            }
        }
        public void Clear()
        {
            lyrics.Clear();
        }
        public LyricItem First()
        {
            return lyrics.First();
        }
        public LyricItem Last()
        {
            return lyrics.Last();
        }
        public int Count
        {
            get
            {
                return lyrics.Count;
            }
        }
        public override string ToString()
        {
            StringBuilder strb = new StringBuilder();
            foreach (LyricItem item in lyrics)
            {
                strb.Append(item.ToString());
            }
            string ret = strb.ToString();
            strb.Clear();
            return ret;
        }

    }
}
