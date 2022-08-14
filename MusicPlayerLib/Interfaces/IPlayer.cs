using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WavePlayer.WavPlayer.SoundBuffer;

namespace WavePlayer.Interfaces
{
    public interface IPlayer:IDisposable
    {
        /// <summary>
        /// event to notify progress has updated
        /// </summary>
        event EventHandler<double> ProgressUpdated;
        /// <summary>
        /// event to notify a music file has arrive end of file.
        /// </summary>
        event EventHandler Finished;
        /// <summary>
        /// effects
        /// </summary>
        IList<IEffectOperator> Effects { get; }
        /// <summary>
        /// information: channel count
        /// </summary>
        int Channels { get; }
        /// <summary>
        /// information: sample rate
        /// </summary>
        int SampleRate { get;  }
        int BitsPerSample { get; }
        String FileName { get; set; }
        double Duration { get; set; }
        bool SetDataSource(String name);
        long Position { get; set; }
        void Stop();
        bool Play();
        bool Playing { get;  }
        void Pause();
        void Close();
        bool HasVideo { get; }
        void AttachCanvas(ICanvas canvas);
    }
}
