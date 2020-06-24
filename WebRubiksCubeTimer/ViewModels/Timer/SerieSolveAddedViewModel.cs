using System;

namespace WebRubiksCubeTimer.ViewModels.Timer
{
    public class SerieSolveAddedViewModel
    {
        public long SeriesID { get; set; }
        public string UserID { get; set; }
        public string SeriesStartTime { get; set; }
        public bool SeriesFinished { get; set; }
        public bool AtLeastOneDNF { get; set; }

        public int NewTimeNumber { get; set; }
        public long NewTimeDurationMS { get; set; }
        public string NewTimeSpan { get; set; }
        public bool NewTimeDNF { get; set; }
        public bool NewTimePenalty { get; set; }

        public long WorstTimeMS { get; set; }
        public long BestTimeMS { get; set; }
        public long AverageTimeMS { get; set; }
        public long MeanOf3MS { get; set; }
        public long AverageOf5MS { get; set; }
    }
}
