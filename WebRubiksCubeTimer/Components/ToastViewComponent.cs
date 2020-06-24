using Microsoft.AspNetCore.Mvc;

namespace WebRubiksCubeTimer.Components
{
    public class ToastViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
