using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using TimerDataBase.Abstractions;
using WebRubiksCubeTimer.Models.Users;

namespace WebRubiksCubeTimer.Components
{
    public class SeriesFilterModalViewComponent : ViewComponent
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly ICubeCollectionService _cubeCollectionService;
        private readonly IConfiguration _configuration;

        public SeriesFilterModalViewComponent(UserManager<UserModel> userManager,
            ICubeCollectionService cubeCollectionService, IConfiguration configuration)
        {
            _userManager = userManager;
            _cubeCollectionService = cubeCollectionService;
            _configuration = configuration;
        }

        public IViewComponentResult Invoke()
        {
            var model = new Dictionary<string, int>();

            var user = _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user?.Result != null)
            {
                var categories = (_cubeCollectionService.GetAllCubesOfUserAsync(user.Result.Id)).Result
                .Select(cc => cc.Category)
                .Distinct()
                .OrderBy(c => c.Name)
                .ToList();

                foreach (var category in categories)
                {
                    model.Add(category.Name, category.Identity);
                }
            }

            ViewBag.Limit = Convert.ToInt32(_configuration["DefaultValues:SeriesDefaultAmount"]);

            return View(model);
        }
    }
}
