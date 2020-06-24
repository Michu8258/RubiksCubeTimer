using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Reports.Abstractions;
using Results.UserInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimerDataBase.Abstractions;
using TimerDataBase.TableModels;
using WebRubiksCubeTimer.Extensions;
using WebRubiksCubeTimer.Models.Users;
using WebRubiksCubeTimer.ViewModels.Series;

namespace WebRubiksCubeTimer.Controllers
{
    public class SeriesController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly ISeriesService _seriesService;
        private readonly ICubeCollectionService _cubeCollectionService;
        private readonly ICubeService _cubeService;
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _configuration;
        private readonly IReportService _reportService;

        public SeriesController(UserManager<UserModel> userManager,
            ISeriesService seriesService,
            ICubeCollectionService cubeCollectionService,
            ICubeService cubeService,
            ICategoryService categoryService,
            IConfiguration configuration,
            IReportService reportService)
        {
            _userManager = userManager;
            _seriesService = seriesService;
            _cubeCollectionService = cubeCollectionService;
            _cubeService = cubeService;
            _categoryService = categoryService;
            _configuration = configuration;
            _reportService = reportService;
        }

        public async Task<IActionResult> Best(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    return RedirectToAction("BestResults", "Series", new { id = user.Id, type = 0 });
                }
            }

            return RedirectToAction("Index", "Home");
        }

        //type != 0 => category, type == 1 => cube
        public async Task<IActionResult> BestResults(string id, int type = 0)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("StatusCodePage", "Error", new { code = 404 });
            }
            else if (user != null && ((user.UserName == User.Identity.Name) || User.IsInRole("Administrator")))
            {
                return View(await CollectBestTimes(user.Id, type));
            }
            else
            {
                return RedirectToAction("AccesDenied", "Login", new { returnUrl = "/Series/BestResults/" + user.Id + "/" + type });
            }
        }

        public async Task<IActionResult> Trend(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    return RedirectToAction("CategoryTrend", "Series", new { id = user.Id });
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CategoryTrend(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            {
                if(user == null)
                {
                    return RedirectToAction("StatusCodePage", "Error", new { code = 404 });
                }
                else if(user != null && ((user.UserName == User?.Identity?.Name) || User.IsInRole("Administrator")))
                {
                    return View(CreateCategoryTrendStartModel(user, new SolvesTrendViewModel()));
                }
                else
                {
                    return RedirectToAction("AccesDenied", "Login", new { returnUrl = "/Series/CategoryTrend/" + user.Id });
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTrendDefaultData(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var category = (await _seriesService.GetLastSerieOfUserAsync(user.Id))?.Cube?.Category;
                
                if (category != null)
                {
                    var series = await (_seriesService.GetSeriesOfUserFilteredAsync(user.Id,
                        50, null, null, category, null, null));

                    return Json(new Result(string.Empty, false)
                    {
                        JsonData = new UpdateTrendViewModel()
                        {
                            TrendDescription = string.Empty,
                            Series = series,
                        }
                    });
                }

                return Json(new Result(ApplicationResources.UserInterface.Common.NoDataToDisplay));
            }

            return Json(new Result(ApplicationResources.UserInterface.Common.SomethingWrong));
        }

        [HttpGet]
        public async Task<IActionResult> GetTrendFilteredData(string id, int limit, string startDate,
            string endDate, int categoryId, long cubeId, int categoryOptionId)
        {
            DateTime start;
            DateTime stop;
            (start, stop) = ProcessStartAndEndDate(startDate, endDate);

            var user = await _userManager.FindByIdAsync(id);
            var category = await _categoryService.GetCategoryAsync(categoryId);
            var cube = await _cubeService.GetCubeAsync(cubeId);
            var categoryOption = await _categoryService.GetSingleCategoryOptionAsync(categoryOptionId);

            if (user != null)
            {
                var series = await _seriesService.GetSeriesOfUserFilteredAsync(user.Id, limit,
                    start, stop, category, cube, categoryOption);

                var model = new UpdateTrendViewModel()
                {
                    Series = series,
                };

                CreateTredDescription(model, start, stop, category, cube, categoryOption, limit);

                return Json(new Result(string.Empty, false)
                {
                    JsonData = model
                });
            }

            return Json(new Result(ApplicationResources.UserInterface.Common.SomethingWrong));
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCubesInCategoryList(string id, int categoryId)
        {
            var user = await _userManager.FindByIdAsync(id);
            var category = await _categoryService.GetCategoryAsync(categoryId);

            if (user != null && category != null)
            {
                var cubes = (await _cubeCollectionService.GetAllCubesOfUserAsync(user.Id))
                .Where(cc => cc.Category == category)
                .Distinct()
                .ToList();

                return Json(new Result(string.Empty, false)
                {
                    JsonData = cubes
                });
            }
            else
            {
                return Json(new Result(ApplicationResources.UserInterface.Common.SomethingWrong));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCategoryOptionsOfCubeList(string id, long cubeId)
        {
            var user = await _userManager.FindByIdAsync(id);
            var cube = await _cubeService.GetCubeAsync(cubeId);

            if (user != null && cube != null)
            {
                var options = (await _categoryService.GetOptionsForCubeAsync(cube))
                    .OrderBy(o => o.Name)
                    .Distinct()
                    .ToList();

                return Json(new Result(string.Empty, false)
                {
                    JsonData = options
                });
            }
            else
            {
                return Json(new Result(ApplicationResources.UserInterface.Common.SomethingWrong));
            }
        }

        public async Task<IActionResult> Explore(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    return RedirectToAction("SeriesExplorer", "Series", new { id = user.Id});
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> SeriesExplorer(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            {
                if (user == null)
                {
                    return RedirectToAction("StatusCodePage", "Error", new { code = 404 });
                }
                else if (user != null && ((user.UserName == User?.Identity?.Name) || User.IsInRole("Administrator")))
                {
                    return View(CreateCategoryTrendStartModel(user, new SolvesTrendViewModel()));
                }
                else
                {
                    return RedirectToAction("AccesDenied", "Login", new { returnUrl = "/Series/SeriesExplorer/" + user.Id });
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSeriesData(string id, int limit, string startDate,
            string endDate, int categoryId, long cubeId, int categoryOptionId, int page = 1)
        {
            DateTime start;
            DateTime stop;
            (start, stop) = ProcessStartAndEndDate(startDate, endDate);

            var user = await _userManager.FindByIdAsync(id);
            var category = await _categoryService.GetCategoryAsync(categoryId);
            var cube = await _cubeService.GetCubeAsync(cubeId);
            var categoryOption = await _categoryService.GetSingleCategoryOptionAsync(categoryOptionId);

            var pageSize = Convert.ToInt32(_configuration["Paginations:SeriesExplorerPagination"]);

            if (user != null)
            {
                var rawSeries = (await _seriesService.GetSeriesOfUserFilteredAsync(user.Id, limit,
                    start, stop, category, cube, categoryOption))
                    .OrderByDescending(s => s.StartTimeStamp)
                    .ToList();

                var series = rawSeries
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                var model = new SeriesExplorerViewModel()
                {
                    Pagination = this.GetPaginationData(rawSeries.Count(), pageSize, page),
                    SerieData = series,
                };

                return Json(new Result(string.Empty, false)
                {
                    JsonData = model,
                });
            }

            return Json(new Result(ApplicationResources.UserInterface.Common.SomethingWrong));
        }

        [HttpGet]
        public async Task<IActionResult> GetSerieChartData(string id, long serieId)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if ((User?.Identity?.Name == user.UserName) || User.IsInRole("Administrator"))
                {
                    var serie = await _seriesService.GetSpecificSerieAsync(serieId);
                    if (serie != null)
                    {
                        var solves = await _seriesService.GetAllSolvesOfSerieAsync(serieId);
                        if (solves != null && solves.Any())
                        {
                            return Json(new Result(string.Empty, false)
                            {
                                JsonData = new SerieChartViewModel()
                                {
                                    UserName = user.UserName,
                                    Serie = serie,
                                    Solves = solves,
                                }
                            });
                        }
                        else
                        {
                            return Json(new Result(ApplicationResources.UserInterface.Common.NoSolvesInSerie));
                        }
                    }
                    else
                    {
                        return Json(new Result(ApplicationResources.TimerDB.Messages.SerieDoesNotExist));
                    }
                }
                else
                {
                    return Json(new Result(ApplicationResources.UserInterface.Common.UserNotOwnsSerie));
                }
            }
            else
            {
                return Json(new Result(ApplicationResources.UserInterface.Common.UserDoNotExist));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSerieReport(long serieId)
        {
            var report = await _reportService.GenerateSeriesReport(serieId);
            return Json(report);
        }

        private async Task<BestTimesViewModel> CollectBestTimes(string userId, int type)
        {
            var outputModel = new BestTimesViewModel()
            {
                UserId = userId,
                UserName = (await _userManager.FindByIdAsync(userId))?.UserName,
                ViewType = type == 1 ? 1 : 0,
            };

            if (type == 1)
            {
                await CollectBestTimesCube(userId, outputModel);
            }
            else
            {
                await CollectBestTimesCategory(userId, outputModel);
            }

            return outputModel;
        }

        private async Task CollectBestTimesCategory(string userId, BestTimesViewModel model)
        {
            var allSeries = await _seriesService.GetAllSeriesOfUserAsync(userId);
            var categories = (await _cubeCollectionService.GetAllCubesOfUserAsync(userId))
                .Select(cc => cc.Category)
                .Distinct()
                .ToList();

            if (allSeries != null && categories != null)
            {
                foreach (var category in categories)
                {
                    var options = allSeries
                        .Where(s => s.Cube.Category == category)
                        .Select(s => s.SerieOption)
                        .Distinct()
                        .ToList();

                    var singleCategoryBestTimes = new BestTimesCollectionViewModel()
                    {
                        Description = category.Name,
                    };

                    foreach (var option in options)
                    {
                        var serie = allSeries
                            .Where(s => s.Cube.Category == category && s.SerieOption == option)
                            .OrderBy(s => s.ShortestResult)
                            .ToList()
                            .First();

                        var solve = (await _seriesService.GetAllSolvesOfSerieAsync(serie.Identity))
                            .OrderBy(s => s.Duration)
                            .FirstOrDefault();
                        
                        if (solve != null)
                        {
                            singleCategoryBestTimes.Times.Add(new BestTimeViewModel()
                            {
                                SerieID = serie.Identity,
                                Duration = solve.Duration,
                                TimeStamp = solve.FinishTimeSpan,
                                CategoryOption = option.Name,
                                DNF = solve.DNF,
                                Penalty = solve.PenaltyTwoSeconds,
                            });
                        }
                    }

                    model.TimeCollection.Add(singleCategoryBestTimes);
                }

                model.TimeCollection
                    .OrderBy(ct => ct.Description);

                foreach (var times in model.TimeCollection)
                {
                    times.Times = times.Times.OrderBy(t => t.Duration).ToList();
                }
            }
        }

        private async Task CollectBestTimesCube(string userId, BestTimesViewModel model)
        {
            var allCubes = await _cubeCollectionService.GetAllCubesOfUserAsync(userId);
            var allSeries = await _seriesService.GetAllSeriesOfUserAsync(userId);

            if (allCubes != null && allSeries != null)
            {
                foreach (var cube in allCubes)
                {
                    var options = allSeries
                        .Where(s => s.Cube == cube)
                        .Select(s => s.SerieOption)
                        .Distinct()
                        .ToList();

                    var singleCubeBestTimes = new BestTimesCollectionViewModel()
                    {
                        Description = string.Format("{0} {1} {2}", cube.Manufacturer.Name, cube.ModelName, cube.PlasticColor.Name),
                    };

                    foreach (var option in options)
                    {
                        var serie = allSeries
                            .Where(s => s.Cube == cube && s.SerieOption == option)
                            .OrderBy(s => s.ShortestResult)
                            .ToList()
                            .First();

                        var solve = (await _seriesService.GetAllSolvesOfSerieAsync(serie.Identity))
                            .OrderBy(s => s.Duration)
                            .FirstOrDefault();

                        if (solve != null)
                        {
                            singleCubeBestTimes.Times.Add(new BestTimeViewModel()
                            {
                                SerieID = serie.Identity,
                                Duration = solve.Duration,
                                TimeStamp = solve.FinishTimeSpan,
                                CategoryOption = option.Name,
                                DNF = solve.DNF,
                                Penalty = solve.PenaltyTwoSeconds,
                            });
                        }
                    }

                    model.TimeCollection.Add(singleCubeBestTimes);
                }

                model.TimeCollection
                    .OrderBy(ct => ct.Description);

                foreach (var times in model.TimeCollection)
                {
                    times.Times = times.Times.OrderBy(t => t.Duration).ToList();
                }
            }
        }

        private SolvesTrendViewModel CreateCategoryTrendStartModel(UserModel user, SolvesTrendViewModel model)
        {
            model.UserId = user.Id;
            model.UserName = user.UserName;
            model.TrendDescription = ApplicationResources.UserInterface.Common.LastSeriesTrend;

            return model;
        }

        private void CreateTredDescription(UpdateTrendViewModel model, DateTime start, DateTime stop,
            Category cat, Cube cub, CategoryOption option, int limit)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Chart: ");
            sb.Append("From: " + start.ToString() + " ");
            sb.Append("To: " + stop.ToString() + ", ");
            if (cat != null || cub != null || option != null)
            {
                sb.Append("For: ");
                if (cat != null) sb.Append("category " + cat.Name + ", ");
                if (cub != null) sb.Append("cube: " + cub.Manufacturer.Name + " " + cub.ModelName + " " + cub.PlasticColor.Name + ", ");
                if (option != null) sb.Append("option: " + option.Name + ", ");
            }
            if (limit > 0)
            {
                sb.Append("Last " + limit + " series, ");
            }

            model.TrendDescription = sb.ToString()[0..^2];
        }

        private (DateTime, DateTime) ProcessStartAndEndDate(string startDate, string endDate)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            string format = "d";
            DateTime str = startDate != null ? DateTime.ParseExact(startDate, format, provider) : new DateTime(1, 1, 1, 0, 0, 0);
            DateTime start = new DateTime(str.Year, str.Month, str.Day, 0, 0, 0);
            DateTime sto = endDate != null ? DateTime.ParseExact(endDate, format, provider) : new DateTime(3000, 1, 1, 0, 0, 0);
            DateTime stop = new DateTime(sto.Year, sto.Month, sto.Day, 23, 59, 59);

            return (start, stop);
        }
    }
}
