using System.Collections.Generic;

namespace WebRubiksCubeTimer.ViewModels.Series
{
    public class BestTimesViewModel
    {
        public BestTimesViewModel()
        {
            TimeCollection = new List<BestTimesCollectionViewModel>();
        }

        public int ViewType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IList<BestTimesCollectionViewModel> TimeCollection { get; set; }
    }
}
