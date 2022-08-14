using Microsoft.DirectX.DirectSound;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WavePlayer.Interfaces;
using WavePlayer.WavPlayer.SoundBuffer;
using WavePlayer.WavPlayer.Streaming;

namespace WavePlayer.Enhance
{
    public class RealtimeStreamPlayer : IPlayer, IDisposable
    {
        public event EventHandler<double> ProgressUpdated;
        public event EventHandler<String> StreamFileFinished;
        public event EventHandler Finished;

        Dictionary<String, Stream> StreamFileMap = new Dictionary<string, Stream>();
        Dictionary<Stream, String> StreamFileMapInv = new Dictionary<Stream, String>();
        public List<String> GetStreamFileList()
        {
            List<String> ret = new List<string>();
            if (StreamFileMap == null) return ret;
            if (StreamFileMap.Count == 0) return ret;
            ret.AddRange(StreamFileMap.Keys.ToList());
            return ret;
        }
        private Device ApplicationDevice = null;
        
        MultiStreamWrapper streamWrapper;
        
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
                    return null;
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
        MemoryStreamFormatWrapper nodeWaveFormatWrapper;
        public bool AddMP3File(String name)
        {
            if(streamWrapper == null)
            {
                return false;
            }
            if (!File.Exists(name))
            {
                return false;
            }
            if (Path.GetExtension(name).IndexOf("mp3", StringComparison.CurrentCultureIgnoreCase) == -1)
            {
                return false;
            }
            var stream = new NAudio.Wave.Mp3FileReader(name);
            StreamFileMap[name] = stream;
            StreamFileMapInv[stream] = name;
            streamWrapper.AddStream(stream);
            
            return true;
        }
        public void RemoveMP3File(String file)
        {
            if (!StreamFileMap.ContainsKey(file)) return;
            var stream = StreamFileMap[file];
            streamWrapper.RemoveStream(stream);
            StreamFileMapInv.Remove(stream);
            StreamFileMap.Remove(file);
        }
        public bool Start()
        {

            if (ApplicationStreamedSound!=null && ApplicationStreamedSound.Playing)
            {
                ApplicationStreamedSound.Stop();
                ApplicationStreamedSound.Terminate();
                ApplicationStreamedSound = null;
            }

            this.Duration = -1;
            this.Channels = nodeWaveFormatWrapper.Channels;
            this.SampleRate = nodeWaveFormatWrapper.SampleRate;
            this.BitsPerSample = nodeWaveFormatWrapper.BitsPerSample;
            if (BitsPerSample == 0)
            {
                BitsPerSample = 16;
            }
            BaseSamplePeriod = (Channels * SampleRate * (BitsPerSample / 8));
            ApplicationStreamedSound = new WavePlayer.WavPlayer.Streaming.StreamedSound(
                this.ApplicationDevice,
                streamWrapper,
                WavePlayer.WavPlayer.SoundBuffer.SoundUtil.CreateWaveFormat(SampleRate, (short)BitsPerSample, (short)Channels)
            );
            ApplicationStreamedSound.BufferNotification += ApplicationStreamedSound_BufferNotification;
            ApplicationStreamedSound.SoundFinishedListener += ApplicationStreamedSound_SoundFinishedListener;
            return true;
        }

        void currentNode_OnStreamBytePushed(object sender, byte[] e)
        {
            streamWrapper.Write(e, 0, e.Length);
        }

        void ApplicationStreamedSound_SoundFinishedListener(object sender, EventArgs e)
        {
           
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
                return 0;
            }
            set
            {
                
            }
        }
        public RealtimeStreamPlayer(Control owner)
        {
            ApplicationDevice = new Microsoft.DirectX.DirectSound.Device();
            ApplicationDevice.SetCooperativeLevel(owner, Microsoft.DirectX.DirectSound.CooperativeLevel.Priority);

            nodeWaveFormatWrapper = new MemoryStreamFormatWrapper();
            streamWrapper = new MultiStreamWrapper();
            streamWrapper.SteamRemoved += StreamWrapper_SteamRemoved;
        }

        private void StreamWrapper_SteamRemoved(object sender, Stream e)
        {
            if (!StreamFileMapInv.ContainsKey(e)) return;
            String file = StreamFileMapInv[e];
            StreamFileMapInv.Remove(e);
            StreamFileMap.Remove(file);
            StreamFileFinished(this, file);
        }

        private void ResetApplicationStreamedSound()
        {
            if (ApplicationStreamedSound != null)
            {
                if (ApplicationStreamedSound.Playing)
                {
                    ApplicationStreamedSound.Stop();
                    ApplicationStreamedSound.Terminate();
                }
            }
            ApplicationStreamedSound = new WavePlayer.WavPlayer.Streaming.StreamedSound(
                   this.ApplicationDevice,
                   streamWrapper,
                   SoundUtil.CreateWaveFormat(SampleRate, (short)this.BitsPerSample, (short)Channels)
                );
            ApplicationStreamedSound.BufferNotification += ApplicationStreamedSound_BufferNotification;
            ApplicationStreamedSound.SoundFinishedListener += ApplicationStreamedSound_SoundFinishedListener;
            //streamWrapper.Clear();

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

        public bool SetDataSource(string name)
        {
            Start();
            AddMP3File(name);
            return true;
        }
    }
}
