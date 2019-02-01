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
        private static readonly ManualResetEvent _stopper = new ManualResetEvent(true);

        private CancellationTokenSource _tokenSource;

        private Task _searchTask;

        public event Action<double> OnProgress;
        public event Action<string> OnFind;
        public event Action OnStart;
        public event Action OnStop;
        public event Action OnPause;
        public event Action OnResume;
        public event Action OnEnd;
        public event Action<Exception> OnError;

        public void Start(string directoryName, string inSubDirectory, string regExpPattern, SearchOption option)
        {
            if (string.IsNullOrEmpty(directoryName) || string.IsNullOrWhiteSpace(directoryName))
                throw new ArgumentNullException(directoryName);

            _tokenSource?.Dispose();

            _tokenSource = new CancellationTokenSource();

            var token = _tokenSource.Token;

        }

        public void Start(string directoryName, string inSubDirectory, string fileName, bool isPartialName, bool ignoreCase, SearchOption option)
        {
            if (string.IsNullOrEmpty(directoryName) || string.IsNullOrWhiteSpace(directoryName))
                throw new ArgumentNullException(directoryName);

            _tokenSource?.Dispose();

            _tokenSource = new CancellationTokenSource();

            var token = _tokenSource.Token;

            fileName = fileName.Trim();

            if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName)) fileName = "*";
            else
            {
                if (isPartialName && (!fileName.StartsWith("*") || !fileName.EndsWith("*"))) fileName = "*" + fileName + "*";
            }

            _searchTask = Task.Factory.StartNew(
                async () => await Search(directoryName, fileName, option, token),
                token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            OnStart?.Invoke();
        }

        public void Stop()
        {
            _stopper.Set();
            _tokenSource.Cancel();
        }

        public void Pause()
        {
            _stopper.Reset();

            OnPause?.Invoke();
        }

        public void Resume()
        {
            _stopper.Set();
            OnResume?.Invoke();
        }

        private async Task Search(string path, string filePattern, SearchOption option, CancellationToken token)
        {
            try
            {
                if (option == SearchOption.TopDirectoryOnly)
                {
                    try
                    {
                        foreach (var file in Directory.GetFiles(path, filePattern, option))
                        {
                            _stopper.WaitOne();

                            OnFind?.Invoke(file);
                        }
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    await GetFilesFromDirectory(path, token);
                }

                OnEnd?.Invoke();
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e);
                OnStop?.Invoke();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
            }
        }

        private async Task GetFilesFromDirectory(string path, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            _stopper.WaitOne();

            try
            {
                var filesInCurrentDirectory = Directory.GetFiles(path);
                foreach (var s in filesInCurrentDirectory)
                {
                    token.ThrowIfCancellationRequested();
                    _stopper.WaitOne();
                    OnFind?.Invoke(s);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e);
            }

            token.ThrowIfCancellationRequested();
            _stopper.WaitOne();

            try
            {
                var directories = Directory.GetDirectories(path);
                if (directories.Any())
                {
                    var derictoriesTaskList = directories.Select(item => GetFilesFromDirectory(item, token));
                    await Task.WhenAll(derictoriesTaskList);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
