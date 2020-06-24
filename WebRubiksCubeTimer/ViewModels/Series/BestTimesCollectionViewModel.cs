using System.Collections.Generic;

namespace WebRubiksCubeTimer.ViewModels.Series
{
    public class BestTimesCollectionViewModel
    {
        public BestTimesCollectionViewModel()
        {
            Times = new List<BestTimeViewModel>();
        }

        public string Description { get; set; }
        public IList<BestTimeViewModel> Times { get; set; }
    }
}
