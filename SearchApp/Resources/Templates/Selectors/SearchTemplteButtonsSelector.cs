using System;
using System.Windows;
using System.Windows.Controls;
using SearchApp.Enums;

namespace SearchApp.Resources.Templates.Selectors
{
    public class SearchTemplteButtonsSelector: DataTemplateSelector
    {
        public DataTemplate InProgressTemplate { get; set; }
        public DataTemplate InPauseTemplate { get; set; }
        public DataTemplate WaitForStartTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (item is OperationStatus status && element != null)
            {
                switch (status)
                {
                    case OperationStatus.InPause:
                        return InPauseTemplate;
                    case OperationStatus.InProgress:
                        return InProgressTemplate;
                    case OperationStatus.Stopped:
                        return WaitForStartTemplate;
                }
            }

            return null;
        }
    }
}
