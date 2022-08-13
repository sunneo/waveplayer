using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using WavePlayer.Interfaces;


namespace WavePlayer.Enhance
{
    public class SoundVisualizationEffect:IEffectOperator
    {
        public volatile bool Enabled = true;
        ICanvas Canvas;
        AsyncTask task = new AsyncTask();
        volatile bool taskHandling = false;
        Locker mLocker = new Locker();
        LinkedList<byte[]> Buffers = new LinkedList<byte[]>();
        static double Decibels(Complex c)
        {
            return ((c.Real == 0 && c.Imaginary == 0) ? (0) :
       10.0 * Math.Log10(c.Magnitude));
        }
         /* Performs a Bit Reversal Algorithm on a postive integer 
         * for given number of bits
         * e.g. 011 with 3 bits is reversed to 110 */
        public static int BitReverse(int n, int bits) {
           int reversedN = n;
           int count = bits - 1;
 
           n >>= 1;
           while (n > 0) {
                reversedN = (reversedN << 1) | (n & 1);
                count--;
                n >>= 1;
            }
 
            return ((reversedN << count) & ((1 << bits) - 1));
        }
 
        /* Uses Cooley-Tukey iterative in-place algorithm with radix-2 DIT case
         * assumes no of points provided are a power of 2 */
        public static void FFT(Complex[] buffer)
        {

            int bits = (int)Math.Log(buffer.Length, 2);
            for (int j = 1; j < buffer.Length / 2; j++)
            {

                int swapPos = BitReverse(j, bits);
                var temp = buffer[j];
                buffer[j] = buffer[swapPos];
                buffer[swapPos] = temp;
            }

            for (int N = 2; N <= buffer.Length; N <<= 1)
            {
                for (int i = 0; i < buffer.Length; i += N)
                {
                    for (int k = 0; k < N / 2; k++)
                    {

                        int evenIndex = i + k;
                        int oddIndex = i + k + (N / 2);
                        var even = buffer[evenIndex];
                        var odd = buffer[oddIndex];

                        double term = -2 * Math.PI * k / (double)N;
                        Complex exp = new Complex(Math.Cos(term), Math.Sin(term)) * odd;

                        buffer[evenIndex] = even + exp;
                        buffer[oddIndex] = even - exp;

                    }
                }
            }
        }
    
    
        
 
        public SoundVisualizationEffect(ICanvas canvas)
        {
            this.Canvas = canvas;
        }
        private Bitmap GenerateBMP(Complex[] data,double maxVal=1)
        {
            Size sz = Canvas.CanvasSize;
            
            Bitmap bmp = new Bitmap(sz.Width, sz.Height);
            bool hasBaseImage = false;
            if (Canvas.BaseImage != null)
            {
                hasBaseImage = true;
            }
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                if (!hasBaseImage)
                {
                    graphics.Clear(Color.Black);
                }
                else
                {
                    graphics.DrawImageUnscaledAndClipped(Canvas.BaseImage, new Rectangle(0,0,sz.Width,sz.Height));
                }
                int partWidth = 5;
                int parts = sz.Width / partWidth; // 1 part 10 pixel wide
                // 300 pixel -> 30 parts
                // 1 part = X len data = data.length/part
                int dataPerPart = data.Length / parts;
                double[] vals = new double[parts];
                Rectangle[] rects = new Rectangle[parts];
                DateTime dtStart = DateTime.Now;
                // data size is not big enought to parallelize
                for(int i=0; i<vals.Length; ++i){
                    int start = i * dataPerPart;
                    int end = start + dataPerPart;
                    if (end > data.Length)
                    {
                        end = data.Length;
                    }
                    double val = 0;
                    for (int j = start; j < end; ++j)
                    {
                        val += Decibels(data[j]) / maxVal;
                    }
                    val /= dataPerPart;
                    vals[i] = val;
                    {
                        int xStart = i * partWidth;
                        int xEnd = xStart + partWidth;
                        int yStart = sz.Height - (int)(val * sz.Height);
                        int yEnd = sz.Height;
                        rects[i] = new Rectangle(xStart, yStart, partWidth, (int)yEnd - yStart);
                    }
                }
                //DateTime dtEnd = DateTime.Now;
                //Console.WriteLine("Takes {0} Millis", dtEnd.Subtract(dtStart).TotalMilliseconds);
                Brush brush = Brushes.White;
                if (hasBaseImage)
                {
                    brush = new SolidBrush(Color.FromArgb(64, Color.White));
                }
                for (int i = 0; i < parts; ++i)
                {
                    graphics.FillRectangle(brush, rects[i]);
                }
                if (hasBaseImage)
                {
                    brush.Dispose();
                }

            }
            return bmp;
        }
        private void HandleNewBuffer()
        {
            try
            {
                while (taskHandling)
                {
                    if (Buffers.Count == 0)
                    {
                        taskHandling = false;
                        return;
                    }
                    byte[] data = null;
                    using (var locker = mLocker.Lock())
                    {
                        data = Buffers.First.Value;
                        Buffers.RemoveFirst();
                    }
                    if (data.Length == 0)
                    {
                        continue;
                    }
                    BinaryReader bReader = new BinaryReader(new MemoryStream(data));
                    int shortLen = data.Length / 2;
                    int shortLenRoundPower2 = (int)Math.Pow(2, (int)(Math.Ceiling(Math.Log((double)shortLen, 2))));
                    int minsize = shortLen;
                    int maxsize = shortLenRoundPower2;
                    if (shortLenRoundPower2 < shortLen)
                    {
                        minsize = shortLenRoundPower2;
                        maxsize = shortLen;
                    }
                    Complex[] doubles = new Complex[maxsize];
                    for (int i = 0; i < minsize; ++i)
                    {
                        doubles[i] = ((double)bReader.ReadInt16()) / 65536.0;
                    }
                    try
                    {
                        FFT(doubles);
                        double maxval = 0;
                        for (int i = 0; i < minsize; ++i)
                        {
                            double val = Math.Abs(Decibels(doubles[i]));
                            if (val > maxval) maxval = val;

                        }
                        Bitmap bmp = GenerateBMP(doubles, maxval);
                        Canvas.OnBitmapReady(bmp);
                    }
                    catch (Exception ee)
                    {

                    }
                }
            }
            catch (Exception ee)
            {
                taskHandling = false;
            }
        }
        public bool Handle(EffectEventArgs Args)
        {
            if (!Enabled)
            {
                if (Buffers.Count > 0)
                {
                    Buffers.Clear();
                }
                return false;
            }
            if (Args.NewSoundByte.Length > 0)
            {
                using (var locker = mLocker.Lock())
                {
                    Buffers.AddLast(Args.NewSoundByte);
                }
                if (!taskHandling)
                {
                    task.AddAfterFinishJob(HandleNewBuffer);
                    task.FlushJob(false);
                    taskHandling = true;
                }
                return true;
            }
            return false;
        }
    }
}
