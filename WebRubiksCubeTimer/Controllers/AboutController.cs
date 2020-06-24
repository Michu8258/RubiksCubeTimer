using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebRubiksCubeTimer.Controllers
{
    [Authorize]
    public class AboutController : Controller
    {
        public IActionResult Guidelines()
        {
            return View();
        }
    }
}
