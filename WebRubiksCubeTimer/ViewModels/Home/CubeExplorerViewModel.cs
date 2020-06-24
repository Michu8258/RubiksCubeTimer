using System.Collections.Generic;
using TimerDataBase.TableModels;
using WebRubiksCubeTimer.Models.Pagination;

namespace WebRubiksCubeTimer.ViewModels.Home
{
    public class CubeExplorerViewModel
    {
        public PaginationModel Pagination { get; set; }
        public IEnumerable<Cube> Cubes { get; set; }
    }
}
