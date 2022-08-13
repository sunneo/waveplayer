using Microsoft.DirectX.DirectSound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WavePlayer.WavPlayer.Streaming;
using System.IO;
using WavePlayer.WavPlayer.SoundBuffer;
using Utilities;

namespace WavePlayer.Enhance
{
    public class MP3Player : Interfaces.IPlayer, IDisposable
    {
        public event EventHandler<double> ProgressUpdated;
        public event EventHandler Finished;
        private Device ApplicationDevice = null;
        private NAudio.Wave.Mp3FileReader currentMp3Stream;
        private StreamedSound ApplicationStreamedSound = null;
        public string FileName
        {
            get;
            set;
        }
        public IList<Interfaces.IEffectOperator> Effects
        {
            get
            {
                if (ApplicationStreamedSound == null)
                {
                    return new List<Interfaces.IEffectOperator>();
                }
                return ApplicationStreamedSound.Effects;
            }
        }
        public double Duration
        {
            get;
            set;
        }
        private int BaseSamplePeriod = 1;
        public int Channels { get; private set; }
        public int SampleRate { get; private set; }
        public int BitsPerSample { get; private set; }
        public bool SetDataSource(string name)
        {
            if (!File.Exists(name))
            {
                return false;
            }
            if (Path.GetExtension(name).IndexOf("mp3", StringComparison.CurrentCultureIgnoreCase) == -1)
            {
                return false;
            }
            this.FileName = name;
            if (this.currentMp3Stream != null)
            {
                currentMp3Stream.Dispose();
                currentMp3Stream = null;
                if (ApplicationStreamedSound.Playing)
                {
                    ApplicationStreamedSound.Stop();
                    ApplicationStreamedSound.Terminate();
                    ApplicationStreamedSound = null;
                }
            }
            currentMp3Stream = new NAudio.Wave.Mp3FileReader(name);
            this.Duration = currentMp3Stream.TotalTime.TotalSeconds;
            this.Channels = currentMp3Stream.WaveFormat.Channels;
            this.SampleRate = currentMp3Stream.WaveFormat.SampleRate;
            this.BitsPerSample = currentMp3Stream.WaveFormat.BitsPerSample;
            if (BitsPerSample == 0)
            {
                BitsPerSample = 16;
            }
            BaseSamplePeriod = (Channels * SampleRate * (BitsPerSample / 8));
            ApplicationStreamedSound = new WavePlayer.WavPlayer.Streaming.StreamedSound(
                this.ApplicationDevice,
                currentMp3Stream,
                WavePlayer.WavPlayer.SoundBuffer.SoundUtil.CreateWaveFormat(SampleRate, (short)BitsPerSample, (short)Channels)
            );
            ApplicationStreamedSound.BufferNotification += ApplicationStreamedSound_BufferNotification;
            ApplicationStreamedSound.SoundFinishedListener += ApplicationStreamedSound_SoundFinishedListener;
            return true;
        }

        void ApplicationStreamedSound_SoundFinishedListener(object sender, EventArgs e)
        {
            ResetApplicationStreamedSound();
            if (Finished != null)
            {
                Finished(this, EventArgs.Empty);
            }
        }

        void ApplicationStreamedSound_BufferNotification(object sender, WavPlayer.SoundBuffer.BufferNotificationEventArgs e)
        {
            if (ProgressUpdated != null)
            {
                ProgressUpdated(this, this.Position);
            }
        }

        public long Position
        {
            get
            {
                if (currentMp3Stream == null)
                {
                    return 0;
                }
                return (long)(currentMp3Stream.Position * ((double)Duration) / currentMp3Stream.Length);
            }
            set
            {
                if (currentMp3Stream != null)
                {
                    long realPos = (long)(value * ((double)currentMp3Stream.Length) / Duration);
                    if (realPos < 0)
                    {
                        currentMp3Stream.Position = 0;
                    }
                    else if (realPos > currentMp3Stream.Length)
                    {
                        currentMp3Stream.Position = currentMp3Stream.Length;
                    }
                    else
                    {
                        currentMp3Stream.Position = realPos;
                    }
                }
            }
        }
        public MP3Player(Control owner)
        {
            ApplicationDevice = new Microsoft.DirectX.DirectSound.Device();
            ApplicationDevice.SetCooperativeLevel(owner, Microsoft.DirectX.DirectSound.CooperativeLevel.Priority);
        }
        private void ResetApplicationStreamedSound()
        {
            try
            {
                if (ApplicationStreamedSound != null)
                {
                    if (ApplicationStreamedSound.Playing)
                    {
                        if (ApplicationStreamedSound != null)
                        {
                            ApplicationStreamedSound.Stop();
                        }
                        if (ApplicationStreamedSound != null)
                        {
                            ApplicationStreamedSound.Terminate();
                        }
                        
                    }
                }
            }
            catch(Exception ee)
            {
                Tracer.D("[以下錯誤已try-catch忽略]");
                Tracer.D(ee.ToString());
                Tracer.D("=======================================");
            }
            ApplicationStreamedSound = null;
            try
            {
                if (currentMp3Stream != null)
                {
                    currentMp3Stream.Position = 0;

                    ApplicationStreamedSound = new WavePlayer.WavPlayer.Streaming.StreamedSound(
                       this.ApplicationDevice,
                       currentMp3Stream,
                       SoundUtil.CreateWaveFormat(SampleRate, (short)this.BitsPerSample, (short)Channels)
                    );
                    ApplicationStreamedSound.BufferNotification += ApplicationStreamedSound_BufferNotification;
                    ApplicationStreamedSound.SoundFinishedListener += ApplicationStreamedSound_SoundFinishedListener;
                }
            }
            catch(Exception ee)
            {
                Tracer.D("[以下錯誤已try-catch忽略]");
                Tracer.D(ee.ToString());
                Tracer.D("=======================================");
            }


        }
        public void Stop()
        {
            ResetApplicationStreamedSound();
        }

        public bool Play()
        {
            if (ApplicationStreamedSound == null)
            {
                return false;
            }
            if (ApplicationStreamedSound.Playing)
            {
                return true;
            }
            if (currentMp3Stream == null) return false;
            ApplicationStreamedSound.Play();
            return true;
        }

        public bool Playing
        {
            get
            {
                {
                    if (ApplicationStreamedSound == null)
                    {
                        return false;
                    }
                    return ApplicationStreamedSound.Playing;
                }
            }
        }

        public void Pause()
        {
            if (null != ApplicationStreamedSound)
                ApplicationStreamedSound.Stop();
        }

        public void Close()
        {
            this.Stop();
            if (ApplicationStreamedSound != null)
            {
                if (ApplicationStreamedSound.Playing)
                {
                    ApplicationStreamedSound.Stop();
                    ApplicationStreamedSound.Terminate();
                }
            }
            if (this.currentMp3Stream != null)
            {
                currentMp3Stream.Dispose();
                currentMp3Stream = null;
            }
        }

        public void Dispose()
        {
            Close();
            if (ApplicationDevice != null)
            {
                //this.ApplicationDevice.Dispose();
                this.ApplicationDevice = null;
            }
        }
        public bool HasVideo
        {
            get { return false; }
        }

        public void AttachCanvas(Interfaces.ICanvas canvas)
        {
        }
    }
}
