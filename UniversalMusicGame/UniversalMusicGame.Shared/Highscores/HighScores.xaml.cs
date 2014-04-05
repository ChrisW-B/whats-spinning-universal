using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class HighScores : Page
    {
        public HighScores()
        {
            this.InitializeComponent();
            
        }
        bool albumScores;

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // NavigationEventArgs returns destination page
            Page destinationPage = e.Content as Page;
            if (destinationPage != null && destinationPage.GetType().Equals(typeof(HighScorePivot)))
            {
                // Change property of destination page
                (destinationPage as HighScorePivot).isAlbum = albumScores;
            }
        }
        private void album_Tap(object sender, RoutedEventArgs e)
        {
            albumScores = true;
            this.Frame.Navigate(typeof(HighScorePivot));
        }

        private void voice_Tap(object sender, RoutedEventArgs e)
        {
            albumScores = false;
            this.Frame.Navigate(typeof(HighScorePivot));
        }

        private void clear_Tap(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer store  = ApplicationData.Current.RoamingSettings;
            if (store.Containers.Keys.Contains("HighScoreList"))
            {
                store.Values.Remove("HighScoreList");
            }
        }

    }
}
