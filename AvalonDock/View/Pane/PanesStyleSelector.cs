using AvalonDock.ViewModel;
using System.Windows;
using System.Windows.Controls;
namespace AvalonDock.View.Pane
{
    public class PanesStyleSelector : StyleSelector
    {
        public Style LibraryStyle { get; set; }

        public Style PlayerStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is LibraryViewModel)
                return LibraryStyle;
            if (item is PlayerViewModel)
                return PlayerStyle;

            return base.SelectStyle(item, container);
        }
    }
}
