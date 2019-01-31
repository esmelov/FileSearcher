using System.Windows;
using System.Windows.Controls;

namespace SearchApp.Resources.Templates.Selectors
{
    public class SearchTemplteButtonsSelector: DataTemplateSelector
    {
        public DataTemplate InProgressTemplate { get; set; }
        public DataTemplate WaitForStartTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (item is bool inProgress && element != null)
            {
                if (inProgress)
                    return InProgressTemplate;
                else
                    return WaitForStartTemplate;
            }

            return null;
        }
    }
}
