using System;
using System.IO;

namespace SearchApp.Interfaces
{
    public interface ISearcherService
    {
        event Action<double> OnProgress;
        event Action<string> OnFind;
        event Action OnStart;
        event Action OnStop;
        event Action OnPause;
        event Action OnResume;
        event Action OnEnd;
        event Action<Exception> OnError;

        void Start(string directoryName, string inSubDirectory, string fileName, bool isPartialName, bool ignoreCase, SearchOption option);
        void Stop();
        void Pause();
        void Resume();
    }
}
