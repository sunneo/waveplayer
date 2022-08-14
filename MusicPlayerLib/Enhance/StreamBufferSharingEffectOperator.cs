using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace WavePlayer.Enhance
{
    /// <summary>
    /// an effect handler to queue buffer for broadcasting
    /// </summary>
    public class StreamBufferSharingEffectOperator : Interfaces.IEffectOperator
    {
        MediaServer.MediaNode node;
        Locker mLocker = new Locker();
        AsyncTask task = new AsyncTask();
        LinkedList<byte[]> BufferQueue = new LinkedList<byte[]>();



        public StreamBufferSharingEffectOperator(MediaServer.MediaNode node)
        {
            this.node = node;
        }
        private void BroadcastToSubscriber()
        {
            List<String> servers = new List<string>(node.SubscribeServerList);
            byte[] samples = null;
            using(var locker=mLocker.Lock())
            {
                if(BufferQueue.Count ==0) return;
                samples = BufferQueue.First();
                BufferQueue.RemoveFirst();
            }
            for (int i = 0; i < servers.Count; ++i)
            {
                String key = servers[i];
                node.SendServerStreamToSubscriber(key, samples);
            }
        }
        public bool Handle(Interfaces.EffectEventArgs Args)
        {
            // push to buffer
            // add job to broadcast to subscriber
            if (node.SubscribeServerList.Count == 0)
            {
                return false;
            }
            bool shouldWait = false;
            using(var locker=mLocker.Lock())
            {
                BufferQueue.AddLast(Args.NewSoundByte);
                if (BufferQueue.Count > 128)
                {
                    shouldWait = true;
                }
            }
            task.AddAfterFinishJob(BroadcastToSubscriber);
            if (shouldWait)
            {
                task.FlushJob(true);
            }
            else
            {
                task.FlushJob(false);
            }
            return false;
        }
    }
}
