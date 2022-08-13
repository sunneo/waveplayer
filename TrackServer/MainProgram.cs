using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Server;

namespace TrackServer
{
    public class MainProgram
    {
        ServerHolder mServer;
        IniReader mIniReader;
        int mPort = 9000;
        public event EventHandler<String> OnConsoleMessage;
        public event EventHandler<OnHandleConnectionEventArgs> OnNewConnectionArrived;
        
        void OutputMessageHelper(String msg)
        {
            if (OnConsoleMessage != null)
            {
                OnConsoleMessage(this, msg);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }
        void OutputMessage(String msg, params object[] parms)
        {
            this.OutputMessageHelper(String.Format(msg, parms));
        }
        void OutputMessage(String msg, object parm)
        {
            this.OutputMessageHelper(String.Format(msg, parm));
        }
        void OutputMessage(String msg)
        {
            this.OutputMessageHelper(msg);
        }
        public void Main()
        {
            mIniReader = IniReader.FromFile("Server.ini");
            mPort = mIniReader.GetInt("Port", 9000);
            mServer = new ServerHolder();
            OutputMessage("Server Start at Port:{0}", mPort);
            mServer.OnHandleConnection += mServer_OnHandleConnection;
            mServer.OnServerRemoved += mServer_OnServerRemoved;
            mServer.StartAsync(mPort);
        }

        void mServer_OnServerRemoved(object sender, OnHandleConnectionEventArgs e)
        {
            if (!e.Current.Attributes.GetAttributeBool("IsSecondConnection"))
            {
                OnServerCountUpdated(this.mServer);
            }
        }

        
        public class ServerHandlerArgs:EventArgs
        {
            public ServerHolder Server;
            public MediaConnectionInstance Current;
            public BinaryWriter Writer
            {
                get
                {
                    if (Current == null)
                    {
                        return null;
                    }
                    return Current.Writer;
                }
            }
            public BinaryReader Reader
            {
                get
                {
                    if (Current == null)
                    {
                        return null;
                    }
                    return Current.Reader;
                }
            }
            public ServerHandlerArgs(ServerHolder Server)
            {
                this.Server = Server;
            }
        }
        protected virtual void OnServerCountUpdated(ServerHolder holder)
        {
            int cnt = holder.Servers.Count;
            
            for (int i = 0; i < cnt; ++i)
            {
                if (holder.Servers[i].Attributes.GetAttributeBool("IsSecondConnection"))
                {
                    using (var locker = holder.Servers[i].BeginWriterLocker())
                    {
                        holder.Servers[i].Writer.Write((int)ServerFunctionCodes.Constants.TRACKER_NOTIFICATION_UPDATE_LIST);
                        SendServerList(new OnHandleConnectionEventArgs(holder, holder.Servers[i]));
                    }
                }
            }
        }
        private void mServerHandleRegistryMediaServer(OnHandleConnectionEventArgs e)
        {
            e.Server.Registry(e.Current);
            using (var locker = e.Current.BeginReaderLocker())
            {
                int PID = e.Current.Reader.ReadInt32();
                e.Current.Attributes.SetAttribute("PID", PID);
                bool IsSecondConnection = e.Current.Reader.ReadBoolean();
                e.Current.Attributes.SetAttribute("IsSecondConnection", IsSecondConnection);
            }
            using (var locker = e.Current.BeginWriterLocker())
            {
                e.Current.Writer.Write((int)0);
                e.Current.Writer.Flush();
            }

        }
        private void SendServerList(OnHandleConnectionEventArgs e)
        {
            int cnt = e.Server.Servers.Count;
            List<MediaConnectionInstance> firstConnections = new List<MediaConnectionInstance>();
            for (int i = 0; i < cnt; ++i)
            {
                if (e.Server.Servers[i].Attributes.GetAttributeBool("IsSecondConnection")) continue;
                if (e.Server.Servers[i].Attributes.GetAttributeInt("PID") == e.Current.Attributes.GetAttributeInt("PID")) continue;
                firstConnections.Add(e.Server.Servers[i]);
            }
            cnt = firstConnections.Count;
            using (var locker = e.Current.BeginWriterLocker())
            {
                e.Current.Writer.Write((int)cnt);
                for (int i = 0; i < cnt; ++i)
                {
                    e.Current.Writer.Write(firstConnections[i].IPAddress);
                    e.Current.Writer.Write(firstConnections[i].Attributes.GetAttributeInt("Port"));
                    e.Current.Writer.Write(firstConnections[i].Attributes.GetAttributeInt("PID"));
                }
                e.Current.Writer.Flush();
            }
        }
        private void mServerHandleRetrieveMediaServer(OnHandleConnectionEventArgs e)
        {
            using (var locker = e.Current.BeginWriterLocker())
            {
                SendServerList(e);
            }
        }
        private void mServerHandleRegistryMediaServerPort(OnHandleConnectionEventArgs e)
        {
            int port=0;
            using (var locker = e.Current.BeginReaderLocker())
            {
                port = e.Current.Reader.ReadInt32();
            }
            e.Current.Attributes.SetAttribute("Port",port);
            using (var locker = e.Current.BeginWriterLocker())
            {
                e.Current.Writer.Write((int)0);
                e.Current.Writer.Flush();
            }
            OnServerCountUpdated(e.Server);
        }
        void mServer_OnHandleConnection(object sender, OnHandleConnectionEventArgs e)
        {
            if (OnNewConnectionArrived != null)
            {
                OnNewConnectionArrived(this, e);
            }
            OutputMessage("New Connection From {0}",e.Current.IPAddress);
            while (true)
            {
                try
                {
                    int code = e.Current.Reader.ReadInt32();
                    switch (code)
                    {
                        case ServerFunctionCodes.Constants.REGISTRY_MEDIA_SERVER:
                            mServerHandleRegistryMediaServer(e);
                            break;
                        case ServerFunctionCodes.Constants.RETRIEVE_MEDIA_SERVER:
                            mServerHandleRetrieveMediaServer(e);
                            break;
                        case ServerFunctionCodes.Constants.REGISTRY_MEDIA_SERVER_PORT:
                            mServerHandleRegistryMediaServerPort(e);
                            break;
                        default:
                            Console.WriteLine("Unknown code {0} From {1}", code, e.Current.IPAddress);
                            break;
                    }
                    
                }
                catch (IOException ee)
                {
                    if(ee.InnerException != null && (ee.InnerException is SocketException))
                    {
                        
                    }
                    break;
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee.ToString());
                    break;
                }
            }
        }
    }
}
