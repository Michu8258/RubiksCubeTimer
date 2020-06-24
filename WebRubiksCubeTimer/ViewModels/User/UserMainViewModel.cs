using System.Collections.Generic;
using TimerDataBase.TableModels;

namespace WebRubiksCubeTimer.ViewModels.User
{
    public class UserMainViewModel
    {
        public UserMainViewModel()
        {
            Categoreis = new List<CategoriesTableItem>();
            Cubes = new List<CubesTableItem>();
        }

        public string UserId { get; set; }
        public IList<CategoriesTableItem> Categoreis { get; set; }
        public IList<CubesTableItem> Cubes { get; set; }

        public class CategoriesTableItem
        {
            public Category Category { get; set; }
            public long Position { get; set; }
            public long SolvesAmount { get; set; }
        }

        public class CubesTableItem
        {
            public Cube Cube { get; set; }
            public long Position { get; set; }
            public long SolvesAmount { get; set; }
        }
    }
}
