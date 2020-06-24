using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using TimerDataBase.Abstractions;
using WebRubiksCubeTimer.Models.Users;
using WebRubiksCubeTimer.ViewModels.Home;
using WebRubiksCubeTimer.ViewModels.User;
using System.Collections.Generic;
using TimerDataBase.TableModels;
using Results.UserInterface;
using Microsoft.Extensions.Configuration;
using System;
using WebRubiksCubeTimer.Extensions;

namespace WebRubiksCubeTimer.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly ICategoryService _categoryService;
        private readonly ISeriesService _seriesService;
        private readonly ICubeService _cubeService;
        private readonly IConfiguration _configuration;

        public HomeController(ICategoryService categoryService,
            ISeriesService seriesService,
            UserManager<UserModel> userManager,
            ICubeService cubeService,
            IConfiguration configuration)
        {
            _categoryService = categoryService;
            _seriesService = seriesService;
            _userManager = userManager;
            _cubeService = cubeService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Rubik's Cube Timer";
            return View(await GetBestTimesOfAllTimes());
        }

        [HttpGet]
        public async Task<IActionResult> GetIndexChartData()
        {
            var output = new UserChartsDataViewModel();
            var categories = await _categoryService.GetAllCategoriesAsync();

            if (categories != null)
            {
                foreach (var category in categories)
                {
                    var amount = (await _seriesService.GetTotalAmountOfSeriesAsync(new List<Category>() { category }))?.FirstOrDefault();
                    if (amount.HasValue)
                    {
                        output.CategoryChartData.Add(new UserChartsDataViewModel.CategoryChartItem()
                        {
                            Category = category,
                            Amount = amount.Value,
                        });
                    }
                }

                output.CategoryChartData = output.CategoryChartData
                    .OrderByDescending(s => s.Amount)
                    .ToList();

                return Json(new Result(string.Empty, false)
                {
                    JsonData = output
                });
            }

            return Json(new Result(ApplicationResources.UserInterface.Common.NoDataToDisplay));
        }

        public IActionResult Cubes()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCubesData(string category, string manufacturer,
            string modelName, int page = 1)
        {
            var rawCubes = await _cubeService.GetAllCubesAsync();

            if (! string.IsNullOrEmpty(category))
            {
                rawCubes = rawCubes.Where(c => c.Category.Name.Contains(category, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (! string.IsNullOrEmpty(manufacturer))
            {
                rawCubes = rawCubes.Where(c => c.Manufacturer.Name.Contains(manufacturer, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (! string.IsNullOrEmpty(modelName))
            {
                rawCubes = rawCubes.Where(c => c.ModelName.Contains(modelName, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var pageSize = Convert.ToInt32(_configuration["Paginations:CubesExplorerPagination"]);

            var cubes = rawCubes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new CubeExplorerViewModel()
            {
                Pagination = this.GetPaginationData(rawCubes.Count(), pageSize, page),
                Cubes = cubes,
            };

            return Json(new Result(string.Empty, false)
            {
                JsonData = model
            });
        }

        private async Task<IndexBestTimesViewModel> GetBestTimesOfAllTimes()
        {
            var output = new IndexBestTimesViewModel();

            var categories = await _categoryService.GetAllCategoriesAsync();

            foreach (var category in categories)
            {
                var bestSerie = await _seriesService.GetBestSerieOfCategoryAsync(category);
                if (bestSerie != null)
                {
                    var user = await _userManager.FindByIdAsync(bestSerie.UserIdentity);
                    if (user != null)
                    {
                        output.CategoryBestTimes.Add(new IndexBestTimesViewModel.BestTimeItem()
                        {
                            Position = output.CategoryBestTimes.Count + 1,
                            Serie = bestSerie,
                            UserName = user.UserName,
                        });
                    }
                }
            }

            var cubes = await _cubeService.GetAllCubesAsync();

            foreach (var cube in cubes)
            {
                var bestSerie = await _seriesService.GetBestSerieOfCubeAsync(cube);
                if (bestSerie != null)
                {
                    var user = await _userManager.FindByIdAsync(bestSerie.UserIdentity);
                    if (user != null)
                    {
                        output.CubeBestTimes.Add(new IndexBestTimesViewModel.BestTimeItem()
                        {
                            Position = output.CubeBestTimes.Count + 1,
                            Serie = bestSerie,
                            UserName = user.UserName,
                        });
                    }
                }
            }

            return output;
        }
    }
}
