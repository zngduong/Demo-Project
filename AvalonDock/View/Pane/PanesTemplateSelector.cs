using AvalonDock.ViewModel;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;
namespace AvalonDock.View.Pane
{
    class PanesTemplateSelector : DataTemplateSelector
    {
        public PanesTemplateSelector()
        { }

        public DataTemplate LibraryViewTemplate { get; set; }

        public DataTemplate PlayerViewTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is LibraryViewModel)
                return LibraryViewTemplate;
            if (item is PlayerViewModel)
                return PlayerViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
