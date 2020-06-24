using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebRubiksCubeTimer.ViewModels.Series
{
    public class SerieModificationViewModel
    {
        public long Identity { get; set; }
        public long SerieId { get; set; }
        public string UserId { get; set; }
        public string NewDuration { get; set; }
        public bool NewDnf { get; set; }
        public bool NewPenalty { get; set; }
        public bool NewModify { get; set; }
        public bool NewDelete { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
