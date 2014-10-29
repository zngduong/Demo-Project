using AvalonDock.ViewModel.Base;
using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace AvalonDock.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        ViewViewModel[] _views = null;
        LibraryViewModel _libraryView = null;
        PlayerViewModel _playerView = null;
        public MainViewModel()
        {
            
        }

        public IEnumerable<ViewViewModel> Views
        {
            get
            {
                if (_views == null)
                    _views = new ViewViewModel[] { LibraryView, PlayerView};
                return _views;
            }
        }

        public LibraryViewModel LibraryView
        {
            get
            {
                if (_libraryView == null)
                    _libraryView = new LibraryViewModel();
                return _libraryView;
            }
        }
        public PlayerViewModel PlayerView
        {
            get
            {
                if (_playerView == null)
                    _playerView = new PlayerViewModel();
                return _playerView;
            }
        }

    }
}