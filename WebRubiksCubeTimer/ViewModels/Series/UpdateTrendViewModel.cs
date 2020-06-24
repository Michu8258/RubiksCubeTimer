using System.Collections.Generic;
using TimerDataBase.TableModels;

namespace WebRubiksCubeTimer.ViewModels.Series
{
    public class UpdateTrendViewModel
    {
        public string TrendDescription { get; set; }
        public IEnumerable<Serie> Series { get; set; }
    }
}
