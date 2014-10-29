using Declarations;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Taygeta.Core;
namespace Player_mvvm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private D3D9Renderer m_render;
        private BitmapFormat m_format;
        private IMemoryRendererEx m_source;

        

        public MainWindow()
        {
            InitializeComponent();
            var dc = new MainWindowViewModel();
            DataContext = dc;
            //m_videoImage.Initialize(dc.Player);
            d3dimage.IsFrontBufferAvailableChanged += new DependencyPropertyChangedEventHandler(m_image_IsFrontBufferAvailableChanged);
            m_source = dc.Player;
            m_source.SetFormatSetupCallback(OnFormatSetup);
            m_source.SetCallback(OnNewFrame);
            
            dc.StopEvent += new System.EventHandler(StopPlayerEvent);
        }

        #region control d3dimage
        void m_image_IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!d3dimage.IsFrontBufferAvailable)
            {
                DiscardRenderer();
            }
            else
            {
                SetSurface(m_format);
            }
        }

        private void DiscardRenderer()
        {
            if (m_render != null)
            {
                m_render.Dispose();
                m_render = null;
            }
        }

        private void CreateRenderer()
        {
            DiscardRenderer();
            m_render = new D3D9Renderer(IntPtr.Zero);
            m_render.DisplayMode = VideoDisplayMode.KeepAspectRatio;
        }

        private BitmapFormat OnFormatSetup(BitmapFormat format)
        {
            m_format = format;
            SetSurface(format);
            return new BitmapFormat(format.Width, format.Height, ChromaType.I420);
        }

        private void SetSurface(BitmapFormat format)
        {
            CreateRenderer();
            m_render.CreateVideoSurface(format.Width, format.Height, PixelFormat.YV12, m_source.SAR.Ratio);
            d3dimage.Dispatcher.Invoke(new Action(delegate
            {
                d3dimage.Lock();
                d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, m_render.RenderTargetSurface);
                d3dimage.Unlock();
            }), DispatcherPriority.Send);
        }

        private void OnNewFrame(PlanarFrame frame)
        {
            d3dimage.Dispatcher.Invoke(new Action(delegate
            {
                if (!d3dimage.IsFrontBufferAvailable)
                {
                    return;
                }

                d3dimage.Lock();
                m_render.Display(frame.Planes[0], frame.Planes[2], frame.Planes[1], false);
                d3dimage.AddDirtyRect(new Int32Rect(0, 0, d3dimage.PixelWidth, d3dimage.PixelHeight));
                d3dimage.Unlock();
            }), DispatcherPriority.Send);
        }

        public D3D9Renderer RenderTarget
        {
            get
            {
                return m_render;
            }
        }

        public void Clear()
        {
            d3dimage.Dispatcher.BeginInvoke(new Action(delegate
            {
                d3dimage.Lock();
                m_render.Clear(System.Drawing.Color.Black);
                d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                d3dimage.Unlock();
            }), DispatcherPriority.Send);
        }
        void StopPlayerEvent(object sender, System.EventArgs e)
        {
            m_source.Dispose();
            this.Clear();
        }

        #endregion

        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    Clear();
        //    d3dimage.IsFrontBufferAvailableChanged -= new DependencyPropertyChangedEventHandler(m_image_IsFrontBufferAvailableChanged);
        //    DataContext = null;
        //    this.Clear();
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Clear();
        //}



        
    }
}
