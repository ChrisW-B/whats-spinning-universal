using Windows.UI.Xaml.Media.Imaging;

namespace UniversalMusicGame
{
    public class Song
    {
        public string title { get; set; }
        public string album { get; set; }
        public string artist { get; set; }
        public BitmapImage albumCover { get; set; }
    }
}
