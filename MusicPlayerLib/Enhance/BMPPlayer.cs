using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;
using WavePlayer.Interfaces;

namespace WavePlayer.Enhance
{
    public class BMPPlayer:IPlayer,IDisposable
    {
        Locked<ICanvas> Canvas;
        Bitmap BMP;
         
        Timer timer = new Timer() { Interval = 16 };
        public event EventHandler<double> ProgressUpdated;

        public event EventHandler Finished;
        List<IEffectOperator> mEffects = new List<IEffectOperator>();

        public BMPPlayer(Control owner)
        {
            timer.Tick += timer_Tick;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Canvas != null)
                {
                    ICanvas canvas = (ICanvas)Canvas.Value;
                    Canvas.Value.OnBitmapReady(new Bitmap(this.BMP,canvas.CanvasSize));
                }
            }
            catch (Exception ee)
            {

            }
        }
        public IList<IEffectOperator> Effects
        {
            get { return mEffects;  }
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
            get;
            set;
        }

        public bool SetDataSource(string name)
        {
            if (this.BMP != null)
            {
                this.BMP.Dispose();
                this.BMP = null;
            }
            try
            {
                this.BMP = (Bitmap)Bitmap.FromFile(name);
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
                return 0;
            }
            set
            {
                
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
