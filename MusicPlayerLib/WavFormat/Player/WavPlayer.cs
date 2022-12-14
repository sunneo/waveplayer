using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WavePlayer.WavFormat.Player
{
    public class WavPlayer
    {
        public String FileName;
        public double Duration;
        
        Format.WaveFile Wav;
        Microsoft.DirectX.DirectSound.Device ApplicationDevice;
        public Streaming.StreamedSound ApplicationStreamSound;
        public event EventHandler<double> ProgressUpdated;
        public event EventHandler Finished;
        public WavPlayer(Control control)
        {
            ApplicationDevice = new Microsoft.DirectX.DirectSound.Device();
            ApplicationDevice.SetCooperativeLevel(control, Microsoft.DirectX.DirectSound.CooperativeLevel.Priority);
        }
        private int BaseSamplePeriod = 1;
        public bool SetDataSource(String name)
        {
            if (!File.Exists(name))
            {
                return false;
            }
            if (Path.GetExtension(name).IndexOf("wav", StringComparison.CurrentCultureIgnoreCase) == -1)
            {
                return false;
            }
            this.FileName = name;
            if (this.Wav != null)
            {
                Wav.Dispose();
                Wav = null;
                if (ApplicationStreamSound.Playing)
                {
                    ApplicationStreamSound.Stop();
                    ApplicationStreamSound.Terminate();
                    ApplicationStreamSound = null;
                }
            }
            Wav = Format.WaveFile.Parse(new FileStream(name,FileMode.Open, FileAccess.Read));
            BaseSamplePeriod = (Wav.Channels*Wav.SampleRate*(Wav.BitDepth/8));
            Duration = ((double)Wav.DataSize)/BaseSamplePeriod;
            ApplicationStreamSound = new Streaming.StreamedSound(
                this.ApplicationDevice, 
                Wav.DataStream.BaseStream, 
                SoundBuffer.SoundUtil.CreateWaveFormat(Wav.SampleRate,(short)Wav.BitDepth,(short)Wav.Channels)
            );
            ApplicationStreamSound.BufferNotification += ApplicationStreamSound_BufferNotification;
            ApplicationStreamSound.SoundFinishedListener += ApplicationStreamSound_SoundFinishedListener;
            return true;
        }
        private void ResetApplicationStreamedSound()
        {
            if (ApplicationStreamSound != null)
            {
                if (ApplicationStreamSound.Playing)
                {
                    ApplicationStreamSound.Stop();
                    ApplicationStreamSound.Terminate();
                }
            }
            ApplicationStreamSound = null;
            
            Wav.Position = 0;
            
            ApplicationStreamSound = new Streaming.StreamedSound(
               this.ApplicationDevice,
               Wav.DataStream.BaseStream,
               SoundBuffer.SoundUtil.CreateWaveFormat(Wav.SampleRate, (short)Wav.BitDepth, (short)Wav.Channels)
            );
            ApplicationStreamSound.BufferNotification += ApplicationStreamSound_BufferNotification;
            ApplicationStreamSound.SoundFinishedListener += ApplicationStreamSound_SoundFinishedListener;
         
        }
        public long Position
        {
            get
            {
                if (Wav == null)
                {
                    return 0;
                }
                return Wav.Position/BaseSamplePeriod;
            }
            set
            {
                if (Wav == null) return;
                Wav.Position = value * BaseSamplePeriod;
            }
        }
        void ApplicationStreamSound_SoundFinishedListener(object sender, EventArgs e)
        {
            ResetApplicationStreamedSound();
            if (Finished != null)
            {
                Finished(this, EventArgs.Empty);
            }
        }

        void ApplicationStreamSound_BufferNotification(object sender, SoundBuffer.BufferNotificationEventArgs e)
        {
            if (ProgressUpdated != null)
            {
                ProgressUpdated(this, ((double)Wav.Position) / BaseSamplePeriod);
            }
        }
        public void Stop()
        {
            ResetApplicationStreamedSound();
        }
        public bool Play()
        {
            if (ApplicationStreamSound == null)
            {
                return false;
            }
            if(ApplicationStreamSound.Playing)
            {
                return true ;
            }
            if (Wav == null) return false;
            ApplicationStreamSound.Play();
            return true;
        }
        public bool Playing
        {
            get
            {
                if (ApplicationStreamSound == null)
                {
                    return false;
                }
                return ApplicationStreamSound.Playing;
            }
        }
        public void Pause()
        {
            if (null != ApplicationStreamSound)
                ApplicationStreamSound.Stop();
        }
        public void Close()
        {
            this.Stop();
            if (ApplicationStreamSound != null)
            {
                if (ApplicationStreamSound.Playing)
                {
                    ApplicationStreamSound.Stop();
                    ApplicationStreamSound.Terminate();
                }
            }
            if (this.Wav != null)
            {
                Wav.Dispose();
                Wav = null;
            }
        }
    }

}
