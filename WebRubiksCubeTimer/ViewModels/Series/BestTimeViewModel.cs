using System;

namespace WebRubiksCubeTimer.ViewModels.Series
{
    public class BestTimeViewModel
    {
        public long SerieID { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime TimeStamp { get; set; }
        public string CategoryOption { get; set; }
        public bool DNF { get; set; }
        public bool Penalty { get; set; }
    }
}
