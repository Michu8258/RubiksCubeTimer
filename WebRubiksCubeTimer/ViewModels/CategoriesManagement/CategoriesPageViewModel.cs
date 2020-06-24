using System.Collections.Generic;
using TimerDataBase.TableModels;

namespace WebRubiksCubeTimer.ViewModels.CategoriesManagement
{
    public class CategoriesPageViewModel
    {
        public IEnumerable<CategoryOption> CategoryOptions { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public bool OptionsModifyPermission { get; set; }
    }
}
