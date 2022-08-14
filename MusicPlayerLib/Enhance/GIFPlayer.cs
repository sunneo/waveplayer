using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;
using WavePlayer.Interfaces;

namespace WavePlayer.Enhance
{
    public class GIFPlayer : IPlayer, IDisposable
    {
        Locked<ICanvas> Canvas;
        Locked<int> Index = 0;
        Image GIF;
        FrameDimension dimension;
        Timer timer = new Timer() { Interval = 16 };
        public event EventHandler<double> ProgressUpdated;

        public event EventHandler Finished;
        List<IEffectOperator> mEffects = new List<IEffectOperator>();

        public GIFPlayer(Control owner)
        {
            timer.Tick += timer_Tick;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (GIF == null || dimension == null || Canvas == null) return;
                GIF.SelectActiveFrame(dimension, Index.Value);
                ICanvas canvas = (ICanvas)Canvas.Value;
                Bitmap bmp = new Bitmap(canvas.CanvasSize.Width, canvas.CanvasSize.Height);
                {
                     using (Graphics graphics = Graphics.FromImage(bmp))
                     {
                         graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                         graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                         graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                         graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                         graphics.DrawImage(GIF,new Rectangle(0,0,bmp.Width,bmp.Height));
                         PropertyItem item = GIF.GetPropertyItem(0x5100);
                         int delay = (item.Value[0] + item.Value[1] * 256) * 10;
                         if (delay == 0) delay = 15;
                         timer.Interval = delay;
                         Canvas.Value.OnBitmapReady(bmp);
                     }
                    
                }
                if (ProgressUpdated != null)
                {
                    ProgressUpdated(this, Index.Value);
                }
                if (Index.Value+1 < Duration)
                {
                    ++Index;
                }
                else
                {
                    timer.Stop();
                    Index = 0;
                    if (Finished != null)
                    {
                        Finished(this, EventArgs.Empty);
                    }
                    
                }
            }
            catch (Exception ee)
            {

            }
        }
        public IList<IEffectOperator> Effects
        {
            get { return mEffects; }
        }

        public int Channels
        {
            get { return 0; }
        }

        public int SampleRate
        {
            get { return 0; }
        }

        public int BitsPerSample
        {
            get { return 0; }
        }

        public string FileName
        {
            get;
            set;
        }

        public double Duration
        {
            get 
            {
                if (GIF != null && GIF.FrameDimensionsList.Length > 0)
                {
                  
                    return GIF.GetFrameCount(dimension);
                }
                return 0;
            }
            set { }
        }

        public bool SetDataSource(string name)
        {
            this.FileName = name;
            if (this.GIF != null)
            {
                this.GIF.Dispose();
                this.GIF = null;
            }
            try
            {
                this.GIF = (Image)Image.FromFile(name);
                if (this.GIF.FrameDimensionsList.Length > 0)
                {
                    this.dimension = new FrameDimension(GIF.FrameDimensionsList[0]);
                }
                return true;
            }
            catch (Exception ee)
            {
                return false;
            }
        }

        public long Position
        {
            get
            {
                return Index.Value;
            }
            set
            {
                Index.Value = (int)value;
                GIF.SelectActiveFrame(this.dimension, Index);
            }
        }

        public void Stop()
        {
            timer.Stop();
        }

        public bool Play()
        {
            timer.Start();
            return timer.Enabled;
        }

        public bool Playing
        {
            get { return timer.Enabled; }
        }

        public void Pause()
        {
            timer.Stop();
        }

        public void Close()
        {
            timer.Stop();
        }

        public void Dispose()
        {
            Close();
        }
        public bool HasVideo
        {
            get { return true; }
        }

        public void AttachCanvas(Interfaces.ICanvas canvas)
        {
            this.Canvas = new Locked<ICanvas>(canvas);
        }
    }
}
