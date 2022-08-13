using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TrackServer.Server
{
    public class MediaServerInstance
    {
        public event EventHandler ConnectionClosed;
        public Socket RawConnection
        {
            get;
            private set;
        }
        public String IPAddress
        {
            get;
            private set;
        }
        public MediaServerInstance(Socket sck)
        {
            this.RawConnection = sck;
            this.IPAddress = (sck.RemoteEndPoint as System.Net.IPEndPoint).Address.ToString();
        }
        

    }
}
