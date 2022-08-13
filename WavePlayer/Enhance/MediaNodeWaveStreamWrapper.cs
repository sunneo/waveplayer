using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace WavePlayer.Enhance
{
    public class MediaNodeWaveStreamWrapper:System.IO.Stream
    {
        MediaServer.MediaNode Node;
        String key;
        Locker mlocker = new Locker();
        Locked<long> Reader = 0;
        Locked<long> Writer = 0;
        MemoryStream mStream = new MemoryStream();
        public MediaNodeWaveStreamWrapper(MediaServer.MediaNode node, String key)
        {
            this.Node = node;
            this.key = key;
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
                mStream.Position = Reader;
                int ret = mStream.Read(buffer, offset, count);
                Reader = mStream.Position;
                return ret;
            }
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            return 0;
        }

        public override void SetLength(long value)
        {
            mStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            using (var locker = mlocker.Lock())
            {
                this.mStream.Position = Writer;
                this.mStream.Write(buffer, offset, count);
                Writer = this.mStream.Position;
            }
        }
    }
}
