using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WavePlayer.UI
{
    public class DoubleBufferedButton:Button
    {
        volatile bool mIsDown=false;
        public bool IsDown
        {
            get
            {
                return mIsDown;
            }
        }
        public DoubleBufferedButton()
        {
            DoubleBuffered = true;
        }
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            mIsDown = true;
            base.OnMouseDown(mevent);
        }
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            mIsDown = false;
            base.OnMouseUp(mevent);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            mIsDown = false;
            base.OnMouseClick(e);
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            mIsDown = false;
            base.OnMouseDoubleClick(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            mIsDown = false;
            base.OnMouseLeave(e);
        }
    }
}
