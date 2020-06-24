using Microsoft.AspNetCore.Mvc;

namespace WebRubiksCubeTimer.Components
{
    public class RequestStateModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
