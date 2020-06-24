using System;
using System.Collections.Generic;
using TimerDataBase.HelperModels;

namespace WebRubiksCubeTimer.ViewModels.Timer
{
    public class TimerViewModel
    {
        public TimerViewModel()
        {
            Solves = new List<SingleTimeViewModel>();
            ScrambleNames = new Dictionary<long, string>();
        }

        public string Username { get; set; }
        public string UserId { get; set; }
        public int CategoryID  { get; set; }
        public long CubeID{ get; set; }
        public int OptionID{ get; set; }
        public long SeriesId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryOptionName { get; set; }
        public string CubeDescription { get; set; }
        public DateTime StartTime { get; set; }
        public bool SerieFinished { get; set; }
        public bool AtLeastOneDNF { get; set; }
        public TimeSpan LongestResult { get; set; }
        public TimeSpan ShortestResult { get; set; }
        public TimeSpan AverageTime { get; set; }
        public TimeSpan MeanOf3 { get; set; }
        public TimeSpan AverageOf5 { get; set; }
        public IList<SingleTimeViewModel> Solves { get; set; }
        public IDictionary<long, string> ScrambleNames { get; set; }
        public CompleteScramble CurrentScramble { get; set; }

        public class SingleTimeViewModel
        {
            public int Number { get; set; }
            public TimeSpan Duration { get; set; }
            public DateTime TimeSpan { get; set; }
            public bool DNF { get; set; }
            public bool Penalty { get; set; }
        }
    }
}
