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
    public class RemoteRealtimeStreamPlayer : IPlayer, IDisposable
    {
        public event EventHandler<double> ProgressUpdated;
        public event EventHandler Finished;
        private Device ApplicationDevice = null;
        private MediaServer.MediaNode currentNode=null;
        MediaNodeWaveStreamWrapper streamWrapper;
        String remoteServerKey;
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
        MediaNodeWaveFormatWrapper nodeWaveFormatWrapper;

        public bool SetDataSource(String key)
        {
            
            if (this.remoteServerKey != null)
            {
                if (ApplicationStreamedSound.Playing)
                {
                    ApplicationStreamedSound.Stop();
                    ApplicationStreamedSound.Terminate();
                    ApplicationStreamedSound = null;
                }
            }
            this.currentNode.OnStreamBytePushed += currentNode_OnStreamBytePushed;
            this.remoteServerKey = key;
            nodeWaveFormatWrapper = new MediaNodeWaveFormatWrapper(this.currentNode, key);
            streamWrapper = new MediaNodeWaveStreamWrapper(this.currentNode, key);
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
        public RemoteRealtimeStreamPlayer(Control owner,MediaServer.MediaNode node)
        {
            ApplicationDevice = new Microsoft.DirectX.DirectSound.Device();
            ApplicationDevice.SetCooperativeLevel(owner, Microsoft.DirectX.DirectSound.CooperativeLevel.Priority);
            this.currentNode = node;
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


        }
        public void Stop()
        {
            ResetApplicationStreamedSound();
            this.currentNode.SendServerSubscribeStreamRequest(remoteServerKey, false);
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
            if (remoteServerKey == null) return false;
            ApplicationStreamedSound.Play();
            this.currentNode.SendServerSubscribeStreamRequest(remoteServerKey, true);
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
            this.currentNode.SendServerSubscribeStreamRequest(remoteServerKey, false);
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
    }
}
