using System.Collections.Generic;
using TimerDataBase.TableModels;

namespace WebRubiksCubeTimer.ViewModels.CubeManagement
{
    public class CubesPageViewModel
    {
        public IEnumerable<PlasticColor> PlasticColors { get; set; }
        public IEnumerable<Manufacturer> Manufacturers { get; set; }
        public IEnumerable<Cube> Cubes { get; set; }
    }
}
