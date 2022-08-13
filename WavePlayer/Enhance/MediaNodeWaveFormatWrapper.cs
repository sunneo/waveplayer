using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.Enhance
{
    public class MediaNodeWaveFormatWrapper
    {
        String key = "";
        MediaServer.MediaNode node;
        public MediaNodeWaveFormatWrapper(MediaServer.MediaNode node, String key)
        {
            this.node = node;
            this.key = key;
            UpdateWaveFormat();
        }
        public void UpdateWaveFormat()
        {
            int[] description = node.SendServerGetStreamWaveFormatRequest(key);
            Channels = description[0];
            SampleRate = description[1];
            BitsPerSample = description[2];
        }
        public int Channels { get; private set; }
        public int SampleRate { get; private set; }
        public int BitsPerSample { get; private set; }
    }
}
