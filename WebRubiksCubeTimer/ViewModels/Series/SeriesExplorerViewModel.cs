using System.Collections.Generic;
using TimerDataBase.TableModels;
using WebRubiksCubeTimer.Models.Pagination;

namespace WebRubiksCubeTimer.ViewModels.Series
{
    public class SeriesExplorerViewModel
    {
        public PaginationModel Pagination { get; set; }
        public IEnumerable<Serie> SerieData { get; set; }
    }
}
