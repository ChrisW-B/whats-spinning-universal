using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UniversalMusicGame.Games;
using UniversalMusicGame.SelectGame;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalMusicGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategorySelect : Page
    {
        public bool isVoiceGame;

        public CategorySelect()
        {
            InitializeComponent();
        }


        private void myMusicGame_Click(object sender, RoutedEventArgs e)
        {
            if (isVoiceGame)
            {
                this.Frame.Navigate(typeof(MyMusicVoice));
            }
            else
            {
                this.Frame.Navigate(typeof(MyMusicAlbum));
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // NavigationEventArgs returns destination page
            Page destinationPage = e.Content as Page;
            if (destinationPage != null && destinationPage.GetType().Equals(typeof(GenreSelect)))
            {
                // Change property of destination page
                (destinationPage as GenreSelect).isVoiceGame = isVoiceGame;
            }
        }

        private void genreGame_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GenreSelect));
        }
    }
}
