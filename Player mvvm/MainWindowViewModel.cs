using Declarations;
using Declarations.Events;
using Declarations.Media;
using Declarations.Players;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Taygeta.Core;

namespace Player_mvvm
{
    public class MainWindowViewModel : ViewModelBase
    {
        IMediaPlayerFactory m_factory;
        IVideoPlayer m_player;
        IMediaFromFile m_media;

        private ImageSource _source;
        private string _time;
        private string _duration;
        private double _slider;
        private double _volume;
        private volatile bool m_isDrag = false;
        private bool _isPlaying;

        public event EventHandler StopEvent;

        private string FileName = @"C:\Users\cuchuoi137\Downloads\Video\1.MP4";
        #region constructor
        public MainWindowViewModel()
        {
            DispatcherHelper.Initialize(); 
            
            m_factory = new MediaPlayerFactory(true);
            m_player = m_factory.CreatePlayer<IVideoPlayer>();
            
            m_player.Events.PlayerPositionChanged += new EventHandler<MediaPlayerPositionChanged>(Events_PlayerPositionChanged);
            m_player.Events.TimeChanged += new EventHandler<MediaPlayerTimeChanged>(Events_TimeChanged);
            m_player.Events.MediaEnded += new EventHandler(Events_MediaEnded);
            m_player.Events.PlayerStopped += new EventHandler(Events_PlayerStopped);

            //VideoImageSource();
            //Initialize(m_player.CustomRendererEx);
            PlayVideo();
            Volume = m_player.Volume;
            
        }

        
        #endregion

        #region properties

        public IMemoryRendererEx Player
        {
            get { return m_player.CustomRendererEx; }
        }
        public string Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    RaisePropertyChanged("Duration");
                }
            }
        }

        public string Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (_time != value)
                {
                    _time = value;
                    RaisePropertyChanged("Time");
                }
            }
        }
        public double Slider
        {
            get
            {
                return _slider;
            }
            set
            {
                this.Set("Slider", ref this._slider, value);
            }
        }

        public double Volume
        {
            get { return _volume; }
            set 
            { 
                this.Set("Volume", ref this._volume , value);
                m_player.Volume = (int)_volume;
            }
        }

        #endregion

        #region event
        void Events_DurationChanged(object sender, MediaDurationChange e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(
                () =>
                {
                    Duration = TimeSpan.FromMilliseconds(e.NewDuration).ToString().Substring(0, 8);
                });
        }

        void Events_StateChanged(object sender, MediaStateChange e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(
                () =>
                {
                    // do some thing for MediaState play,pause or stop    
                });
        }

        void Events_PlayerPositionChanged(object sender, MediaPlayerPositionChanged e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(
                () =>
                {
                    if (!m_isDrag)
                    {
                        Slider = (double)e.NewPosition;
                    }
                });
        }

        void Events_TimeChanged(object sender, MediaPlayerTimeChanged e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(
                () =>
                {
                    Time = TimeSpan.FromMilliseconds(e.NewTime).ToString().Substring(0, 8);
                });
        }

        void Events_MediaEnded(object sender, EventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(
                () =>
                {
                    InitControl();
                });
        }

        void Events_PlayerStopped(object sender, EventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(
                () =>
                {
                    InitControl();
                });
        }
        #endregion

        #region Command

        RelayCommand<object> _playCommand = null;
        public ICommand Play
        {
            get
            {
                if (_playCommand == null)
                {
                    _playCommand = new RelayCommand<object>((p) => OnPlay(p), (p) => true);
                }
                return _playCommand;
            }
        }

        private void OnPlay(object parameter)
        {
            
            if (!_isPlaying == true)
            {
                m_player.Play();
                _isPlaying = !_isPlaying;
            }
            else
            {
                m_player.Pause();
                _isPlaying = !_isPlaying;
            }
        }


        RelayCommand<object> _stopCommand = null;
        public ICommand Stop
        {
            get
            {
                if (_stopCommand == null)
                {
                    _stopCommand = new RelayCommand<object>((p) => OnStop(p), (p) => true);
                }
                return _stopCommand;
            }
        }
        private void OnStop(object parameter)
        {
            if (StopEvent != null)
                StopEvent(this, null);
        }
        #endregion
        private void PlayVideo()
        {
            m_media = m_factory.CreateMedia<IMediaFromFile>(FileName);
            m_media.Events.DurationChanged += new EventHandler<MediaDurationChange>(Events_DurationChanged);
            m_media.Events.StateChanged += new EventHandler<MediaStateChange>(Events_StateChanged);

            m_player.Open(m_media);
            m_media.Parse(true);
        }

        private void InitControl()
        {
            Slider = 0;
            Time = "00:00:00";
            Duration = "00:00:00";
        }

        #region D3DImage Render
        #region Properties
        public ImageSource SourceD3D
        {
            get
            {
                return _source;
            }
        }
        #endregion

        private D3DImage m_image;
        private D3D9Renderer m_render;
        private BitmapFormat m_format;
        private IMemoryRendererEx m_source;

        public void VideoImageSource()
        {
            m_image = new D3DImage();
            m_image.IsFrontBufferAvailableChanged += new DependencyPropertyChangedEventHandler(m_image_IsFrontBufferAvailableChanged);
            _source = m_image;
        }

        void m_image_IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!m_image.IsFrontBufferAvailable)
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

        public void Initialize(IMemoryRendererEx renderer)
        {
            m_source = renderer;
            m_source.SetFormatSetupCallback(OnFormatSetup);
            m_source.SetCallback(OnNewFrame);
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
            m_render.CreateVideoSurface(format.Width, format.Height, Taygeta.Core.PixelFormat.YV12, m_source.SAR.Ratio);
            m_image.Dispatcher.Invoke(new Action(delegate
            {
                m_image.Lock();
                m_image.SetBackBuffer(D3DResourceType.IDirect3DSurface9, m_render.RenderTargetSurface);
                m_image.Unlock();
            }), DispatcherPriority.Send);
        }

        private void OnNewFrame(PlanarFrame frame)
        {
            m_image.Dispatcher.Invoke(new Action(delegate
            {
                if (!m_image.IsFrontBufferAvailable)
                {
                    return;
                }

                m_image.Lock();
                m_render.Display(frame.Planes[0], frame.Planes[2], frame.Planes[1], false);
                m_image.AddDirtyRect(new Int32Rect(0, 0, m_image.PixelWidth, m_image.PixelHeight));
                m_image.Unlock();
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
            m_image.Dispatcher.BeginInvoke(new Action(delegate
            {
                m_image.Lock();
                m_render.Clear(System.Drawing.Color.Black);
                m_image.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                m_image.Unlock();
            }), DispatcherPriority.Send);
        }
        #endregion

    }
}
