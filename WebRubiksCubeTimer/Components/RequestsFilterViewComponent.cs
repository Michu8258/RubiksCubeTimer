using Microsoft.AspNetCore.Mvc;

namespace WebRubiksCubeTimer.Components
{
    public class RequestsFilterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
