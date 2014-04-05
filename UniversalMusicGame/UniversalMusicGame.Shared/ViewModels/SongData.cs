using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace UniversalMusicGame.ViewModels
{
    public class SongData
    {
        public Uri albumUri { get; set; }

        public int seconds { get; set; }

        public string songName { get; set; }

        public bool correct { get; set; }

        public Uri uri { get; set; }

        public int points { get; set; }
    }

    internal class SongDataWithPicture
    {
        public Uri albumUri { get; set; }

        public string seconds { get; set; }

        public string songName { get; set; }

        public bool correct { get; set; }

        public Uri uri { get; set; }

        public string points { get; set; }

        public BitmapImage albumCover { get; set; }
    }
}
