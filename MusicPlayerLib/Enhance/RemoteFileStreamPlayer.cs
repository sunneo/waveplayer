using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WavePlayer.Interfaces;

namespace WavePlayer.Enhance
{
    public class RemoteFileStreamPlayer:IPlayer,IDisposable
    {
        public event EventHandler<double> ProgressUpdated;

        public event EventHandler Finished;

        MediaServer.MediaNode Node;
        String key;

        public RemoteFileStreamPlayer(MediaServer.MediaNode Node,String key)
        {
            this.Node = Node;
            this.key = key;
        }


        public IList<IEffectOperator> Effects
        {
            get { throw new NotImplementedException(); }
        }

        public int Channels
        {
            get { throw new NotImplementedException(); }
        }

        public int SampleRate
        {
            get { throw new NotImplementedException(); }
        }

        public int BitsPerSample
        {
            get { throw new NotImplementedException(); }
        }

        public string FileName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double Duration
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool SetDataSource(string name)
        {
            throw new NotImplementedException();
        }

        public long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public bool Play()
        {
            throw new NotImplementedException();
        }

        public bool Playing
        {
            get { throw new NotImplementedException(); }
        }

        public void Pause()
        {
           
        }

        public void Close()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}
