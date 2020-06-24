using System.Collections.Generic;
using System.Linq;

namespace WebRubiksCubeTimer.ViewModels.ViewComponents
{
    public class PageToolsViewModel
    {
        public bool ToolsVisible { get; set; }

        public Dictionary<string, bool> Menus { get; set; }

        public PageToolsViewModel()
        {
            Menus = new Dictionary<string, bool>();
        }

        public void SetOverallVisibility()
        {
            ToolsVisible = Menus.Values.Any(v => v == true);
        }
    }
}
