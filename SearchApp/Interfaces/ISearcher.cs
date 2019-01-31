using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchApp.Interfaces
{
    public interface ISearcher
    {
        event EventHandler<(string fileName, double progress)> OnProcess;
        event EventHandler OnStart;
        event EventHandler OnStop;
        event EventHandler OnPause;
        event EventHandler OnResume;
        event EventHandler OnEnd;

        void Start(string directoryName, string fileName, bool isPartialName, bool ignoreCase, SearchOption option);
        void Stop();
        void Pause();
        void Resume();
    }
}
