using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.WavFormat.Format
{
    public class WaveFile:IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct InformationHeader
        {
            public Int32 ChunkID;
            public Int32 FileSize;
            public Int32 RiffType;
            public Int32 FmtID;
            public Int32 FmtSize;
            public Int16 FmtCode;
            public Int16 FmtChannels;
            public Int32 FmtSampleRate;
            public Int32 FmtAvgBPS;
            public Int16 FmtBlockAlign;
            public Int16 FmtBitDepth;
            public Int16 fmtExtraSize;
        }
        public int SampleRate { get; private set; }
        public int BitDepth { get; private set; }
        public int Channels { get; private set; }
        public int DataSize { get; private set; }
        private byte[] mData = null;
        private long Offset;
        public int Length
        {
            get
            {
                return DataSize;
            }
        }
        public byte[] Data
        {
            get
            {
                if (mData == null)
                {
                    if (DataStream != null)
                    {
                        DataStream.BaseStream.Position = this.Offset;
                        mData = DataStream.ReadBytes(DataSize);
                    }
                }
                return mData;
            }
        }
        public BinaryReader DataStream
        {
            get;
            private set;
        }
        public InformationHeader Information
        {
            get;
            private set;
        }
        public long Position
        {
            get
            {
                return DataStream.BaseStream.Position - Offset;
            }
            set
            {
                long newVal = Offset + value;
                if (newVal < 0)
                {
                    DataStream.BaseStream.Position = 0;
                }
                else
                {
                    if (newVal <= Length)
                    {
                        DataStream.BaseStream.Position = Offset + value;
                    }
                    else
                    {
                        DataStream.BaseStream.Position = Length;
                    }
                }
            }
        }
        private WaveFile() { }
        public static WaveFile Empty 
        {
            get
            {
                return new WaveFile();
            }
        }
        public static WaveFile Parse(System.IO.Stream waveFileStream)
        {
            BinaryReader reader = new BinaryReader(waveFileStream);
            InformationHeader info = new InformationHeader();
            info.ChunkID = reader.ReadInt32();
            info.FileSize = reader.ReadInt32();
            info.RiffType = reader.ReadInt32();
            info.FmtID = reader.ReadInt32();
            info.FmtSize = reader.ReadInt32();
            info.FmtCode = reader.ReadInt16();
            info.FmtChannels = reader.ReadInt16();
            info.FmtSampleRate = reader.ReadInt32();
            info.FmtAvgBPS = reader.ReadInt32();
            info.FmtBlockAlign = reader.ReadInt16();
            info.FmtBitDepth = reader.ReadInt16();

            if (info.FmtSize == 18)
            {
                // Read any extra values
                int fmtExtraSize = reader.ReadInt16();
                reader.ReadBytes(fmtExtraSize);
            }

            int dataID = reader.ReadInt32();
            int dataSize = reader.ReadInt32();
            long offset = reader.BaseStream.Position;
            //byte[] dataBytes = reader.ReadBytes(dataSize);

            WaveFile parsedFile = new WaveFile()
            {
                Information= info,
                SampleRate = info.FmtSampleRate,
                BitDepth = info.FmtBitDepth,
                Channels = info.FmtChannels,
              //  Data = dataBytes
            };
            parsedFile.DataSize = dataSize;
            parsedFile.Offset = offset;
            parsedFile.DataStream = reader;
            return parsedFile;
        }

        public void Dispose()
        {
            if (mData != null)
            {
                mData = null;
            }
            if (DataStream != null)
            {
                DataStream.Close();
                DataStream = null;
            }
        }
    }
}
