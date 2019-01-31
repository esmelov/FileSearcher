using System.Windows;
using System.Windows.Controls;
using SearchApp.ViewModels;

namespace SearchApp.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void TreeViewItem_OnExpanded(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TreeViewItem item && this.DataContext is MainViewModel dataContext)
            {
                if (dataContext.OnExpandNodeCommand != null && dataContext.OnExpandNodeCommand.CanExecute(item))
                {
                    dataContext.OnExpandNodeCommand.Execute(item.Header);
                }
            }

        }
    }
}
