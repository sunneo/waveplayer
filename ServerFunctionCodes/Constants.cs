using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFunctionCodes
{
    public class Constants
    {
        public const int REGISTRY_MEDIA_SERVER = 1;
        
        public const int RETRIEVE_MEDIA_SERVER = 2;

        public const int REGISTRY_MEDIA_SERVER_PORT = 3;

        public const int TRACKER_NOTIFICATION_UPDATE_LIST = 4;

        public const int MEDIA_NODE_REPORT_PID = 100;
        public const int MEDIA_NODE_GET_MUSICLIST_REQUEST = 101;
        public const int MEDIA_NODE_GET_STREAM_WAVE_FORMAT_REQUEST = 102;
        public const int MEDIA_NODE_SUBSCRIBE_STREAM_REQUEST = 103;
        public const int MEDIA_NODE_REALTIME_WAVE_STREAM_PUSH = 104;
        public const int MEDIA_NODE_QUERY_FILE_SIZE = 105;
        public const int MEDIA_NODE_PERFORM_GET_FILE = 106;

        public static Dictionary<String, object> StaticAttributes = new Dictionary<string, object>();

        public static int GetStaticAttributeInt(String key)
        {
            if (StaticAttributes.ContainsKey(key))
            {
                return (int)StaticAttributes[key];
            }
            return 0;
        }
        public static String GetStaticAttributeString(String key)
        {
            if (StaticAttributes.ContainsKey(key))
            {
                return (String)StaticAttributes[key];
            }
            return "";
        }
    }
}
