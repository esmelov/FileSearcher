using SearchApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchApp.Models
{
    internal class Searcher: ISearcher
    {
        private readonly object lockObj = new object();

        public event EventHandler<(string fileName, double progress)> OnProcess;
        public event EventHandler OnStart;
        public event EventHandler OnStop;
        public event EventHandler OnPause;
        public event EventHandler OnResume;
        public event EventHandler OnEnd;

        public void Start(string directoryName, string fileName, bool isPartialName, bool ignoreCase, SearchOption option)
        {
            OnStart?.Invoke(this, new EventArgs());
        }

        public void Stop()
        {
            OnStop?.Invoke(this, new EventArgs());
        }

        public void Pause()
        {
            Monitor.Wait(lockObj);
            OnPause?.Invoke(this, new EventArgs());
        }

        public void Resume()
        {
            Monitor.Pulse(lockObj);
            OnResume?.Invoke(this, new EventArgs());
        }

        private void Search()
        {
            OnProcess?.Invoke(this, (fileName: "", progress: 0.0));
            OnEnd?.Invoke(this, new EventArgs());
        }
    }
}
