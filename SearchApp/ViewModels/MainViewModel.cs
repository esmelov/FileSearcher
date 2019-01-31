using SearchApp.Commands;
using SearchApp.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace SearchApp.ViewModels
{
    internal class MainViewModel: BaseViewModel
    {
        private Node<DirectoryInfo> _currentDirectoryNode;
        private RelayCommand _onExpandNodeCommand;
        private ObservableCollection<Node<DirectoryInfo>> _directoryTree;
        private ObservableCollection<string> _findedFiles;
        private string _selectedFile;
        private bool _directoryTreeEnabled = true;

        public MainViewModel()
        {
            var drivers = Environment
                .GetLogicalDrives()
                .Select(item =>
                {
                    var driveInfo = new DirectoryInfo(item);
                    var driveNode = new Node<DirectoryInfo>(driveInfo);
                    var driveDirectorieeNodes = driveInfo.GetDirectories().Select(dir => new Node<DirectoryInfo>(dir));
                    driveNode.Nodes = new ObservableCollection<Node<DirectoryInfo>>(driveDirectorieeNodes);
                    return driveNode;
                });

            DirectoryTree = new ObservableCollection<Node<DirectoryInfo>>(drivers);

            SearcherVM = new SearchViewModel();
            SearcherVM.InProgressEvent += SearcherVM_InProgressEvent;
            SearcherVM.OnFindFile += SearcherVM_OnFindFile;
        }

        public ICommand OnExpandNodeCommand => _onExpandNodeCommand ??
                       (_onExpandNodeCommand = new RelayCommand(OnExpandNode));

        public SearchViewModel SearcherVM { get; set; }

        public bool DirectoryTreeEnabled
        {
            get => _directoryTreeEnabled;
            set => ChangeProperty(ref _directoryTreeEnabled, value);
        }

        public ObservableCollection<Node<DirectoryInfo>> DirectoryTree
        {
            get => _directoryTree;
            set => ChangeProperty(ref _directoryTree, value);
        }

        public ObservableCollection<string> FindedFiles
        {
            get => _findedFiles ??
                   (_findedFiles = new ObservableCollection<string>());
            set => ChangeProperty(ref _findedFiles, value);
        }

        public int FilesCount
        {
            get => FindedFiles.Count;
        }

        public Node<DirectoryInfo> CurrentDirectoryNode
        {
            get => _currentDirectoryNode;
            set
            {
                ChangeProperty(ref _currentDirectoryNode, value);
                SearcherVM.SearchDirectoryName = _currentDirectoryNode?.Current?.FullName;
            }
        }

        public string SelectedFile
        {
            get => _selectedFile;
            set => ChangeProperty(ref _selectedFile, value);
        }

        private void OnExpandNode(object obj)
        {
            if (obj is Node<DirectoryInfo> node)
            {
                foreach (var nodeItem in node.Nodes)
                {
                    nodeItem.Nodes.Clear();
                    try
                    {
                        nodeItem
                            .Current
                            .GetDirectories()
                            .Select(item => new Node<DirectoryInfo>(item))
                            .ToList()
                            .ForEach(item => nodeItem.Nodes.Add(item));
                    }
                    catch (UnauthorizedAccessException unauthorizedAccessException)
                    {
                        Console.WriteLine(unauthorizedAccessException);
                    }
                }
            }
        }

        private void SearcherVM_InProgressEvent(bool obj)
        {
            DirectoryTreeEnabled = !obj;
        }

        private void SearcherVM_OnFindFile(string obj)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => FindedFiles.Add(obj));
        }
    }
}
