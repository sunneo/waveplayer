using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.Enhance
{
    public class MemoryStreamFormatWrapper
    {
        
        public MemoryStreamFormatWrapper()
        {
            UpdateWaveFormat();
        }
        public void UpdateWaveFormat()
        {
            Channels = 2;
            SampleRate = 44100;
            BitsPerSample = 16;
        }
        public int Channels { get; private set; }
        public int SampleRate { get; private set; }
        public int BitsPerSample { get; private set; }
    }
}
