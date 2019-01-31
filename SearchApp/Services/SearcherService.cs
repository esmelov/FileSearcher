using SearchApp.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchApp.Services
{
    internal class SearcherService: ISearcherService
    {
        private readonly object lockObj = new object();

        private Task _searchTask;

        public event Action<double> OnProgress;
        public event Action<string> OnFind;
        public event Action OnStart;
        public event Action OnStop;
        public event Action OnPause;
        public event Action OnResume;
        public event Action OnEnd;

        public void Start(string directoryName, string inSubDirectory, string fileName, bool isPartialName, bool ignoreCase, SearchOption option)
        {
            if (string.IsNullOrEmpty(directoryName) || string.IsNullOrWhiteSpace(directoryName))
                throw new ArgumentNullException(directoryName);

            _searchTask = Task.Factory.StartNew(() => Search(directoryName, option));

            OnStart?.Invoke();
        }

        public void Stop()
        {
            OnStop?.Invoke();
        }

        public void Pause()
        {
            Monitor.Wait(lockObj);
            OnPause?.Invoke();
        }

        public void Resume()
        {
            Monitor.Pulse(lockObj);
            OnResume?.Invoke();
        }

        private void Search(string path, SearchOption option)
        {
            ConcurrentBag<string> files = new ConcurrentBag<string>();

            if (option == SearchOption.TopDirectoryOnly)
            {
                try
                {
                    files = new ConcurrentBag<string>(Directory.GetFiles(path).ToList());
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                GetFilesFromDirectory(path, files);
            }

            var totalCount = files.Count;

            foreach (var file in files)
            {
                OnFind?.Invoke(file);
                OnProgress?.Invoke(100d/totalCount);
            }

            OnEnd?.Invoke();
        }

        private void GetFilesFromDirectory(string path, ConcurrentBag<string> files)
        {
            try
            {
                var filesInCurrentDirectory = Directory.GetFiles(path);
                foreach (var s in filesInCurrentDirectory)
                {
                    files.Add(s);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e);
            }

            try
            {
                var directories = Directory.GetDirectories(path);
                if (directories.Any())
                {
                    foreach (var directory in directories)
                    {
                        GetFilesFromDirectory(directory, files);
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
