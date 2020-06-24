using System.Collections.Generic;
using TimerDataBase.TableModels;

namespace WebRubiksCubeTimer.ViewModels.Series
{
    public class SerieChartViewModel
    {
        public string UserName { get; set; }
        public Serie Serie { get; set; }
        public IEnumerable<Solve> Solves { get; set; }
    }
}
