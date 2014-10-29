﻿using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using Declarations;
using Declarations.Events;
using Declarations.Media;
using Declarations.Players;
using Implementation;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;

namespace PlayerBasic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IMediaPlayerFactory m_factory;
        IVideoPlayer m_player;
        IMediaFromFile m_media;
        private volatile bool m_isDrag;

        public MainWindow()
        {
            InitializeComponent();

            m_factory = new MediaPlayerFactory(true);
            m_player = m_factory.CreatePlayer<IVideoPlayer>();
            m_videoImage.Initialize(m_player.CustomRendererEx);

            m_player.Events.PlayerPositionChanged += new EventHandler<MediaPlayerPositionChanged>(Events_PlayerPositionChanged);
            m_player.Events.TimeChanged += new EventHandler<MediaPlayerTimeChanged>(Events_TimeChanged);
            m_player.Events.MediaEnded += new EventHandler(Events_MediaEnded);
            m_player.Events.PlayerStopped += new EventHandler(Events_PlayerStopped);

            slider2.Value = m_player.Volume;

        }

        void Events_PlayerStopped(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                m_videoImage.Clear();
                InitControls();
            }));
        }

        void Events_MediaEnded(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                InitControls();
            }));
        }

        private void InitControls()
        {
            slider1.Value = 0;
            label1.Content = "00:00:00";
            label3.Content = "00:00:00";
        }

        void Events_TimeChanged(object sender, MediaPlayerTimeChanged e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                label1.Content = TimeSpan.FromMilliseconds(e.NewTime).ToString().Substring(0, 8);
            }));
        }

        void Events_PlayerPositionChanged(object sender, MediaPlayerPositionChanged e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                if (!m_isDrag)
                {
                    slider1.Value = (double)e.NewPosition;
                }
            }));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                textBlock1.Text = ofd.FileName;
                m_media = m_factory.CreateMedia<IMediaFromFile>(ofd.FileName);
                m_media.Events.DurationChanged += new EventHandler<MediaDurationChange>(Events_DurationChanged);
                m_media.Events.StateChanged += new EventHandler<MediaStateChange>(Events_StateChanged);

                m_player.Open(m_media);
                m_media.Parse(true);
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {

            m_player.Play();
        }

        void Events_StateChanged(object sender, MediaStateChange e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {

            }));
        }

        void Events_DurationChanged(object sender, MediaDurationChange e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                label3.Content = TimeSpan.FromMilliseconds(e.NewDuration).ToString().Substring(0, 8);
            }));
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            m_player.Pause();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                m_player.Stop();
            });
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            m_player.ToggleMute();
        }

        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_player != null)
            {
                m_player.Volume = (int)e.NewValue;
            }
        }

        private void slider1_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            m_player.Position = (float)slider1.Value;
            m_isDrag = false;
        }

        private void slider1_DragStarted(object sender, DragStartedEventArgs e)
        {
            m_isDrag = true;
        }
    }
}
