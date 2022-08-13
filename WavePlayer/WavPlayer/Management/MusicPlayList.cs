using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WavePlayer.WavPlayer.Management
{
    public class MusicPlayList
    {
        public List<MusicListItem> Items = new List<MusicListItem>();
        public String FileName = "";
        private MusicPlayList(String filename)
        {
            this.FileName = filename;
        }
        Regex regex = new Regex("'[^\\']+'", RegexOptions.Singleline | RegexOptions.Compiled);
        void GetDurationStringFromFileName(ref MusicListItem ret, String filePath)
        {
            if (String.IsNullOrEmpty(ret.FileName)) return;
            if (!String.IsNullOrEmpty(this.mLocationTag)) return; // from remote
            if (ret.FileName.IndexOf(":\\") == -1)
            {
                ret.FileFullPath = Path.Combine(Path.GetDirectoryName(this.FileName), ret.FileName);
            }
            if (!File.Exists(ret.FileFullPath)) return;
            if (Path.GetExtension(filePath).Equals(".wav", StringComparison.InvariantCultureIgnoreCase))
            {
                using (FileStream fs = new FileStream(ret.FileFullPath, FileMode.Open, FileAccess.Read))
                using (WavFormat.Format.WaveFile parser = WavFormat.Format.WaveFile.Parse(fs))
                {
                    ret.Duration = parser.DataSize / (parser.Channels * parser.SampleRate * (parser.BitDepth / 8));
                    ret.DurationString = TimeSpan.FromSeconds(ret.Duration).ToSimpleString();
                }
            }
            else if (Path.GetExtension(ret.FileName).Equals(".mp3", StringComparison.InvariantCultureIgnoreCase))
            {
                using (FileStream fs = new FileStream(ret.FileFullPath, FileMode.Open, FileAccess.Read))
                using (NAudio.Wave.Mp3FileReader reader = new NAudio.Wave.Mp3FileReader(fs))
                {
                    ret.Duration = (int)reader.TotalTime.TotalSeconds;
                    ret.DurationString = TimeSpan.FromSeconds(ret.Duration).ToSimpleString();
                }
            }
        }
        private MusicListItem ParseLineLst(String line)
        {
            MusicListItem ret = null;
            MatchCollection collection = regex.Matches(line);
            if (collection.Count > 0)
            {
                ret = new MusicListItem();
                ret.FileFullPath = ret.FileName = collection[0].Value.Trim('\'');
                String filePath = ret.FileName;
                GetDurationStringFromFileName(ref ret, filePath);
                if (collection.Count > 1)
                    ret.Name = collection[1].Value.Trim('\'');
                if (collection.Count > 2)
                    ret.Artist = collection[2].Value.Trim('\'');
                if (collection.Count > 3)
                    ret.Album = collection[3].Value.Trim('\'');
                if (collection.Count > 4)
                {
                    ret.CoverPicturePath = collection[4].Value.Trim('\'');
                    if (ret.CoverPicturePath.IndexOf(":\\") == -1)
                    {
                        ret.CoverPictureFullPath = Path.Combine(Path.GetDirectoryName(this.FileName), ret.CoverPicturePath);
                    }
                }
                return ret;
            }
            return null;
        }
        private void ParseLst(String file)
        {
            Items.Clear();
            String[] lines = File.ReadAllLines(this.FileName);
            for (int i = 0; i < lines.Length; ++i)
            {
                String line = lines[i];
                if (String.IsNullOrEmpty(line)) continue;
                MusicListItem item = ParseLineLst(line);
                if (item != null)
                {
                    Items.Add(item);
                }
            }
        }
        public class JSONMusicListItem
        {
            public String Filename;
            public String Title;
            public String Author;
            public String Album;
            public String Duration;
            public String Cover;
        }
        public void SaveToJSONFile(String filename)
        {
            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    JsonWriter writer = new JsonTextWriter(sw) { Formatting = Formatting.Indented };
                    JsonSerializer serializer = new JsonSerializer();
                    List<JSONMusicListItem> objs = new List<JSONMusicListItem>();
                    for (int i = 0; i < this.Items.Count; ++i)
                    {
                        JSONMusicListItem item = new JSONMusicListItem();
                        MusicListItem thisItem = this.Items[i];
                        item.Filename = thisItem.FileFullPath;
                        item.Title = thisItem.Name;
                        item.Author = thisItem.Artist;
                        item.Album = thisItem.Album;
                        item.Cover = thisItem.CoverPicturePath;
                        objs.Add(item);
                    }
                    serializer.Serialize(writer, objs);
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
        }
        public MusicListItem AddFile(String filePath)
        {
            return AddFile(filePath, null);
        }
        public MusicListItem AddFile(String filePath, JSONMusicListItem jsonItem)
        {
            MusicListItem item = new MusicListItem();
            item.FileFullPath = item.FileName = filePath;
            if (jsonItem == null || String.IsNullOrEmpty(jsonItem.Duration))
            {
                GetDurationStringFromFileName(ref item, filePath);
            }
            else
            {
                int.TryParse(jsonItem.Duration, out item.Duration);
                item.DurationString = TimeSpan.FromSeconds(item.Duration).ToSimpleString();
            }
            if (!String.IsNullOrEmpty(mLocationTag))
            {
                item.RemoteLocation = mLocationTag;
                item.FromRemote = true;
            }
            if (jsonItem != null)
            {
                item.Album = jsonItem.Album;
                item.Artist = jsonItem.Author;
                item.Name = jsonItem.Title;
                item.CoverPictureFullPath = item.CoverPicturePath = jsonItem.Cover;
            }
            if (!String.IsNullOrEmpty(item.CoverPicturePath))
            {
                if (item.CoverPicturePath.IndexOf(":\\") == -1)
                {
                    if (!string.IsNullOrEmpty(this.FileName))
                    {
                        item.CoverPictureFullPath = Path.Combine(Path.GetDirectoryName(this.FileName), item.CoverPicturePath);
                    }
                }
            }
            Items.Add(item);
            return item;
        }

        private void ParseJsonFromStreamReader(TextReader sr)
        {
            Items.Clear();
            {
                JsonReader reader = new JsonTextReader(sr);
                JsonSerializer serializer = new JsonSerializer();
                List<JSONMusicListItem> objs = new List<JSONMusicListItem>();
                try
                {
                    objs = serializer.Deserialize<List<JSONMusicListItem>>(reader);
                    if (objs == null) return;
                    for (int i = 0; i < objs.Count; ++i)
                    {
                        JSONMusicListItem jsonItem = objs[i];
                        if (jsonItem == null) continue;
                        String filePath = jsonItem.Filename;
                        AddFile(filePath, jsonItem);
                    }
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee.ToString());
                }
            }
        }
        private void Parse()
        {
            if (String.IsNullOrEmpty(this.FileName)) return;
            if (!File.Exists(this.FileName)) return;
            String ext = Path.GetExtension(this.FileName).ToLower();
            if (ext.Equals(".lst", StringComparison.InvariantCultureIgnoreCase))
            {
                ParseLst(this.FileName);
            }
            else if (ext.Equals(".json", StringComparison.InvariantCultureIgnoreCase))
            {
                using (StreamReader sr = new StreamReader(this.FileName))
                {
                    this.ParseJsonFromStreamReader(sr);
                }
            }

        }
        public static MusicPlayList FromFile(String filename)
        {
            MusicPlayList ret = new MusicPlayList(filename);
            ret.Parse();
            return ret;
        }
        private String mLocationTag = "";
        public static MusicPlayList FromString(String filename, String locationTag = "")
        {
            MusicPlayList ret = new MusicPlayList("");

            ret.mLocationTag = locationTag;
            using (StringReader sr = new StringReader(filename))
            {
                ret.ParseJsonFromStreamReader(sr);
            }
            return ret;
        }
    }
}
