namespace AvalonDock.ViewModel
{
    public class PlayerViewModel : Base.ViewViewModel
    {
        private string _text;
        public PlayerViewModel():base("Player")
        {
            _text = "Player View";
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
