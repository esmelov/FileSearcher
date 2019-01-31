using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SearchApp.Commands;
using SearchApp.Models;

namespace SearchApp.ViewModels
{
    internal class MainViewModel: BaseViewModel
    {
        private Node<DirectoryInfo> _currentDirectoryNode;
        private RelayCommand _onExpandNodeCommand;
        private ObservableCollection<Node<DirectoryInfo>> _directoryTree;

        public MainViewModel()
        {
            var drivers = Environment
                .GetLogicalDrives()
                .Select(item =>
                {
                    var driveInfo = new DirectoryInfo(item);
                    var driveNode = new Node<DirectoryInfo>(new DirectoryInfo(item));
                    var driveDirectorieeNodes = driveInfo.GetDirectories().Select(dir => new Node<DirectoryInfo>(dir));
                    driveNode.Nodes = new ObservableCollection<Node<DirectoryInfo>>(driveDirectorieeNodes);
                    return driveNode;
                });

            DirectoryTree = new ObservableCollection<Node<DirectoryInfo>>(drivers);

            SearcherVM = new SearchViewModel();
        }

        public ICommand OnExpandNodeCommand
        {
            get
            {
                return _onExpandNodeCommand ??
                       (_onExpandNodeCommand = new RelayCommand(OnExpandNode));
            }
        }

        public SearchViewModel SearcherVM { get; set; }

        public ObservableCollection<Node<DirectoryInfo>> DirectoryTree
        {
            get { return _directoryTree; }
            set
            {
                if (_directoryTree != value)
                {
                    _directoryTree = value;
                    OnPropertyChanged();
                }
            }
        }

        public Node<DirectoryInfo> CurrentDirectoryNode {
            get { return _currentDirectoryNode; }
            set
            {
                if (_currentDirectoryNode != value)
                {
                    _currentDirectoryNode = value;
                    SearcherVM.SearchDirectoryName = value?.Current?.FullName;
                    OnPropertyChanged();
                }
            }
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
    }
}
