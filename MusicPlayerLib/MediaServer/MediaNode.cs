using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;
using Utilities.Server;
namespace WavePlayer.MediaServer
{
    public class MediaNode:IDisposable
    {
        public volatile bool EnableSoundVisualization = true;
        Interfaces.IFileSystem mFileSystem;
        ServerHolder mServerHolder = new ServerHolder();
        IniReader mIniReader = null;
        int mPort = 9001;
        int mPID = -1;
        String mTrackServerAddress;
        int mTrackServerPort;
        public event EventHandler MediaServerListUpdated;
        public event EventHandler MediaNodeLoaded;
        public event EventHandler<byte[]> OnStreamBytePushed;
        volatile MediaConnectionInstance mTrackerServerConnection;
        volatile MediaConnectionInstance mTrackerServerSecondConnection;
        AsyncTask customHandlerDispatcher = new AsyncTask();
        Locker mLocker = new Locker();
        public DynamicAttributes Attributes = new DynamicAttributes();
        public Interfaces.IFileSystem FileSystem
        {
            get
            {
                using (var locker = mLocker.Lock())
                {
                    return mFileSystem;
                }
            }
            set
            {
                using (var locker = mLocker.Lock())
                {
                    mFileSystem = value;
                }
            }
        }
        //1. connect to tracker (active connection)
        //2. use second connection to tracker
        //3. second connection as tracker notification
        
        //4. when tracker send server list, store to ConnectionInformation
        //5. connect to other server list with 1 connection
        Dictionary<String, MediaConnectionInstance> mOtherServerConnections = new Dictionary<String, MediaConnectionInstance>();
        public Dictionary<String, String> Subscriber = new Dictionary<string, string>();
        List<String> mCachedServerList = null;
        List<String> mCachedSubscribeServerList = null;
        public List<String> ServerList
        {
            get
            {
                if (mCachedServerList == null)
                {
                    mCachedServerList = new List<string>(mOtherMediaServerList.Keys);
                }
                return mCachedServerList;
            }
        }

        public List<String> SubscribeServerList
        {
            get
            {
                using (var locker2 = mLocker.Lock())
                {
                    if (mCachedSubscribeServerList == null)
                    {
                        mCachedSubscribeServerList = new List<string>(Subscriber.Keys);
                    }
                    return mCachedSubscribeServerList;
                }
            }
        }
        

        public bool NodeHasSubscribeStream(String key)
        {
            using (var locker = mLocker.Lock())
            {
                return Subscriber.ContainsKey(key);
            }
        }
        public class ConnectionInformation
        {
            public String Address;
            public int Port;
            public int PID;
            public ConnectionInformation(String Address, int Port,int pid)
            {
                this.Address = Address;
                this.Port = Port;
                this.PID = pid;
            }
            public ConnectionInformation()
            {
                Address = "";
                Port = -1;
            }
        }
        private Dictionary<String, ConnectionInformation> mOtherMediaServerList = new Dictionary<String, ConnectionInformation>();
        AsyncTask mMeshConnectionThread = null;
        object mMeshConnectionLocker = new object();
        object mMeshConnectionStatusLocker = new object();
        public MediaNode()
        {
            mIniReader = IniReader.FromFile("MediaNode.ini");
            mPort = mIniReader.GetInt("Stream.Port");
            mTrackServerAddress = mIniReader.GetString("TrackServer.IP","127.0.0.1");
            mTrackServerPort = mIniReader.GetInt("TrackServer.Port",9000);
            EnableSoundVisualization = mIniReader.GetBoolean("MediaNode.EnableSoundVisualization", true);
            mServerHolder.OnHandleConnection += mServerHolder_OnHandleConnection;
            mServerHolder.OnServerRemoved += mServerHolder_OnServerRemoved;
        }

        void mServerHolder_OnServerRemoved(object sender, OnHandleConnectionEventArgs e)
        {
            lock (mMeshConnectionStatusLocker)
            {
                string address = e.Current.IPAddress;
                int pid = e.Current.Attributes.GetAttributeInt("PID");
                String key = address + ":" + pid.ToString();
                if (mOtherServerConnections.ContainsKey(key))
                {
                    MediaConnectionInstance instance = mOtherServerConnections[key];
                    instance.Dispose();
                    mOtherServerConnections.Remove(key);
                    using (var locker2 = mLocker.Lock())
                    {
                        if (Subscriber.ContainsKey(key))
                        {
                            mCachedSubscribeServerList = null;
                            Subscriber.Remove(key);
                        }
                    }
                }
            }
        }

