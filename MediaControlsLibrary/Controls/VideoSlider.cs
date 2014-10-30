using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary.Controls
{
    public class VideoSlider : Slider
    {
        static VideoSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoSlider), new FrameworkPropertyMetadata(typeof(VideoSlider)));
        }
    }
}
