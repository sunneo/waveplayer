using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.Interfaces
{
    public interface ICanvas
    {
        Bitmap BaseImage { get; }
        Size CanvasSize { get; }
        void OnBitmapReady(Bitmap bmp);
    }
}
