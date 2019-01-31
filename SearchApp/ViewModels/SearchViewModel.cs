using SearchApp.Commands;
using SearchApp.Enums;
using SearchApp.Interfaces;
using SearchApp.Services;
using System;
using System.IO;
using System.Windows.Input;

namespace SearchApp.ViewModels
{
    internal class SearchViewModel: BaseViewModel
    {
        private OperationStatus _searchStatus;

        private string _directoryName;
        private string _searchDirectoryName;
        private string _fileName;

        private bool _includeSubdirectories;
        private bool _inProgress;

        private RelayCommand _startCommand;
        private RelayCommand _stopCommand;
        private RelayCommand _pauseCommand;
        private RelayCommand _resumeCommand;

        private double _progress;

        private readonly ISearcherService _sercherService;

        public event Action<bool> InProgressEvent;
        public event Action<string> OnFindFile;

        public SearchViewModel()
        {
            _sercherService = new SearcherService();
            _sercherService.OnProgress += SerchProgress;
            _sercherService.OnFind += SercherFind;
            _sercherService.OnEnd += SercherEnd;
            SearchStatus = OperationStatus.Stopped;
        }

        public ICommand StartCommand => _startCommand ??
                       (_startCommand = new RelayCommand(
                           StartSearch,
                           _ => !string.IsNullOrEmpty(SearchDirectoryName)));

        public ICommand StopCommand => _stopCommand ??
                       (_stopCommand = new RelayCommand(StopSearch));

        public ICommand PauseCommand => _pauseCommand ??
                       (_pauseCommand = new RelayCommand(PauseSearch));

        public ICommand ResumeCommand => _resumeCommand ??
                       (_resumeCommand = new RelayCommand(ResumeSearch));

        public bool InProgress
        {
            get => _inProgress;
            set
            {
                ChangeProperty(ref _inProgress, value);
                InProgressEvent?.Invoke(_inProgress);
            }
        }

        public OperationStatus SearchStatus
        {
            get => _searchStatus;
            set => ChangeProperty(ref _searchStatus, value);
        }

        public double Progress
        {
            get => _progress;
            set => ChangeProperty(ref _progress, value);
        }

        public bool IncludeSubdirectories
        {
            get => _includeSubdirectories;
            set => ChangeProperty(ref _includeSubdirectories, value);
        }

        public string SearchDirectoryName
        {
            get => _searchDirectoryName;
            set => ChangeProperty(ref _searchDirectoryName, value, ValidateSearchDirectory);
        }

        public string DirectoryName
        {
            get => _directoryName;
            set => ChangeProperty(ref _directoryName, value);
        }

        public string FileName
        {
            get => _fileName;
            set => ChangeProperty(ref _fileName, value);
        }

        private void StartSearch(object obj)
        {
            SearchStatus = OperationStatus.InProgress;
            _sercherService.Start(SearchDirectoryName, DirectoryName, FileName, false, false, IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            Progress = 0;
            InProgress = true;
        }

        private void StopSearch(object obj)
        {
            _sercherService.Stop();
            SearchStatus = OperationStatus.Stopped;
            InProgress = false;
        }

        private void PauseSearch(object obj)
        {
            _sercherService.Pause();
            SearchStatus = OperationStatus.InPause;
        }

        private void ResumeSearch(object obj)
        {
            _sercherService.Resume();
            SearchStatus = OperationStatus.InProgress;
        }

        private void SerchProgress(double obj)
        {
            Progress += obj;
        }

        private void SercherFind(string obj)
        {
            OnFindFile?.Invoke(obj);
        }

        private void SercherEnd()
        {
            SearchStatus = OperationStatus.Stopped;
            InProgress = false;
        }

        private void ValidateSearchDirectory(string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path)) return;
            if (!Directory.Exists(path)) throw new ArgumentException(nameof(path));
        }
    }
}
