using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace TrackServer.Server
{
    public class ServerHolder:IDisposable
    {
        List<MediaServerInstance> Servers = new List<MediaServerInstance>();
        Socket ServerSck;
        public event EventHandler<MediaServerInstance> OnHandleConnection;
        private void HandleConnection(MediaServerInstance sck)
        {
            AsyncTask task = new AsyncTask(() => {
                if (OnHandleConnection != null)
                {
                    OnHandleConnection(this, sck);
                }
            });
            task.AddAfterFinishJob(() => {
                Servers.Remove(sck);
            });
            task.Start(false);
        }
        public void Start(int Port)
        {
            ServerSck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("0.0.0.0"), Port);
            ServerSck.Bind(ip);
            ServerSck.Listen(128);
            while (true)
            {
                Socket sck = ServerSck.Accept();
                if (sck != null)
                {
                    MediaServerInstance server = new MediaServerInstance(sck);
                    Servers.Add(server);
                    HandleConnection(server);
                }
            }
        }
        public void Stop()
        {
            try
            {
                if (ServerSck != null)
                {
                    ServerSck.Close();
                    ServerSck.Dispose();
                    ServerSck = null;
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
