using Microsoft.AspNetCore.Mvc;

namespace WebRubiksCubeTimer.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("/Error/StatusCodePage/{code:int}")]
        public IActionResult StatusCodePage(int code)
        {
            if (code == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View("UnidentifiedError");
            }
        }
    }
}
