using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace WavePlayer.Enhance
{
    public class MultiStreamWrapper : System.IO.Stream
    {
        Locker mlocker = new Locker();
        Locked<long> Reader = 0;
        Locked<long> Writer = 0;
        LinkedList<Stream> streams = new LinkedList<Stream>();
        public event EventHandler<Stream> SteamRemoved;
        public void Clear()
        {
            using (var locker = mlocker.Lock())
            {
                streams.Clear();
            }
        }
        public void AddStream(Stream stream)
        {
            using (var locker = mlocker.Lock())
            {
                streams.AddLast(stream);
            }
        }
        public void RemoveStream(Stream stream)
        {
            using (var locker = mlocker.Lock())
            {
                streams.Remove(stream);
            }
        }
        public MultiStreamWrapper()
        {
        }
        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {

        }

        public override long Length
        {
            get { return long.MaxValue; }
        }

        public override long Position
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            using (var locker = mlocker.Lock())
            {
                byte[] secondBuf = new byte[count];
                LinkedListNode<Stream> streamNode = streams.First;
                int bytesRead = 0;
                while (streamNode != null)
                {
                    LinkedListNode<Stream> next = streamNode.Next;
                    Stream stream = streamNode.Value;
                    bool doRemove = false;
                    if(stream == null)
                    {
                        doRemove = true;                       
                    }
                    if (stream.Length - stream.Position > count)
                    {
                        int readFromThisStream = stream.Read(secondBuf, 0, count);
                        bytesRead = Math.Max(bytesRead, readFromThisStream);
                        if (readFromThisStream > 0)
                        {
                            Sum32BitAudio(buffer, offset, secondBuf, readFromThisStream);
                        }
                    }
                    else
                    {
                        bytesRead = Math.Max(bytesRead, count);
                        stream.Position += count;
                        doRemove = true;
                    }
                    if (doRemove)
                    {
                        streams.Remove(streamNode);
                    }
                    streamNode = next;
                }
                return bytesRead;
            }
        }
        /// <summary>
        /// Actually performs the mixing
        /// </summary>
        unsafe void Sum32BitAudio(byte[] destBuffer, int offset, byte[] sourceBuffer, int bytesRead)
        {
            fixed (byte* pDestBuffer = &destBuffer[offset],
                          pSourceBuffer = &sourceBuffer[0])
            {
                short* pfDestBuffer = (short*)pDestBuffer;
                short* pfReadBuffer = (short*)pSourceBuffer;
                int samplesRead = bytesRead / 2;
                int cnt = streams.Count;
                if (cnt <= 0) cnt = 1;
                for (int n = 0; n < samplesRead; n++)
                {
                    short val1 = pfDestBuffer[n];
                    short val2 = pfReadBuffer[n];
                    int newVal = (int)val1 + (int)val2;
                    if (newVal > short.MaxValue)
                    {
                        newVal = short.MaxValue - 1;
                    }
                    else if (newVal < short.MinValue)
                    {
                        newVal = short.MinValue + 1;
                    }
                    pfDestBuffer[n] = (short)newVal;
                }
            }
        }
        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            return 0;
        }

        public override void SetLength(long value)
        {
            
        }

        public override void Write(byte[] buffer, int offset, int count)
        {

        }
    }
}
