using System;
using System.IO;
using Microsoft.DirectX.DirectSound;
using WavePlayer.WavFormat.Format;
using WavePlayer.SoundBuffer;
namespace WavePlayer.Streaming
{

    /// <summary>
    /// Plays streamed PCM-format sounds.
    /// </summary>
    public class StreamedSound
    {
        public StreamedSound(Device device, Stream stream, Microsoft.DirectX.DirectSound.WaveFormat waveFormat)
        {
            Device = device; 
            Stream = stream; 
            WaveFormat = waveFormat;
            this.Closed = false;
        }
        Boolean Closed = true;
        private Device Device;

        /// <summary>
        /// Gets or sets the source stream used to provide PCM-encoded bytes.
        /// </summary>
        public Stream Stream
        {
            get { return StreamRep; }
            set { StreamRep = value; this.Closed = false; }
        }
        private Stream StreamRep = null;

        private void InitBuffer()
        {
            OnBufferInitializing();
            if (ERSB == null)
            {
                ERSB = new EventRaisingSoundBuffer(Device, WaveFormat, BufferLength);
                ERSB.BufferNotification += new BufferNotificationEventHandler(OnBufferNotification);
                ERSB.PlayFinishedHandler += this.ERSBFinishAdapter;
            }
            else
            {
                ERSB.WaveFormat = WaveFormat;
                ERSB.BufferLength = BufferLength;
            }
            OnBufferInitialized();
        }

        protected virtual void OnBufferInitializing() { }
        protected virtual void OnBufferInitialized() { }

        public WaveFormat WaveFormat
        {
            get { return WaveFormatRep; }
            set { WaveFormatRep = value; if (ERSB != null) ERSB.WaveFormat = value; }
        }
        public WaveFormat WaveFormatRep;

        public TimeSpan BufferLength
        {
            get { return BufferLengthRep; }
            set { BufferLengthRep = value; if (ERSB != null) ERSB.BufferLength = value; }
        }
        private TimeSpan BufferLengthRep = TimeSpan.FromMilliseconds(300);

        /// <summary>
        /// The event-raising sound buffer used by the streamed sound.
        /// </summary>
        protected EventRaisingSoundBuffer ERSB;

        public bool Playing
        {
            get
            {
                if (ERSB == null)
                {
                    return false;
                }
                return ERSB.Playing;
            }
            set
            {
                if (value) Play(); else Stop();
            }
        }
        private Boolean LoopingRep = false;
        public bool Looping
        {
            get
            {
                if (ERSB == null)
                {
                    return false;
                }
                return LoopingRep && ERSB.Playing;
            }
            set
            {
                LoopingRep = value;
                if (value)
                {
                    Loop();
                }
                else Stop();
            }
        }

        public Boolean isFinished()
        {
            return soundFinished;
        }
        public event EventHandler SoundFinishedListener;
        Boolean soundFinished = false;

        public class OnBufferFetchedEventArgs : EventArgs
        {
            public OnBufferFetchedEventArgs(int numBytesRequired)
            {
                NumBytesRequiredRep = numBytesRequired;
            }
            public bool SoundFinished { get { return SoundFinishedRep; } set { SoundFinishedRep = value; } }
            private bool SoundFinishedRep = false;
            public int NumBytesRequired { get { return NumBytesRequiredRep; } }
            internal int NumBytesRequiredRep;
            public byte[] NewSoundByte;
        }

        
        public void OnBufferNotification_V1(object sender, BufferNotificationEventArgs e)
        {

            if (e.NewSoundByte == null || e.NewSoundByte.Length < e.NumBytesRequired)
                e.NewSoundByte = new byte[e.NumBytesRequired];
            try
            {
                
                byte[] sampleBytes = new byte[(int)(e.NumBytesRequired)];
                int bytesRead = Stream.Read(sampleBytes, 0, (int)(e.NumBytesRequired));
                /**
                 *  esspecially occur while playing complete
                 */
                if (bytesRead > 0 && bytesRead < e.NumBytesRequired)
                {
                    byte[] trimmedBytes = new byte[e.NumBytesRequired];
                    Array.Copy(e.NewSoundByte, trimmedBytes, bytesRead);
                    e.NewSoundByte = trimmedBytes;
                }
                else if (bytesRead == e.NumBytesRequired)
                {
                    e.NewSoundByte = sampleBytes;
                }

                e.SoundFinished = Stream.Length <= Stream.Position;
                if (BufferNotification != null) BufferNotification(sender, e);
            }
            catch (Exception ee)
            {

                Console.WriteLine(ee);
            }
            if (e.SoundFinished)
            {
                if (SoundFinishedListener != null)
                {
                    SoundFinishedListener(this, EventArgs.Empty);
                }
            }
        }


       
       
        public const int BUFFER_NOTIFY_VERSION = 1;
        public void OnBufferNotification(object sender, BufferNotificationEventArgs e)
        {
            switch (BUFFER_NOTIFY_VERSION)
            {
                default:
                case 1:
                    OnBufferNotification_V1(sender, e);
                    break;
            }
        }

        private void ERSBFinishAdapter(object sender, EventArgs e)
        {

            if (SoundFinishedListener != null)
            {
                SoundFinishedListener.Invoke(sender, new EventArgs());
            }


        }
        /// <summary>
        /// Event that is raised after bytes are added to the buffer.  The event arguments will contain 
        /// any new bytes being provided by the streamed sound.
        /// </summary>
        public event BufferNotificationEventHandler BufferNotification;

        public long Position()
        {
            return Stream.Position;
        }

        public void Seek(long pos)
        {
            Stream.Seek(pos, SeekOrigin.Begin);
        }


        public void Play()
        {
            InitBuffer();
            if (ERSB != null)
            {
                ERSB.Play();
            }

        }

        public void Loop()
        {
            InitBuffer();
            if (ERSB != null)
            {
                ERSB.Play();
            }
        }
        public void Stop()
        {
            if (ERSB != null)
            {
                ERSB.Stop();
            }
        }

        public void Terminate()
        {
            this.Closed = true;
            if (ERSB != null)
            {
                ERSB.terminate();
            }

        }

        public void Rewind()
        {
            if (Playing) Stop();
            Stream.Position = 0;
        }
    }
}