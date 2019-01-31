using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SearchApp.Interfaces;
using SearchApp.Models;

namespace SearchApp.ViewModels
{
    internal class SearchViewModel: BaseViewModel
    {
        private bool _inProgress;
        private string _directoryName;
        private string _searchDirectoryName;
        private string _fileName;
        private bool _includeSubdirectories;
        private readonly ISearcher _sercher;

        public SearchViewModel()
        {
            _sercher = new Searcher();
        }

        public bool InProgress
        {
            get { return _inProgress; }
            set { _inProgress = value; OnPropertyChanged(); }
        }

        public bool IncludeSubdirectories
        {
            get { return _includeSubdirectories; }
            set { _includeSubdirectories = value; OnPropertyChanged(); }
        }

        public string SearchDirectoryName
        {
            get { return _searchDirectoryName; }
            set { _searchDirectoryName = value; OnPropertyChanged(); }
        }

        public string DirectoryName
        {
            get { return _directoryName; }
            set { _directoryName = value; OnPropertyChanged(); }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; OnPropertyChanged(); }
        }

    }
}
