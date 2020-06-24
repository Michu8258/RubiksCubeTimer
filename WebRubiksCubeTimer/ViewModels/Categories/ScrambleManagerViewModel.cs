using System.Collections.Generic;
using TimerDataBase.TableModels;

namespace WebRubiksCubeTimer.ViewModels.Categories
{
    public class ScrambleManagerViewModel
    {
        public ScrambleManagerViewModel()
        {
            CategoryName = "No category";
            CategoryId = 0;
            Scrambles = new List<Scramble>();
            AllCategories = new List<Category>();
            CategoriesWithNoScramble = new List<Category>();
        }

        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<Scramble> Scrambles { get; set; }
        public IEnumerable<Category> AllCategories { get; set; }
        public IEnumerable<Category> CategoriesWithNoScramble { get; set; }
    }
}
