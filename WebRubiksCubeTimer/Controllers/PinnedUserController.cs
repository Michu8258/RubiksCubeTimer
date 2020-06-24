using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WebRubiksCubeTimer.Models.Users;

namespace WebRubiksCubeTimer.Controllers
{
    public class PinnedUserController : Controller
    {
        public PinnedUserController()
        {

        }

        public IActionResult UserMainPage()
        {
            var id = GetSessionPinnedUserId();
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("ShowMyAccount", "User", new { userName = GetCurrentUserName() });
            }

            return RedirectToAction("Account", "User", new { id });
        }
        public IActionResult UserCubesCollection()
        {
            var id = GetSessionPinnedUserId();
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("ShowMyCubesCollection", "User", new { userName = GetCurrentUserName() });
            }

            return RedirectToAction("CubesCollection", "User", new { userId = id });
        }
        public IActionResult UserBestResults()
        {
            var id = GetSessionPinnedUserId();
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Best", "Series", new { userName = GetCurrentUserName() });
            }

            return RedirectToAction("BestResults", "Series", new { id });
        }
        public IActionResult UserCategoryTrend()
        {
            var id = GetSessionPinnedUserId();
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Trend", "Series", new { userName = GetCurrentUserName() });
            }

            return RedirectToAction("CategoryTrend", "Series", new { id });
        }
        public IActionResult UserSeriesExplorer()
        {
            var id = GetSessionPinnedUserId();
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Explore", "Series", new { userName = GetCurrentUserName() });
            }

            return RedirectToAction("SeriesExplorer", "Series", new { id });
        }

        private string GetSessionPinnedUserId()
        {
            if (HttpContext.Session.Keys.Contains(ApplicationResources.Static.SessionKeys.PinnedUserKey))
            {
                return HttpContext.Session.GetString(ApplicationResources.Static.SessionKeys.PinnedUserKey);
            }

            return string.Empty;
        }

        private string GetCurrentUserName()
        {
            return User?.Identity?.Name;
        }
    }
}
