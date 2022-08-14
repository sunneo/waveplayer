using SoundTouch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.Enhance
{
    public class SoundTouchEffect:Interfaces.IEffectOperator
    {
        Interfaces.IPlayer Player;
        public SoundTouchEffect(Interfaces.IPlayer player)
        {
            this.Player = player;
        }
        public void ResetSoundTouch()
        {
            sndTouchRep = new SoundTouch<short, long>();
            sndTouchRep.SetRate(1.0f);
            sndTouchRep.SetSampleRate(Player.SampleRate);
            sndTouchRep.SetChannels(Player.Channels);
            //sndTouchRep.SetPitch(1.25f);
            //sndTouchRep.SetTempoChange(0);
            sndTouchRep.SetPitchSemiTones(0);
            sndTouchRep.SetTempo(1);
            //sndTouchRep.SetRateChange(200);
            sndTouchRep.SetSetting(SettingId.UseAntiAliasFilter, 1);
            sndTouchRep.SetSetting(SettingId.AntiAliasFilterLength, 8);
        }
        SoundTouch<short, long> sndTouchRep;
        SoundTouch<short, long> sndTouch
        {
            get
            {
                if (sndTouchRep != null)
                {
                    return sndTouchRep;
                }
                ResetSoundTouch();
                return sndTouchRep;
            }
        }
        double playRate = 1;
        double tempo = 1;

        public void SetPitchSemiTones(int t)
        {
            try
            {
                if (sndTouch != null)
                {
                    if (this.playRate * this.tempo < 1)
                    {
                        // sndTouch.Flush();
                    }
                    sndTouch.SetPitchSemiTones(t);
                }
            }
            catch (Exception ee)
            {

            }
            
        }

        public void SetTempo(double tempo)
        {
            try
            {
                this.tempo = Math.Round(tempo, 2);
                if (sndTouch != null)
                {
                    if (this.playRate * this.tempo < 1)
                    {
                        //sndTouch.Clear();
                    }
                    sndTouch.SetTempo((float)tempo);
                }
            }
            catch (Exception ee)
            {

            }
        }
        public void SetPlayRate(double rate)
        {
            try
            {
                if (sndTouch != null)
                {
                    if (this.playRate * this.tempo < 1)
                    {
                        sndTouch.Clear();
                    }

                    sndTouch.SetRate((float)rate);
                }
                this.playRate = rate;
            }catch(Exception ee)
            {

            }
        }
        public void ClearSample()
        {
            sndTouch.Clear();
        }

        private int normalOrHiTempBufferNotification_V2(byte[] sampleBytes, double lenExt, Interfaces.EffectEventArgs e)
        {
            if (!e.Stream.CanRead)
            {
                return 0;
            }
            //int bytesRead = Stream.Read(e.NewSoundByte, 0, e.NumBytesRequired);
            int bytesRead = e.Stream.Read(sampleBytes, 0, (int)(e.NumBytesRequired * lenExt));
            if (sndTouch != null)
            {
                if (bytesRead % 2 > 0)
                {
                    bytesRead += 1;
                }
                short[] trimmedshorts = new short[bytesRead / 2];
                System.Buffer.BlockCopy(sampleBytes, 0, trimmedshorts, 0, bytesRead);
                sndTouch.PutSamples(trimmedshorts, (int)(trimmedshorts.Length / 2));
                {
                    short[] requireshort = new short[e.NumBytesRequired / 2];
                    byte[] trimmedBytes = new byte[e.NumBytesRequired];
                    int ret = sndTouch.ReceiveSamples(requireshort, requireshort.Length / 2);
                    System.Buffer.BlockCopy(requireshort, 0, trimmedBytes, 0, ret * 2 * Player.Channels);
                    e.NewSoundByte = trimmedBytes;
                }

            }
            else
            {
                byte[] trimmedBytes = new byte[e.NumBytesRequired];
                System.Buffer.BlockCopy(sampleBytes, 0, trimmedBytes, 0, e.NumBytesRequired);
                e.NewSoundByte = trimmedBytes;
            }
            return bytesRead;
        }
        private int slowerTempBufferNotification_V2(byte[] sampleBytes, double lenExt, Interfaces.EffectEventArgs e)
        {
            if (sndTouch.AvailableSamples >= e.NumBytesRequired / 2 * Player.Channels)
            {
                short[] requireshort = new short[e.NumBytesRequired / 2];
                byte[] trimmedBytes = new byte[e.NumBytesRequired];
                int ret = sndTouch.ReceiveSamples(requireshort, requireshort.Length / 2);
                System.Buffer.BlockCopy(requireshort, 0, trimmedBytes, 0, ret * 2 * Player.Channels);
                e.NewSoundByte = trimmedBytes;
                return 0;
            }
            else
            {
                return normalOrHiTempBufferNotification_V2(sampleBytes, lenExt, e);
            }
        }
        public bool Handle(Interfaces.EffectEventArgs e)
        {
            Boolean slower = false;

            double lenExt = playRate * tempo;
            if (lenExt < 1)
            {
                lenExt = 1;
                slower = true;
            }
            byte[] sampleBytes = new byte[(int)(e.NumBytesRequired * lenExt)];
            int bytesRead = 0;
            if (slower)
            {
                bytesRead = slowerTempBufferNotification_V2(sampleBytes, lenExt, e);
            }
            else
            {
                bytesRead = normalOrHiTempBufferNotification_V2(sampleBytes, lenExt, e);
            }

            return false;
        }

        public void SetVolume(double ratio)
        {
            sndTouch.Volume = ratio;
        }
    }
}
