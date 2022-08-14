using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.Lyrics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;


    public class LyricParser
    {
        #region values
        String lrcCode;

        public String LrcCode
        {
            get
            {
                return lrcCode;
            }
            set
            {
                lrcCode = value;
                lyricsDictionary = lrcToDictionary(lrcCode, lyricLines);
                timeArray = sortedKeysInDictionary(lyricsDictionary);
                try
                {
                    if(lyricsDictionary.ContainsKey("ti"))
                        title = lyricsDictionary["ti"].ToString();
                }
                catch { }
                try
                {
                    if (lyricsDictionary.ContainsKey("ar"))
                        artist = lyricsDictionary["ar"].ToString();
                }
                catch { }
                try
                {
                    if (lyricsDictionary.ContainsKey("by"))
                        lyricsMaker = lyricsDictionary["by"].ToString();
                }
                catch { }
                try
                {
                    if (lyricsDictionary.ContainsKey("al"))
                        album = lyricsDictionary["al"].ToString();
                }
                catch { }
                switch (isOffsetEnabled)
                {
                    case true:
                        if (lyricsDictionary["offset"] != null)
                            int.TryParse(lyricsDictionary["offset"].ToString(), out offset);
                        else
                            offset = 0;
                        break;
                    case false:
                        offset = 0;
                        break;
                }
            }
        }
        int offset;

        public int Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
            }
        }
        System.Collections.Hashtable lyricsDictionary;

        public Hashtable LyricsDictionary
        {
            get
            {
                return lyricsDictionary;
            }
            set
            {
                lyricsDictionary = value;
                timeArray = sortedKeysInDictionary(lyricsDictionary);
                try
                {
                    title = lyricsDictionary["ti"].ToString();
                }
                catch { }
                try
                {
                    artist = lyricsDictionary["ar"].ToString();
                }
                catch { }
                try
                {
                    lyricsMaker = lyricsDictionary[@"by"].ToString();
                }
                catch { }
                try
                {
                    album = lyricsDictionary["al"].ToString();
                }
                catch { }
                switch (isOffsetEnabled)
                {
                    case true:
                        if (lyricsDictionary["offset"] != null)
                            int.TryParse(lyricsDictionary["offset"].ToString(), out offset);
                        else
                            offset = 0;
                        break;
                    case false:
                        offset = 0;
                        break;
                }
            }
        }
        System.Collections.ArrayList timeArray;
        String currentLyrics;

        public String CurrentLyrics
        {
            get
            {
                return currentLyrics;
            }
            set
            {
                currentLyrics = value;
            }
        }
        String nextLyrics;

        public String NextLyrics
        {
            get
            {
                return nextLyrics;
            }
            set
            {
                nextLyrics = value;
            }
        }
        String previousLyrics;

        public String PreviousLyrics
        {
            get
            {
                return previousLyrics;
            }
            set
            {
                previousLyrics = value;
            }
        }
        int currentIndex;

        public int CurrentIndex
        {
            get
            {
                return currentIndex;
            }
            set
            {
                currentIndex = value;
            }
        }
        String title;

        public String Title
        {
            get
            {
                return title;
            }
        }
        String album;

        public String Album
        {
            get
            {
                return album;
            }
        }
        String artist;

        public String Artist
        {
            get
            {
                return artist;
            }
        }
        String lyricsMaker;

        public String LyricsMaker
        {
            get
            {
                return lyricsMaker;
            }
        }
        bool isOffsetEnabled;

        public bool OffsetEnabled
        {
            set
            {
                switch (value)
                {
                    case true:
                        offset = (int)lyricsDictionary["offset"];
                        break;
                    case false:
                        offset = 0;
                        break;
                }
                isOffsetEnabled = value;
            }
            get
            {
                return isOffsetEnabled;
            }
        }
        #endregion
        #region build

        public LyricParser()
        {
            OffsetEnabled = true;
            offset = 0;
            currentIndex = 0;
        }

        public LyricParser(String filePath, Encoding enc)
        {
            isOffsetEnabled = true;
            offset = 0;
            currentIndex = 0;
            StreamReader streamReader = new StreamReader(filePath, enc);
            LrcCode = streamReader.ReadToEnd();
        }

        public LyricParser(String code)
        {
            isOffsetEnabled = true;
            offset = 0;
            currentIndex = 0;
            LrcCode = code;
        }
        #endregion

        public IList<LyricLine> lyricLines = new List<LyricLine>();
        protected static Hashtable lrcToDictionary(String lrc)
        {
            return lrcToDictionary(lrc, null);
        }
        #region protected functions
        protected static Hashtable lrcToDictionary(String lrc, IList<LyricLine> lines)
        {
            String lrct = lrc.Replace("\r", "\n");
            Hashtable md = new Hashtable();
            String aline;
            String[] av = lrct.Split('\n');
            int i;
            for (i = 0; i < av.GetLength(0); i++)
            {
                if (av[i] != "")
                {
                    aline = av[i].Replace("[", "");

                    if (aline.IndexOf("]") != -1)
                    {
                        if (aline.Split(']').GetLength(0) == 2)
                        {
                            if (aline.IndexOf("ti:") != -1
                            || aline.IndexOf("ar:") != -1
                            || aline.IndexOf("al:") != -1
                            || aline.IndexOf("by:") != -1
                            || aline.IndexOf("offset:") != -1)
                            {// [ti:title ] ...
                                aline = aline.Replace("]", "");
                                md[aline.Split(':').GetValue(0)] = aline.Split(':').GetValue(1);
                            }
                            else
                            {// [00:00.00] 
                                String intervalString = (String)aline.Split(']').GetValue(0);
                                String content = (String)aline.Split(']').GetValue(1);
                                md[intervalString] = content;
                                if (lines != null)
                                {
                                    LyricLine line = new LyricLine();
                                    LyricItem item = new LyricItem();
                                    item.groupid = lines.Count;
                                    item.start = stringToMilliInterval(intervalString);
                                    item.text = content;
                                    line.lyrics.Add(item);
                                    lines.Add(line);
                                }
                            }
                        }
                        else
                        {   // [00:00.01] String1 [00:00.02] String2
                            LyricLine line = null;

                            int subi;
                            if (lines != null)
                            {
                                line = new LyricLine();
                            }

                            for (subi = 0; subi < aline.Split(']').GetLength(0); subi++)
                            {

                                if (subi < aline.Split(']').GetLength(0) - 1)
                                {
                                    String intervalString = (String)aline.Split(']').GetValue(subi);
                                    //Console.WriteLine("intervalString={0}", intervalString);
                                    String content = (String)aline.Split(']').GetValue(aline.Split(']').GetLength(0) - 1);
                                    //Console.WriteLine("content={0}", content);
                                    md[intervalString] = content;
                                    if (lines != null)
                                    {
                                        LyricItem item = new LyricItem();
                                        item.groupid = lines.Count;
                                        item.start = stringToMilliInterval(intervalString);
                                        item.text = content;
                                        line.lyrics.Add(item);
                                    }
                                }
                            }
                            if (lines != null && line != null && line.lyrics.Count > 0)
                            {
                                lines.Add(line);
                            }
                        }
                    }
                }
            }
            return md;
        }
        protected static ArrayList sortedKeysInDictionary(Hashtable dictionary)
        {
            String[] av = new String[dictionary.Keys.Count];
            ArrayList al;
            dictionary.Keys.CopyTo(av, 0);
            al = new ArrayList(av);
            al.Sort();
            return al;
        }

        public static String millisec_intervalToString(long interval)
        {
            int min = (int)(interval / 60000L);
            int sec = (int)((interval % 60000L) / 1000L);
            int msec = (int)(interval % 1000L);

            String smin = String.Format("{0:d2}", min);
            String ssec = String.Format("{0:00}.{1:000}", sec, msec);
            return smin + ":" + ssec;
        }
        // sec in double
        public static String intervalToString(double interval)
        {
            int min;
            float sec;
            min = (int)interval / 60;
            sec = (float)(interval - (float)min * 60.0);
            String smin = String.Format("{0:d2}", min);
            String ssec = String.Format("{0:00.00}", sec);
            return smin + ":" + ssec;
        }
        public static int stringToMilliInterval(String str)
        {
            try
            {
                double min = double.Parse(str.Split(':').GetValue(0).ToString());
                double sec = double.Parse(str.Split(':').GetValue(1).ToString());
                return (int)(min * 60000 + sec * 1000);
            }
            catch
            {
                return int.MaxValue;
            }
        }
        protected static double stringToInterval(String str)
        {
            if (str.IndexOf(":") == -1)
            {
                return uint.MaxValue;
            }
            try
            {
                double min = double.Parse(str.Split(':').GetValue(0).ToString());
                double sec = double.Parse(str.Split(':').GetValue(1).ToString());
                return min * 60.0 + sec;
            }
            catch
            {
                return uint.MaxValue;
            }
        }
        #endregion
        #region refresh functions

        public void Refresh(double time)
        {
            if (time - (double)offset / 1000.0 >= stringToInterval(timeArray[currentIndex].ToString()) && currentIndex + 1 < timeArray.Count && time - (double)offset / 1000.0 < stringToInterval(timeArray[currentIndex + 1].ToString()))
            {
                currentLyrics = lyricsDictionary[timeArray[currentIndex]].ToString();
                if (currentIndex + 1 < timeArray.Count)
                    nextLyrics = lyricsDictionary[timeArray[currentIndex + 1]].ToString();
                if (currentIndex - 1 >= 0)
                    previousLyrics = lyricsDictionary[timeArray[currentIndex - 1]].ToString();
            }
            else if (currentIndex + 1 < timeArray.Count && time - (double)offset / 1000.0 >= stringToInterval(timeArray[currentIndex + 1].ToString()) && currentIndex + 2 < timeArray.Count && time - (double)offset / 1000.0 < stringToInterval(timeArray[currentIndex + 2].ToString()))
            {
                currentIndex++;
                currentLyrics = lyricsDictionary[timeArray[currentIndex]].ToString();
                if (currentIndex + 1 < timeArray.Count)
                    nextLyrics = lyricsDictionary[timeArray[currentIndex + 1]].ToString();
                if (currentIndex - 1 >= 0)
                    previousLyrics = lyricsDictionary[timeArray[currentIndex - 1]].ToString();
            }
            else
            {
                int i;
                bool found = false;
                bool overflow=false;
                for (i = 0; i < timeArray.Count; i++)
                {
                    if( i + 1 >= timeArray.Count )
                    {
                        overflow=true;
                    }
                    if (time - (double)offset / 1000.0 >= stringToInterval(timeArray[i].ToString()) && i + 1 < timeArray.Count && time - (double)offset / 1000.0 < stringToInterval(timeArray[i + 1].ToString()))
                    {
                        found = true;
                        currentIndex = i;
                        currentLyrics = lyricsDictionary[timeArray[i]].ToString();
                        if (i + 1 < timeArray.Count)
                            nextLyrics = lyricsDictionary[timeArray[i + 1]].ToString();
                        if (i - 1 >= 0)
                            previousLyrics = lyricsDictionary[timeArray[i - 1]].ToString();
                        break;
                    }
                }
                if(!found && overflow)
                {
                    currentIndex = timeArray.Count - 1;
                    currentLyrics = lyricsDictionary[timeArray[timeArray.Count-1]].ToString();
                    if (timeArray.Count - 1 >= 0)
                        previousLyrics = lyricsDictionary[timeArray[timeArray.Count - 1]].ToString();
                    nextLyrics = "";
                }
            }
        }

        public void Refresh(String aString)
        {
            Refresh(stringToInterval(aString));
        }
        #endregion
        #region directly get lyrics functions

        public String Lyrics(double time)
        {
            try
            {
                String strkey=intervalToString(time - (double)offset / 1000.0);
                if (!lyricsDictionary.ContainsKey(strkey))
                {
                    return null;
                }
                return lyricsDictionary[strkey].ToString();
            }
            catch(Exception ee)
            {
                return null;
            }
        }

        public String Lyrics(String aString)
        {
            try
            {
                return lyricsDictionary[intervalToString(stringToInterval(aString) - (double)offset / 1000.0)].ToString();
            }
            catch
            {
                return null;
            }
        }

        public String LyricsAtIndex(int index)
        {
            try
            {
                return lyricsDictionary[timeArray[index]].ToString();
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
        
}
