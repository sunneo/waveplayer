using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackServer
{
    public class DisposableWrapper : IDisposable
    {
        private event EventHandler Disposed;
        public void Dispose()
        {
            if (Disposed != null)
            {
                Disposed(this, EventArgs.Empty);
            }
        }
        public DisposableWrapper(Action action)
        {
            if (action != null)
            {
                Disposed += new EventHandler((sender, args) =>
                {
                    action();
                });
            }
        }
    }
}
