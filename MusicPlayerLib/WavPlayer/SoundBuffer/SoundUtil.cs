using Microsoft.DirectX.DirectSound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.WavPlayer.SoundBuffer
{
    class SoundUtil
    {
        public static WaveFormat CreateWaveFormat(int samplingRate, short bitsPerSample, short numChannels)
        {
            WaveFormat wf = new WaveFormat();

            wf.FormatTag = WaveFormatTag.Pcm;
            wf.SamplesPerSecond = samplingRate;
            wf.BitsPerSample = bitsPerSample;
            wf.Channels = numChannels;

            wf.BlockAlign = (short)(wf.Channels * (wf.BitsPerSample / 8));
            wf.AverageBytesPerSecond = wf.SamplesPerSecond * wf.BlockAlign;

            return wf;
        }
    }
}
