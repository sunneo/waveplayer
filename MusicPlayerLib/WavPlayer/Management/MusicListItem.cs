using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.WavPlayer.Management
{
    public class MusicListItem
    {
        public String FileName;
        public String FileFullPath;
        public int Duration;
        public String DurationString;
        public String RemoteLocation;
        public String Name;
        public String Artist;
        public String Album;
        public String CoverPicturePath;
        public String CoverPictureFullPath;
        public volatile bool FromRemote = false;
    }
}
