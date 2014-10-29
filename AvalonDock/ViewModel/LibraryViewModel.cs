namespace AvalonDock.ViewModel
{
    public class LibraryViewModel : Base.ViewViewModel
    {
        private string _text;
        public LibraryViewModel() : base("Library")
        {
            _text = "Library";
            
        }
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    RaisePropertyChanged("Text");
                }
            }
        }
    }
}
