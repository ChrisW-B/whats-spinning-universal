using System.Collections.ObjectModel;

namespace UniversalMusicGame
{
    public class HighScoreResults
    {
        public ObservableCollection<ScoreGenre> albumList { get; set; }

        public ObservableCollection<ScoreGenre> voiceList { get; set; }
    }

    public class Scores
    {
        public double points { get; set; }

        public int totalTime { get; set; }
    }

    public class ScoreGenre
    {
        public string genre { get; set; }

        public ObservableCollection<Scores> scoreList { get; set; }
    }
}