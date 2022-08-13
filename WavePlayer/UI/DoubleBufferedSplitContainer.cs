using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WavePlayer.UI
{
    public class DoubleBufferedSplitContainer:SplitContainer
    {
        public DoubleBufferedSplitContainer()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
