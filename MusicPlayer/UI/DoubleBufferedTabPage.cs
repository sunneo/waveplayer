using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WavePlayer.UI
{
    public class DoubleBufferedTabPage:TabPage
    {
        public DoubleBufferedTabPage()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer,true);
        }
    }
}
