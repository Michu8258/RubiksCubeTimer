using System.Collections.Generic;
using TimerDataBase.TableModels;

namespace WebRubiksCubeTimer.ViewModels.Home
{
    public class IndexBestTimesViewModel
    {
        public IndexBestTimesViewModel()
        {
            CategoryBestTimes = new List<BestTimeItem>();
            CubeBestTimes = new List<BestTimeItem>();
        }

        public IList<BestTimeItem> CategoryBestTimes { get; set; }
        public IList<BestTimeItem> CubeBestTimes { get; set; }

        public class BestTimeItem
        {
            public Serie Serie { get; set; }
            public string UserName { get; set; }
            public long Position { get; set; }
        }
    }
}
