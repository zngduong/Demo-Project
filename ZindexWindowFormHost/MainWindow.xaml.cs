using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;

namespace ZindexWindowFormHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        IntPtr HWND_TOP = IntPtr.Zero;


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = System.Linq.Enumerable.Range(0, 1);
            System.Windows.Forms.Application.EnableVisualStyles();

            listBox.SelectionChanged += (sender, e) =>
            {
                if (listBox.IsLoaded)
                {
                    foreach (var item in e.AddedItems)
                    {
                        ListBoxItem listBoxItem = listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                        WindowsFormsHost host = GetHost(listBoxItem);
                        if (host != null)
                        {
                            host.RenderTransform = new System.Windows.Media.TranslateTransform() { X = 8, Y = 8 };

                            // Unfortunately, we need to pinvoke into SetWindowPos API to get this working.
                            SetWindowPos(host.Handle, HWND_TOP, 0, 0, 0, 0, 3);
                        }
                    }

                    foreach (var item in e.RemovedItems)
                    {
                        ListBoxItem listBoxItem = listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                        WindowsFormsHost host = GetHost(listBoxItem);
                        if (host != null)
                        {
                            host.RenderTransform = null;
                        }
                    }
                }
            };
        }

        private WindowsFormsHost GetHost(ListBoxItem item)
        {
            WindowsFormsHost host = null;
            if (item == null)
            {
                return host;
            }
            ContentPresenter contentPresenter = item.GetVisualChild<ContentPresenter>();
            if (contentPresenter != null)
            {
                host = item.ContentTemplate.FindName("wfh", contentPresenter) as WindowsFormsHost;
            }

            return host;
        }


    }

    public static class VisualExtensions
    {
        public static T GetVisualChild<T>(this Visual referenceVisual) where T : Visual
        {
            Visual child = null;
            for (Int32 i = 0; i < VisualTreeHelper.GetChildrenCount(referenceVisual); i++)
            {
                child = VisualTreeHelper.GetChild(referenceVisual, i) as Visual;
                if (child != null && (child.GetType() == typeof(T)))
                {
                    break;
                }
                else if (child != null)
                {
                    child = GetVisualChild<T>(child);
                    if (child != null && (child.GetType() == typeof(T)))
                    {
                        break;
                    }
                }
            }
            return child as T;
        }
    }
}
