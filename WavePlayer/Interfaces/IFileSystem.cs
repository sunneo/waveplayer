using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.Interfaces
{
    public interface IFileSystem
    {
        String GetFullFilePath(String filenameKey);
        int GetFileSize(String filenameKey);
    }
}
