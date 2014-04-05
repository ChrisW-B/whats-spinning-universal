using Nokia.Music;
using Nokia.Music.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UniversalMusicGame.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalMusicGame.Games
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NokiaMusicAlbum : Page
    {
        public string genre;
        public string name;

         #region variables
        private MusicClient client;
        private Random rand;
        private ObservableCollection<Product> topSongs;
        private ObservableCollection<Product> pickedSongs;
        private ObservableCollection<DataItemViewModel> albumArtList;
        private ObservableCollection<SongData> winningSongList;
        private bool gameOver;
        private int roundPoints;
        private ApplicationDataContainer store;
        private Product winningSong;
        private Product lastWinningSong;
        private int timesPlayed;
        private int points;
        private int numTimesWrong;
        private int numTicks;
        private DispatcherTimer playTime;
        private Grid grid;
        private Grid correctAnsGrid;
        private ProgressBar progBar;
        private bool isRight;

        private bool openInNokiaMusic = true;
        
        private enum ProgBarStatus
        {
            On,
            Off
        }

        private enum TimerStatus
        {
            On,
            Off,
            Pause
        }

        private enum AnsVisibility
        {
            On,
            Off
        }

        private const string MUSIC_API_KEY = "987006b749496680a0af01edd5be6493";
        #endregion
        public NokiaMusicAlbum()
        {
            InitializeComponent();
            initialize();
        }

        private void initialize()
        {
            client = new MusicClient(MUSIC_API_KEY);
            rand = new Random();
            pickedSongs = new ObservableCollection<Product>();
            albumArtList = new ObservableCollection<DataItemViewModel>();
            winningSongList = new ObservableCollection<SongData>();
            store = ApplicationData.Current.RoamingSettings;
            topSongs = new ObservableCollection<Product>();
            playTime = new DispatcherTimer();
            playTime.Interval = new TimeSpan(0, 0, 1);
            playTime.Tick += playTime_Tick;
            numTimesWrong = 0;
            timesPlayed = 0;
            points = 0;
            roundPoints = 0;
            gameOver = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
                Genre nokGenre = pickGenre(genre, name);
                setupGenre(nokGenre);
            
        }

        //creates the genre
        private Genre pickGenre(string genre, string name)
        {
            Genre nokGenre = new Genre();
            nokGenre.Id = genre;
            nokGenre.Name = name;
            return nokGenre;
        }

        //gets the list of top songs from a genre
        async private void setupGenre(Genre nokGenre)
        {
            await getTopMusic(nokGenre);
            pickWinner();
        }

        async private Task getTopMusic(Genre nokGenre)
        {
            ListResponse<Product> songPage = await client.GetTopProductsForGenreAsync(nokGenre, Category.Track, 0, 100);
            foreach (Product prod in songPage)
            {
                topSongs.Add(prod);
            }
            pickSongs();
        }

        async private Task toggleProgBar(ProgBarStatus stat)
        {
            if (winningSongList.Count == 0)
            {
                if (stat == ProgBarStatus.On)
                {
                    progBar = new ProgressBar();
                    grid = new Grid();
                    TextBlock text = new TextBlock();
                    text.Text = "loading the next song";
                    text.TextAlignment = TextAlignment.Center;
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.Foreground = new SolidColorBrush(Colors.White);
                    progBar.IsIndeterminate = true;
                    progBar.IsEnabled = true;
                    Thickness pad = new Thickness(0, 0, 0, 40);
                    progBar.Padding = pad;
                    SolidColorBrush brush = new SolidColorBrush(Colors.Black);
                    brush.Opacity = .7;
                    grid.Background = brush;
                    ContentPanel.Children.Add(grid);
                    grid.Children.Add(progBar);
                    grid.Children.Add(text);
                }
                else if (ContentPanel.Children.Contains(grid))
                {
                    progBar.IsIndeterminate = false;
                    grid.Children.Remove(progBar);
                    ContentPanel.Children.Remove(grid);
                }
            }
            else
            {
                if (stat == ProgBarStatus.On)
                {
                    await displayData(isRight, AnsVisibility.On, lastWinningSong);
                }
                else
                {
                    await displayData(isRight, AnsVisibility.Off, lastWinningSong);
                }
            }
        }

        async private Task displayData(bool win, AnsVisibility vis, Product song)
        {
            if (vis == AnsVisibility.On)
            {
                string winStat;
                if (win)
                {
                    winStat = "Congratulations! It ";
                }
                else
                {
                    winStat = "Nope, it ";
                }
                correctAnsGrid = new Grid();
                StackPanel panel = new StackPanel();

                //add background
                SolidColorBrush backgroundColor = new SolidColorBrush(Colors.Black);
                backgroundColor.Opacity = .7;
                correctAnsGrid.Background = backgroundColor;

                //show song name
                TextBlock text = new TextBlock();
                text.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
                text.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
                text.TextWrapping = TextWrapping.Wrap;
                text.TextAlignment = TextAlignment.Center;
                text.Foreground = new SolidColorBrush(Colors.White);
                text.Text = winStat + "was " + song.Name + " by " + song.Performers[0].Name;
                text.Padding = new Thickness(20, 20, 20, 20);
                panel.Children.Add(text);

                //show "loading" text
                TextBlock loadingText = new TextBlock();
                loadingText.Text = "loading the next song";
                loadingText.TextAlignment = TextAlignment.Center;
                loadingText.HorizontalAlignment = HorizontalAlignment.Center;
                loadingText.VerticalAlignment = VerticalAlignment.Center;
                loadingText.Foreground = new SolidColorBrush(Colors.White);

                //show progress bar
                progBar = new ProgressBar();
                progBar.IsIndeterminate = true;
                progBar.IsEnabled = true;
                Thickness pad = new Thickness(0, 0, 0, 40);
                progBar.Padding = pad;

                //show album art
                Image img = new Image();
                img.Height = 200;
                img.Width = 200;
                BitmapImage btmp = getBitmap(song);

                if (btmp != null)
                {
                    img.Source = btmp;
                    correctAnsGrid.Children.Add(img);
                }

                panel.Children.Add(loadingText);
                panel.Children.Add(progBar);
                panel.UpdateLayout();
                correctAnsGrid.Children.Add(panel);
                ContentPanel.Children.Add(correctAnsGrid);
                this.ContentPanel.UpdateLayout();
            }
            else if (ContentPanel.Children.Contains(correctAnsGrid))
            {
                await Task.Delay(new TimeSpan(0, 0, 2));
                correctAnsGrid.Children.Clear();
                ContentPanel.Children.Remove(correctAnsGrid);
                progBar.IsIndeterminate = false;
            }
        }

        //Pick and play the winning song
        private void pickWinner()
        {
            //picks a random song from the selected songs to be the winner

            winningSong = pickedSongs[rand.Next(pickedSongs.Count)];
            if (alreadyPicked(winningSong))
            {
                pickWinner();
            }
            else
            {
                playWinner();
            }
        }

        private bool alreadyPicked(Product winningSong)
        {
            foreach (SongData song in winningSongList)
            {
                if (song.uri == winningSong.WebUri)
                {
                    return true;
                }
            }
            return false;
        }

        async private void playWinner()
        {
            await toggleProgBar(ProgBarStatus.On);
            setAlbumArt();
            player.Resources.Clear();
            Uri songUri = client.GetTrackSampleUri(winningSong.Id);
            player.AutoPlay = false;
            player.Source = songUri;
            player.MediaOpened += player_MediaOpened;
            player.MediaFailed += player_MediaFailed;
        }

        private async void player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            await toggleProgBar(ProgBarStatus.Off);
        }

        private async void player_MediaOpened(object sender, RoutedEventArgs e)
        {
            await toggleProgBar(ProgBarStatus.Off);
            playForLimit();
        }

        private void playForLimit()
        {
            numTimesWrong = 0;
            timesPlayed = 0;
            numTicks = 25;
            toggleClock(TimerStatus.On);
            player.Play();
        }

        private void toggleClock(TimerStatus stat)
        {
            if (stat == TimerStatus.On)
            {
                timer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                playTime.Start();
            }
            else if (stat == TimerStatus.Off)
            {
                timer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                numTicks = 25;
                timer.Value = numTicks;
                playTime.Stop();
            }
            else
            {
                timer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                playTime.Stop();
            }
        }

        void playTime_Tick(object sender, object e)
        {
            timeLeft.Text = numTicks.ToString();
            timer.Value = (100.0*((double)numTicks) / 25));
            if (numTicks % 5 == 0 && numTicks != 25)
            {
                timesPlayed++;
            }
            if (numTicks < 0)
            {
                toggleClock(TimerStatus.Off);
                timeOut();
            }
            numTicks--;
        }

        //Get a list of 12 songs with album art
        private void pickSongs()
        {
            for (int i = 0; i < 12; i++)
            {
                pickSong();
            }
        }

        private void pickSong()
        {
            //picks an idividual song from the list of all songs in library
            Product prod = topSongs[rand.Next(topSongs.Count)];
            BitmapImage albumArt = getBitmap(prod);
            if (albumArt != null)
            {
                if (!onList(prod))
                {
                    pickedSongs.Add(prod);
                    DataItemViewModel album = new DataItemViewModel();
                    album.ImageSource = albumArt;
                    album.Prod = prod;
                    albumArtList.Add(album);
                }
                else
                {
                    pickSong();
                }
            }
            else
            {
                pickSong();
            }
        }

        private BitmapImage getBitmap(Product prod)
        {
            return new BitmapImage(prod.Thumb320Uri);
        }

        private void setAlbumArt()
        {
            //sets up the grid of album art
            albumArtGrid.ItemsSource = albumArtList;
        }

        private bool onList(Product prod)
        {
            foreach (Product picked in pickedSongs)
            {
                if (picked.TakenFrom.Name == prod.TakenFrom.Name)
                {
                    return true;
                }
            }
            return false;
        }

        //check correctness
        private void Image_Tap(object sender, PointerRoutedEventArgs e)
        {
            //checks to see if the correct answer was selected
            DataItemViewModel selected = ((sender as Image).DataContext as DataItemViewModel);
            if (selected.Title != MUSIC_API_KEY)
            {
                albumArtGrid.SelectedItem = selected;
                if (selected.Prod == winningSong)
                {
                    correctAns();
                }
                else
                {
                    removeFromList(selected);
                    wrongAns();
                }
            }
        }

        private void removeFromList(DataItemViewModel selected)
        {
            int i = 0;
            foreach (DataItemViewModel item in albumArtList)
            {
                if (item.Title != MUSIC_API_KEY)
                {
                    if (selected.Prod.Name == item.Prod.Name && selected.Prod.Duration == item.Prod.Duration)
                    {
                        break;
                    }
                }
                i++;
            }
            albumArtList.RemoveAt(i);
            //creates blank tile to prevent shifts
            DataItemViewModel wrong = new DataItemViewModel();
            wrong.ImageSource = new BitmapImage(new Uri("/Assets/x.png", UriKind.Relative));
            //API key unlikely to be song title
            wrong.Title = MUSIC_API_KEY;
            albumArtList.Insert(i, wrong);
        }

        private void wrongAns()
        {
            //handles incorrect answers
            roundPoints--;
            points--;
            numTimesWrong++;
            Points.Text = points.ToString() + "/30 Points";
            if (numTimesWrong > 2)
            {
                isRight = false;
                newBoard();
            }
        }

        private void correctAns()
        {
            //handles correct answers
            isRight = true;
            roundPoints += (5 - timesPlayed);
            points += (5 - timesPlayed);
            newBoard();
        }

        private void timeOut()
        {
            isRight = false;
            numTicks = 0;
            newBoard();
        }

        private void newBoard()
        {
            //clears the current board and creates a new one
            if (openInNokiaMusic)
            {
                winningSongList.Add(new SongData() { albumUri = winningSong.Thumb200Uri, points = roundPoints, correct = isRight, seconds = (25 - numTicks), songName = winningSong.Name, uri = winningSong.AppToAppUri });
            }
            else
            {
                winningSongList.Add(new SongData() { albumUri = winningSong.Thumb200Uri, points = roundPoints, correct = isRight, seconds = (25 - numTicks), songName = winningSong.Name, uri = winningSong.WebUri });
            }
            toggleClock(TimerStatus.Off);
            lastWinningSong = winningSong;
            if (winningSongList.Count > 5)
            {
                store.Values["results"] = winningSongList;
                if (gameOver)
                {
                    Frame.Navigate(typeof(ResultsPage));
                    gameOver = false;
                }
            }
            else
            {
                numTimesWrong = 0;
                roundPoints = 0;
                timesPlayed = 0;
                Points.Text = points.ToString() + "/30 Points";
                roundNum.Text = "Round " + (winningSongList.Count + 1) + "/6";
                player.Stop();
                player.Resources.Clear();
                albumArtList.Clear();
                pickedSongs.Clear();
                reInitialize();
                pickSongs();
                pickWinner();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // NavigationEventArgs returns destination page
            Page destinationPage = e.Content as Page;
            if (destinationPage != null && destinationPage.GetType().Equals(typeof(ResultsPage)))
            {
                // Change property of destination page
                (destinationPage as ResultsPage).isAlbum = true;
                (destinationPage as ResultsPage).genre = genre;
            }
        }

        private void reInitialize()
        {
            player.Source = null;
            albumArtList = null;
            pickedSongs = null;
            albumArtList = new ObservableCollection<DataItemViewModel>();
            pickedSongs = new ObservableCollection<Product>();
        }

        //override back button to stop games
        //protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        //{
        //    toggleClock(TimerStatus.Off);
        //    Frame.GoBack();
        //}
    }
}
