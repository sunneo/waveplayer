using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WavePlayer.UI
{
    public partial class TrackBar : UserControl
    {
        volatile bool mShowThumb = true;
        [Browsable(true)]
        public bool ShowThumb
        {
            get
            {
                return mShowThumb;
            }
            set
            {
                mShowThumb = value;
                this.Invalidate();
            }
        }

        private int mMax = 100;
        private int mMin = 0;
        private int mValue = 0;
        public event EventHandler MaxChanged;
        public event EventHandler MinChanged;
        public event EventHandler ValueChanged;
        private Color mTrackBarBackground = Color.Gray;
        private Color mTrackBarForeColor = Color.Cyan;
        [Browsable(true)]
        public Color TrackBarForeColor
        {
            get
            {
                return mTrackBarForeColor;
            }
            set
            {
                mTrackBarForeColor = value;
                this.Invalidate();
            }
        }
        [Browsable(true)]
        public Color TrackBarBackground
        {
            get
            {
                return mTrackBarBackground;
            }
            set
            {
                mTrackBarBackground = value;
                this.Invalidate();
            }
        }
        public int Maximum
        {
            get
            {
                return mMax;
            }
            set
            {
                mMax = value;
                this.Invalidate();
                if (MaxChanged != null)
                {
                    MaxChanged(this, EventArgs.Empty);
                }
            }
        }
        public int Min
        {
            get
            {
                return mMin;
            }
            set
            {
                mMin = value;
                this.Invalidate();
                if (MinChanged != null)
                {
                    MinChanged(this, EventArgs.Empty);
                }
            }
        }
        public int Value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
                if (mValue < mMin) mValue = mMin;
                if (mValue > mMax) mValue = mMax;
                this.Invalidate();
                if (ValueChanged != null)
                {
                    ValueChanged(this, EventArgs.Empty);
                }
            }
        }

        public TrackBar()
        {
            InitializeComponent();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(this.TrackBarBackground))
                e.Graphics.FillRectangle(brush, this.DisplayRectangle);
            int len = mMax - mMin;
            
            double ratio = ((double)mValue) / Math.Max(1,len);
            int xPos = (int)(ratio * this.DisplayRectangle.Width);
            if (mMax < 0)
            {
                using (SolidBrush brush = new SolidBrush(this.TrackBarForeColor))
                {
                    e.Graphics.FillRectangle(brush, new Rectangle(this.DisplayRectangle.Left, this.DisplayRectangle.Top, Math.Max(0,this.DisplayRectangle.Width), Math.Max(0, this.DisplayRectangle.Height)));
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(this.TrackBarForeColor))
                {
                    
                    if (ShowThumb)
                    {
                        e.Graphics.FillRectangle(brush, new Rectangle(this.DisplayRectangle.Left, this.DisplayRectangle.Top + this.Height / 4, Math.Max(0, xPos + this.Height / 2), Math.Max(0, this.DisplayRectangle.Height / 4 * 3)));
                        e.Graphics.FillEllipse(brush, new Rectangle(Math.Max(0, xPos - this.Height / 2), Math.Max(0, (int)(this.DisplayRectangle.Top - 1)), this.Height, this.DisplayRectangle.Height + 2));
                    }
                    else
                    {
                        e.Graphics.FillRectangle(brush, new Rectangle(this.DisplayRectangle.Left, this.DisplayRectangle.Top, Math.Max(0, xPos + this.Height / 2), Math.Max(0, this.DisplayRectangle.Height)));
                    }
                }
            }
            base.OnPaint(e);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
    }
}
