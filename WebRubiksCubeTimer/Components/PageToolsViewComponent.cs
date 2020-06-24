using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimerDataBase.Abstractions;
using System.Linq;
using WebRubiksCubeTimer.ViewModels.ViewComponents;

namespace WebRubiksCubeTimer.Components
{
    public class PageToolsViewComponent : ViewComponent
    {
        private readonly PageToolsViewModel _model;
        private readonly IScrambleService _scrambleService;
        private readonly ICategoryService _categoryService;

        public PageToolsViewComponent(IScrambleService scrambleService,
            ICategoryService categoryService)
        {
            _model = new PageToolsViewModel();
            _scrambleService = scrambleService;
            _categoryService = categoryService;
        }

        public IViewComponentResult Invoke(SidebarViewModel model)
        {
            SetPinnedUserToolsEnabme();
            SetScrambleToolsEnable(model.CategoryID);
            _model.SetOverallVisibility();
            return View(_model);
        }

        private void SetPinnedUserToolsEnabme()
        {
            if (!_model.Menus.ContainsKey("Pinned_User"))
            {
                var available = false;

                if (User.IsInRole("Administrator"))
                {
                    if (HttpContext.Session.Keys.Contains(ApplicationResources.Static.SessionKeys.PinnedUserKey))
                    {
                        available = true;
                    }
                }

                _model.Menus.Add("Pinned_User", available);
            }
        }

        private void SetScrambleToolsEnable(int categoryId)
        {
            if(! _model.Menus.ContainsKey("Scramble"))
            {
                var available = false;
                var category = _categoryService.GetCategoryAsync(categoryId).Result;

                if (HttpContext.Request.RouteValues["controller"].ToString() == "Timer" && HttpContext.Request.RouteValues["action"].ToString() == "Series")
                {
                    var defaultScramble = _scrambleService.GetDefaultCompleteScrambleAsync(category).Result;
                    if (defaultScramble?.Moves?.Count() > 0)
                    {
                        available = true;
                    }
                }

                _model.Menus.Add("Scramble", available);
            }
        }
    }
}
