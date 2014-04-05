using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class MenuPage : Page
    {
        private bool isVoiceGame;
        public MenuPage()
        {
            this.InitializeComponent();
        }
        private void scores_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HighScores));
        }

        private void sayIt_Click(object sender, RoutedEventArgs e)
        {
            isVoiceGame = true;
            Frame.Navigate(typeof(CategorySelect));
        }

        private void albumPick_Click(object sender, RoutedEventArgs e)
        {
            isVoiceGame = false;
            Frame.Navigate(typeof(CategorySelect));
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // NavigationEventArgs returns destination page
            Page destinationPage = e.Content as Page;
            if (destinationPage != null && destinationPage.GetType().Equals(typeof(CategorySelect)))
            {
                // Change property of destination page
                (destinationPage as CategorySelect).isVoiceGame = isVoiceGame;
            }
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }
    }
}
