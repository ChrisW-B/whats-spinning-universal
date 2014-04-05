using System.Linq;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalMusicGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HighScorePivot : Page
    {
        public bool isAlbum;
        private ApplicationDataContainer store;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);            
        }
        public HighScorePivot()
        {
            this.InitializeComponent();
            store = ApplicationData.Current.RoamingSettings;
            setupList();
        }

        private void setupList()
        {
            if ((store.Containers.Keys.Contains("HighScoreList")))
            {
                getHighScores();
            }
            else
            {
                noAlbumArtMessage();
            }
        }

        private void noAlbumArtMessage()
        {
            Grid grid = new Grid();
            grid.Background = new SolidColorBrush(Colors.Black);
            grid.Opacity = .7;
            TextBlock text = new TextBlock() { Text = "No results! Try playing some games and coming back later" };
            text.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            text.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            text.FontSize = 20;
            text.TextAlignment = TextAlignment.Center;
            text.TextWrapping = TextWrapping.Wrap;
            grid.Children.Add(text);
            GenresPivot.Children.Add(grid);
        }

        private void getHighScores()
        {
            HighScoreResults highScores = (HighScoreResults)store.Values["HighScoreList"];
            if (isAlbum)
            {
                GenresPivot.Children.Add(new TextBox() { Text = "albumview" });
                if (highScores.albumList != null)
                {
                    foreach (ScoreGenre sg in highScores.albumList)
                    {

                        StackPanel outer = new StackPanel();
                        Grid panel = new Grid();
                        Grid title = new Grid() { VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom, Margin = new Thickness(10, 0, 0, 20) };
                        TextBlock header = new TextBlock() { Text = sg.genre, VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom, FontSize = 30, FontFamily = new FontFamily("Segoe UI Light"), FontWeight = new Windows.UI.Text.FontWeight() { Weight = 0 } };
                        
                        ListBox lb = new ListBox();
                        foreach (Scores s in sg.scoreList)
                        {
                            ListBoxItem lbi = new ListBoxItem();
                            TextBlock tb = new TextBlock();
                            tb.Foreground = new SolidColorBrush(Colors.White);
                            tb.Margin = new Thickness(5, 5, 0, 0);
                            tb.Text = s.points + " points in " + s.totalTime + " seconds";
                            lbi.Content = tb;
                            lb.Items.Add(lbi);
                        }
                        title.Children.Add(header);
                        panel.Children.Add(lb);
                        panel.Children.Add(title);
                        outer.Children.Add(panel);
                        GenresPivot.Children.Add(outer);

                    }
                }
                else
                {
                    noAlbumArtMessage();
                }
            }
            else
            {
                if (highScores.voiceList != null)
                {

                    foreach (ScoreGenre sg in highScores.voiceList)
                    {
                        StackPanel outer = new StackPanel();
                        Grid panel = new Grid();
                        Grid title = new Grid() { VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom, Margin = new Thickness(10, 0, 0, 20) };
                        TextBlock header = new TextBlock() { Text = sg.genre, VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom, FontSize = 30, FontFamily = new FontFamily("Segoe UI Light"), FontWeight = new Windows.UI.Text.FontWeight() { Weight = 0 } };

                        ListBox lb = new ListBox();
                        foreach (Scores s in sg.scoreList)
                        {
                            ListBoxItem lbi = new ListBoxItem();
                            lbi.Content = s.points;
                            lb.Items.Add(lbi);
                        }
                        title.Children.Add(header);
                        panel.Children.Add(lb);
                        panel.Children.Add(title);
                        outer.Children.Add(panel);
                        GenresPivot.Children.Add(outer);
                    }
                }
                else
                {
                    noAlbumArtMessage();
                }
            }
        }
    }
}