        private int RegisterToTrackerHelper(MediaConnectionInstance trackerConnection,bool IsSecondConnection)
        {
            Console.WriteLine("Registry");
            using (var locker = trackerConnection.BeginWriterLocker())
            {
                trackerConnection.Writer.Write((int)ServerFunctionCodes.Constants.REGISTRY_MEDIA_SERVER);
                int PID = mPID;
                trackerConnection.Writer.Write((int)PID);
                trackerConnection.Writer.Write((bool)IsSecondConnection);
                trackerConnection.Writer.Flush();
            } 
            Console.WriteLine("Get Response");
            using (var locker = trackerConnection.BeginReaderLocker())
            {    
                return trackerConnection.Reader.ReadInt32();
            }
        }
        private void RegistryToTracker(MediaConnectionInstance trackerConnection)
        {
            int Response = 0;
            {
                Response = RegisterToTrackerHelper(trackerConnection, false);
            }
            {
                Console.WriteLine("Registry Port");
                using (var locker = trackerConnection.BeginWriterLocker())
                {
                    trackerConnection.Writer.Write((int)ServerFunctionCodes.Constants.REGISTRY_MEDIA_SERVER_PORT);
                    trackerConnection.Writer.Write((int)mPort);
                    trackerConnection.Writer.Flush();
                }
                Console.WriteLine("Get Response");
                using (var locker = trackerConnection.BeginReaderLocker())
                {
                    Response = trackerConnection.Reader.ReadInt32();
                }
            }
        }
        private void RegistrySecondToTracker(MediaConnectionInstance trackerConnection)
        {
            int Response = 0;
            {
                Response = RegisterToTrackerHelper(trackerConnection, true);
            }
        }
        private bool IsCurrentNode(ConnectionInformation info)
        {
            return "127.0.0.1".Equals(info.Address) && info.PID == mPID;
        }
        private void ReadServerList(MediaConnectionInstance trackerConnection)
        {
            int Count = 0;
            List<ConnectionInformation> infos = new List<ConnectionInformation>();
            using (var locker = trackerConnection.BeginReaderLocker())
            {
                Count = trackerConnection.Reader.ReadInt32();
                for (int i = 0; i < Count; ++i)
                {
                    String addr = trackerConnection.Reader.ReadString();
                    int port = trackerConnection.Reader.ReadInt32();
                    int pid = trackerConnection.Reader.ReadInt32();
                    ConnectionInformation info = new ConnectionInformation(addr, port, pid);
                    if (IsCurrentNode(info)) continue;
                    infos.Add(info);
                }
            }
            lock (mMeshConnectionLocker)
            {
                mOtherMediaServerList.Clear();
                foreach (ConnectionInformation info in infos)
                {
                    String key = info.Address + ":" + info.PID.ToString();
                    mOtherMediaServerList[key] = info;
                }
                if (mMeshConnectionThread == null)
                {
                    mMeshConnectionThread = new AsyncTask(MeshThreadStartConnect);
                    mMeshConnectionThread.SetName("MeshConnect");
                    mMeshConnectionThread.AddAfterFinishJob(() => {
                        mMeshConnectionThread = null;
                        if (MediaNodeLoaded != null)
                        {
                            MediaNodeLoaded(this, EventArgs.Empty);
                        }
                    });
                    mMeshConnectionThread.Start(false);
                }
            }
            // notify
            if (MediaServerListUpdated != null)
            {
                mCachedServerList = null;
                MediaServerListUpdated(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// connect to other media node if not connected yet
        /// </summary>
        private void MeshThreadStartConnect()
        {
            List<ConnectionInformation> infos = new List<ConnectionInformation>();
            lock (mMeshConnectionLocker)
            {
                infos.AddRange(mOtherMediaServerList.Values);
            }
            for (int i = 0; i < infos.Count; ++i)
            {
                ConnectionInformation info = infos[i];
                String key = info.Address + ":" + info.PID.ToString();
                lock (mMeshConnectionStatusLocker)
                {
                    if (!mOtherServerConnections.ContainsKey(key))
                    {
                        Socket connection = MediaNode.MakeConnection(info.Address, info.Port);
                        MediaConnectionInstance instance = new MediaConnectionInstance(connection);
                        int PID = mPID;
                        using (var locker=instance.BeginWriterLocker())
                        {
                            instance.Writer.Write((int)ServerFunctionCodes.Constants.MEDIA_NODE_REPORT_PID);
                            instance.Writer.Write((int)PID);
                            instance.Writer.Flush();
                        }
                        using (var locker = instance.BeginReaderLocker())
                        {
                            instance.Reader.ReadInt32();
                        }
                        mOtherServerConnections[key] = instance;
                    }
                }
            }
            for (int i = 0; i < infos.Count; ++i)
            {
                ConnectionInformation info = infos[i];
                String key = info.Address + ":" + info.PID.ToString();
                SendServerGetMusicListRequest(key);
            }
        }

        private void RetrieveServerList(MediaConnectionInstance trackerConnection)
        {
            using(var locker=trackerConnection.BeginWriterLocker())
            {
                trackerConnection.Writer.Write((int)ServerFunctionCodes.Constants.RETRIEVE_MEDIA_SERVER);
                trackerConnection.Writer.Flush();
                ReadServerList(trackerConnection);
            }
        }
        /// <summary>
        /// handle message from tracker server
        /// i.e. server list update
        /// </summary>
        /// <param name="trackerConnection"></param>
        private void HandleTrackerResponse(MediaConnectionInstance trackerConnection)
        {
            int Code = trackerConnection.Reader.ReadInt32();
            switch (Code)
            {
                case ServerFunctionCodes.Constants.TRACKER_NOTIFICATION_UPDATE_LIST:
                    ReadServerList(trackerConnection);
                    break;
            }
        }

        private static Socket MakeConnection(String address, int port)
        {
            Socket sckClient = null;
            try
            {
                sckClient = new Socket(SocketType.Stream, ProtocolType.Tcp);
                sckClient.Connect(address, port);
            }
            catch (SocketException ee)
            {

            }
            return sckClient;
        }
        private void NeotiateToTracker()
        {
            AsyncTask trackerNegotiataion = new AsyncTask(() =>
            {

                while (true)
                {
                    Socket sckClient = null;
                    Socket sckClientSecond = null;
                    try
                    {
                        sckClient = MakeConnection(mTrackServerAddress, mTrackServerPort);
                        
                        if (sckClient.Connected)
                        {
                            Console.WriteLine("Connected");
                            mTrackerServerConnection = new MediaConnectionInstance(sckClient);
                            RegistryToTracker(mTrackerServerConnection);
                            sckClientSecond = MakeConnection(mTrackServerAddress, mTrackServerPort); 
                            mTrackerServerSecondConnection = new MediaConnectionInstance(sckClientSecond);
                            RegistrySecondToTracker(mTrackerServerSecondConnection);
                            RetrieveServerList(mTrackerServerConnection);
                            // wait for tracker response
                            while (true)
                            {
                                HandleTrackerResponse(mTrackerServerSecondConnection);
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine(ee.ToString());
                    }
                    finally
                    {
                        // tracker closed
                        if (sckClient != null)
                        {
                            sckClient.Dispose();
                            sckClient = null;
                        }
                        if (sckClientSecond != null)
                        {
                            sckClientSecond.Dispose();
                            sckClientSecond = null;
                        }
                    }
                    Console.WriteLine("Retry Connect to Tracker");
                    Thread.Sleep(3000);
                }
            });
            trackerNegotiataion.SetName("TrackerClient");
            trackerNegotiataion.Start(false);
        }
        public void Start()
        {
            Process process = Process.GetCurrentProcess();
            mPID = process.Id;
            mServerHolder.StartAsync(mPort);
            NeotiateToTracker();
            
        }

        public static bool TryAccessFile(String filePath, Action<String> action)
        {
            DateTime startRetry = DateTime.Now;
            while (DateTime.Now.Subtract(startRetry).TotalSeconds < 10)
            {
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {

                    }
                    action(filePath);
                    return true;
                }
                catch (Exception ee)
                {

                }
            }
            return true;
        }
        public void SendServerStreamToSubscriber(String key, byte[] samples)
        {
            if (!NodeHasSubscribeStream(key)) return;
            int req = ServerFunctionCodes.Constants.MEDIA_NODE_REALTIME_WAVE_STREAM_PUSH;
            MediaConnectionInstance current = mOtherServerConnections[key];
            using (var locker = current.BeginWriterLocker())
            {
                current.Writer.Write((int)req);
                current.Writer.Write((int)samples.Length);
                current.Writer.Write(samples);
                current.Writer.Flush();
            }
            using (var locker = current.BeginReaderLocker())
            {
                current.Reader.ReadInt32();
            }
        }
        public void SendServerSubscribeStreamRequest(String key,bool bSubscribe)
        {
            if (!mOtherServerConnections.ContainsKey(key)) return;
            int req = ServerFunctionCodes.Constants.MEDIA_NODE_SUBSCRIBE_STREAM_REQUEST;
            MediaConnectionInstance current = mOtherServerConnections[key];
            using (var locker = current.BeginWriterLocker())
            {
                current.Writer.Write((int)req);
                current.Writer.Write(bSubscribe);
                current.Writer.Flush();
            }
            using (var locker = current.BeginReaderLocker())
            {
                current.Reader.ReadInt32();
            }
        }
        public int[] SendServerGetStreamWaveFormatRequest(String key)
        {
            if (!mOtherServerConnections.ContainsKey(key)) return new int[3];
            int req = ServerFunctionCodes.Constants.MEDIA_NODE_GET_STREAM_WAVE_FORMAT_REQUEST;
            MediaConnectionInstance current = mOtherServerConnections[key];
            using (var locker = current.BeginWriterLocker())
            {
                current.Writer.Write((int)req);
                current.Writer.Flush();
            }
            using (var locker = current.BeginReaderLocker())
            {
                int[] ret = new int[3];
                
                int Channels = current.Reader.ReadInt32();
                int SampleRate = current.Reader.ReadInt32();
                int BitsPerSample = current.Reader.ReadInt32();
                ret = new int[3]{Channels,SampleRate,BitsPerSample};
                current.Attributes.SetAttribute("MEDIA_NODE_GET_STREAM_WAVE_FORMAT_REQUEST", ret);
                return ret;
            }
        }
        public void PerformP2PParallelDownload(WavPlayer.Management.MusicListItem item,String dir,EventHandler<Tuple<WavPlayer.Management.MusicListItem,String>> OnFinish=null)
        {
            String[] servers = item.RemoteLocation.Split(';');
            List<String> filenameParts = new List<string>();
            List<String> availableServer = new List<string>();
            
            for (int i = 0; i < servers.Length; ++i)
            {
                String key = servers[i];
                if (mOtherServerConnections.ContainsKey(key))
                    availableServer.Add(key);
            }
            for (int i = 0; i < availableServer.Count; ++i)
            {
                filenameParts.Add(Path.Combine(dir,Path.GetFileName(item.FileFullPath) + ".p" + i));
            }
            AsyncTask task = new AsyncTask(() =>
            {
                Locker sharedLocker = new Locker();
                int[] fileSizes = new int[availableServer.Count];
                int minSize = -1;
                // get minimal size
                Parallelx.For(availableServer.Count, (i) =>
                {
                    String key = availableServer[i];
                    String queryFileName = Path.GetFileName(item.FileName);
                    MediaConnectionInstance current = mOtherServerConnections[key];
                    using (var locker = current.BeginWriterLocker())
                    {
                        current.Writer.Write((int)ServerFunctionCodes.Constants.MEDIA_NODE_QUERY_FILE_SIZE);
                        current.Writer.Write(queryFileName);
                        current.Writer.Flush();
                    }
                    using (var locker = current.BeginReaderLocker())
                    {
                        fileSizes[i] = current.Reader.ReadInt32();
                    }
                }, true);
                minSize = fileSizes[0];
                for (int i = 0; i < fileSizes.Length; ++i)
                {
                    minSize = Math.Min(minSize, fileSizes[i]);
                }
                int partSize = minSize / availableServer.Count;

                // fetch file
                Parallelx.For(availableServer.Count, (i) =>
                {
                    String key = availableServer[i];
                    String queryFileName = Path.GetFileName(item.FileName);
                    String targetFileName = Path.Combine(dir, filenameParts[i]);
                    MediaConnectionInstance current = mOtherServerConnections[key];
                    int begin = partSize * i;
                    int length = partSize;
                    if (i == availableServer.Count - 1)
                    {
                        int remain = (minSize % availableServer.Count);
                        if (remain > 0)
                        {
                            length += remain;
                        }
                    }
                    using (var locker = current.BeginWriterLocker())
                    {
                        current.Writer.Write((int)ServerFunctionCodes.Constants.MEDIA_NODE_PERFORM_GET_FILE);
                        current.Writer.Write(queryFileName);
                        current.Writer.Write((int)partSize * i);
                        current.Writer.Write((int)length);
                        current.Writer.Flush();
                    }
                    using (var locker = current.BeginReaderLocker())
                    {
                        byte[] bytes = current.Reader.ReadBytes(length);
                        File.WriteAllBytes(targetFileName, bytes);
                    }
                }, true);
                String outputTargetFileName = Path.Combine(dir, Path.GetFileName(item.FileName));
                using (FileStream outputFile = new FileStream(outputTargetFileName, FileMode.Create, FileAccess.Write))
                {
                    for (int i = 0; i < filenameParts.Count; ++i)
                    {
                        byte[] bytes = File.ReadAllBytes(filenameParts[i]);
                        outputFile.Write(bytes, 0, bytes.Length);
                        File.Delete(filenameParts[i]);
                    }
                }
                if (OnFinish != null)
                {
                    OnFinish(this,new Tuple<WavPlayer.Management.MusicListItem,String>(item,outputTargetFileName));
                }
            });
            task.Start(false);
        }
        public String SendServerGetMusicListRequest(String key)
        {
            if (!mOtherServerConnections.ContainsKey(key)) return "";
            int req = ServerFunctionCodes.Constants.MEDIA_NODE_GET_MUSICLIST_REQUEST;
            MediaConnectionInstance current = mOtherServerConnections[key];
            using (var locker = current.BeginWriterLocker())
            {
                current.Writer.Write((int)req);
                current.Writer.Flush();
            }
            using (var locker = current.BeginReaderLocker())
            {
                String json = current.Reader.ReadString();
                current.Attributes.SetAttribute("MEDIA_NODE_GET_MUSICLIST_ACK", json);
                return json;
            }
        }
        
        private void HandleOtherMediaServerRequest(int Code,OnHandleConnectionEventArgs e)
        {
            // do something
            switch (Code)
            {
                case ServerFunctionCodes.Constants.MEDIA_NODE_REPORT_PID:
                    {
                        using (var locker = e.Current.BeginReaderLocker())
                        {
                            int PID=e.Current.Reader.ReadInt32();
                            e.Current.Attributes.SetAttribute("PID", PID);
                        }
                        //ack
                        using (var locker = e.Current.BeginWriterLocker())
                        {
                            e.Current.Writer.Write((int)0);
                            e.Current.Writer.Flush();
                        }
                    }
                    break;
                case ServerFunctionCodes.Constants.MEDIA_NODE_GET_MUSICLIST_REQUEST:
                    {
                        String dataJson=Path.Combine(Application.StartupPath,"data.json");
                        if (File.Exists(dataJson))
                        {
                            if (MediaNode.TryAccessFile(dataJson, (path) => {

                            }))
                            {
                                String text = File.ReadAllText(dataJson);
                                using (var locker = e.Current.BeginWriterLocker())
                                {
                                    e.Current.Writer.Write(text);
                                    e.Current.Writer.Flush();
                                }
                            }
                        }
                        else
                        {
                            using (var locker = e.Current.BeginWriterLocker())
                            {
                                e.Current.Writer.Write("");
                                e.Current.Writer.Flush();
                            }
                        }
                    }
                    break;
                case ServerFunctionCodes.Constants.MEDIA_NODE_GET_STREAM_WAVE_FORMAT_REQUEST:
                    {
                        int channel = Attributes.GetAttributeInt("Current.Channels");
                        int sampleRate = Attributes.GetAttributeInt("Current.SampleRate");
                        int bitsPerSample = Attributes.GetAttributeInt("Current.BitsPerSample");
                        using (var locker = e.Current.BeginWriterLocker())
                        {
                            e.Current.Writer.Write((int)channel);
                            e.Current.Writer.Write((int)sampleRate);
                            e.Current.Writer.Write((int)bitsPerSample);
                            e.Current.Writer.Flush();
                        }
                    }
                    break;
                case ServerFunctionCodes.Constants.MEDIA_NODE_SUBSCRIBE_STREAM_REQUEST:
                    {
                        using (var locker = e.Current.BeginReaderLocker())
                        {
                            bool subscribed = e.Current.Reader.ReadBoolean();
                            e.Current.Attributes.SetAttribute("SubscribeStream", subscribed);
                            using (var locker2 = mLocker.Lock())
                            {
                                string address = e.Current.IPAddress;
                                int pid = e.Current.Attributes.GetAttributeInt("PID");
                                String key = address + ":" + pid.ToString();
                                if (subscribed)
                                {
                                    if (!Subscriber.ContainsKey(key))
                                    {
                                        mCachedSubscribeServerList = null;
                                    }
                                    Subscriber[key] = key;
                                }
                                else
                                {
                                    mCachedSubscribeServerList = null;
                                    Subscriber.Remove(key);
                                }
                            }
                        }
                        
                        //ack
                        using (var locker = e.Current.BeginWriterLocker())
                        {
                            e.Current.Writer.Write((int)0);
                            e.Current.Writer.Flush();
                        }
                        // TODO when stream is ok
                        // 1. copy a stream to buffer
                        // 2. push stream with ServerFunctionCodes.Constants.MEDIA_NODE_REALTIME_WAVE_STREAM_PUSH
                    }
                    break;
                case ServerFunctionCodes.Constants.MEDIA_NODE_REALTIME_WAVE_STREAM_PUSH:
                    {
                        using (var locker = e.Current.BeginReaderLocker())
                        {
                            int length = e.Current.Reader.ReadInt32();
                            byte[] bytes=e.Current.Reader.ReadBytes(length);
                            // push byte to sound buffer
                            if (OnStreamBytePushed != null)
                            {
                                OnStreamBytePushed(this, bytes);
                            }
                        }
                        //ack
                        using (var locker = e.Current.BeginWriterLocker())
                        {
                            e.Current.Writer.Write((int)0);
                            e.Current.Writer.Flush();
                        }
                    }
                    break;
                case ServerFunctionCodes.Constants.MEDIA_NODE_QUERY_FILE_SIZE:
                    {
                        String filename = "";
                        using (var locker = e.Current.BeginReaderLocker())
                        {
                            filename = e.Current.Reader.ReadString();
                        }
                        int fileSize = FileSystem.GetFileSize(filename);
                        //ack
                        using (var locker = e.Current.BeginWriterLocker())
                        {
                            e.Current.Writer.Write((int)fileSize);
                            e.Current.Writer.Flush();
                        }
                    }
                    break;
                case ServerFunctionCodes.Constants.MEDIA_NODE_PERFORM_GET_FILE:
                    {
                        String filename = "";
                        int offset = 0;
                        int length = 0;
                        using (var locker = e.Current.BeginReaderLocker())
                        {
                            filename = e.Current.Reader.ReadString();
                            offset = e.Current.Reader.ReadInt32();
                            length = e.Current.Reader.ReadInt32();
                        }
                        String fullName = FileSystem.GetFullFilePath(filename);
                        //ack
                        using (var locker = e.Current.BeginWriterLocker())
                        {
                            byte[] data = new byte[length];
                            using (FileStream fs = new FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                fs.Position = offset;
                                fs.Read(data, 0, length);
                            }
                            e.Current.Writer.Write(data);
                            e.Current.Writer.Flush();
                        }
                    }
                    break;
            }            
        }
        public object GetServerAttribute(String server, String key)
        {
            if (mServerHolder == null) return null;
            if (!mOtherServerConnections.ContainsKey(server)) return null;
            MediaConnectionInstance instance = mOtherServerConnections[server];
            return instance.Attributes.GetAttribute(key);
        }
        void mServerHolder_OnHandleConnection(object sender, OnHandleConnectionEventArgs e)
        {
            // other media server
            try
            {

                while (true)
                {
                    int Code = -1;
                    using (var locker = e.Current.BeginReaderLocker())
                    {
                        Code = e.Current.Reader.ReadInt32();
                    }
                    HandleOtherMediaServerRequest(Code, e);
                }
            }
            catch (IOException ee)
            {
                if (ee.InnerException != null && (ee.InnerException is SocketException))
                {
                    //
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
        }


        public void Dispose()
        {
            mIniReader = null;
            if (mServerHolder != null)
            {
                mServerHolder.Dispose();
                mServerHolder = null;
            }
        }
    }
}
