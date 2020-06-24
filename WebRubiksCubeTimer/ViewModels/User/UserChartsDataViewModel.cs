using System.Collections.Generic;
using TimerDataBase.TableModels;

namespace WebRubiksCubeTimer.ViewModels.User
{
    public class UserChartsDataViewModel
    {
        public UserChartsDataViewModel()
        {
            CategoryChartData = new List<CategoryChartItem>();
            CubeChartData = new List<CubeChartItem>();
        }

        public IList<CategoryChartItem> CategoryChartData { get; set; }
        public IList<CubeChartItem> CubeChartData { get; set; }

        public class CategoryChartItem
        {
            public Category Category { get; set; }
            public long Amount { get; set; }
        }

        public class CubeChartItem
        {
            public Cube Cube { get; set; }
            public long Amount { get; set; }
        }
    }
}
