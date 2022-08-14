using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.Interfaces
{
    public class EffectEventArgs:EventArgs
    {
        public byte[] NewSoundByte;
        public Stream Stream;
        public int NumBytesRequired;
        public static EffectEventArgs Empty
        {
            get
            {
                return new EffectEventArgs();
            }
        }
    }
}
