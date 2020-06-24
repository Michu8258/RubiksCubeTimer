using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Results.UserInterface;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TimerDataBase.Abstractions;
using TimerDataBase.TableModels;
using WebRubiksCubeTimer.Models.Users;
using WebRubiksCubeTimer.ViewModels.Timer;

namespace WebRubiksCubeTimer.Controllers
{
    public class TimerController : Controller
    {
        private readonly ISeriesService _seriesService;
        private readonly UserManager<UserModel> _userManager;
        private readonly ICubeService _cubeService;
        private readonly ICategoryService _categoryService;
        private readonly IScrambleService _scrambleService;

        public TimerController(ISeriesService seriesService,
            UserManager<UserModel> userManager,
            ICubeService cubeService,
            ICategoryService categoryService,
            IScrambleService scrambleService)
        {
            _seriesService = seriesService;
            _userManager = userManager;
            _cubeService = cubeService;
            _categoryService = categoryService;
            _scrambleService = scrambleService;
        }

        public async Task<IActionResult> Series(string userId, long serieId,
            long cubeId, int categoryOptionId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var cube = await _cubeService.GetCubeAsync(cubeId);
            var categoryOption = await _categoryService.GetSingleCategoryOptionAsync(categoryOptionId);

            if (user != null && cube != null && categoryOption != null)
            {
                var model = new TimerViewModel();
                await CreateSeriesModel(model, cube, user, categoryOption, serieId);

                return View(model);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> AddSolve(string userId, long categoryId, long cubeId, int optionId,
            bool seriesDone, long miliseconds, bool dnf, bool penalty, long seriesId, string timeStamp, string scramble)
        {
            Result result;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(timeStamp) || categoryId < 1 ||
                cubeId < 1 || optionId < 1 || miliseconds < 0 || seriesId < 0)
            {
                result = new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }
            else
            {
                var user = await _userManager.FindByIdAsync(userId);
                var cube = await _cubeService.GetCubeAsync(cubeId);
                var option = await _categoryService.GetSingleCategoryOptionAsync(optionId);

                if (user == null || cube == null || option == null)
                {
                    result = new Result(ApplicationResources.TimerDB.Messages.NullArgument);
                }
                else
                {
                    DateTime parsedTimeStamp;
                    try
                    {
                        parsedTimeStamp = DateTime.ParseExact(timeStamp, "dd-MM-yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        parsedTimeStamp = DateTime.Now;
                    }

                    if (seriesId == 0)
                    {
                        var serieAdded = await _seriesService.CreateNewSeriesAsync(user.Id, cube, option, parsedTimeStamp);
                        if (serieAdded.IsFaulted)
                        {
                            return Json(serieAdded);
                        }

                        seriesId = Convert.ToInt64(serieAdded.JsonData);
                    }

                    var newSolveAdded = await _seriesService.AddSeriesNewSolveAsync(user.Id, seriesId, seriesDone,
                        TimeSpan.FromMilliseconds(miliseconds), dnf, penalty, parsedTimeStamp, scramble);
                    if (newSolveAdded.IsFaulted)
                    {
                        return Json(newSolveAdded);
                    }

                    var serie = await _seriesService.GetSpecificSerieAsync(seriesId);
                    var solves = await _seriesService.GetAllSolvesOfSerieAsync(seriesId);
                    if (serie != null && solves != null && solves.Count() > 0)
                    {
                        var lastSolveID = solves.Max(s => s.Identity);

                        var response = new SerieSolveAddedViewModel()
                        {
                            SeriesID = seriesId,
                            UserID = user.Id,
                            SeriesStartTime = serie.StartTimeStamp.ToString("dd-MM-yyyy HH:mm:ss"),
                            SeriesFinished = serie.SerieFinished,
                            AtLeastOneDNF = serie.AtLeastOneDNF,
                            NewTimeNumber = solves.Count(),
                            NewTimeDurationMS = Convert.ToInt64(solves.FirstOrDefault(s => s.Identity == lastSolveID)?.Duration.TotalMilliseconds),
                            NewTimeSpan = solves.FirstOrDefault(s => s.Identity == lastSolveID).FinishTimeSpan.ToString("dd-MM-yyyy HH:mm:ss"),
                            NewTimeDNF = solves.FirstOrDefault(s => s.Identity == lastSolveID).DNF,
                            NewTimePenalty = solves.FirstOrDefault(s => s.Identity == lastSolveID).PenaltyTwoSeconds,
                            WorstTimeMS = Convert.ToInt64(serie.LongestResult.TotalMilliseconds),
                            BestTimeMS = Convert.ToInt64(serie.ShortestResult.TotalMilliseconds),
                            AverageTimeMS = Convert.ToInt64(serie.AverageTime.TotalMilliseconds),
                            MeanOf3MS = Convert.ToInt64(serie.MeanOf3.TotalMilliseconds),
                            AverageOf5MS = Convert.ToInt64(serie.AverageOf5.TotalMilliseconds),
                        };

                        result = new Result(string.Empty, false)
                        {
                            JsonData = response,
                        };
                    }
                    else
                    {
                        result = new Result(ApplicationResources.UserInterface.Common.ErrorWithSeriesOrSolves);
                    }
                }
            }

            return await Task.FromResult(Json(result));
        }

        [HttpPost]
        public async Task<IActionResult> MarkSeriesAsFinished(long serieId)
        {
            if (serieId > 0)
            {
                _ = await _seriesService.MarkSeriesFinishedAsync(serieId, true);
            }

            return Json(true);
        }

        [HttpGet]
        public async Task<JsonResult> GetScrambles(int categoryId)
        {
            var scrambles = (await _scrambleService.GetAllCompleteScramblesForCategoryAsync(categoryId))
                    .Where(cs => cs.Scramble.Disabled == false && cs.Scramble.MovesAmount > 0)
                    .OrderBy(cs => cs.Scramble.Name)
                    .ToList();

            return Json(scrambles);
        }

        private async Task CreateSeriesModel(TimerViewModel model, Cube cube, UserModel user,
            CategoryOption categoryOption, long serieId)
        {
            model.Username = user.UserName;
            model.UserId = user.Id;
            model.CategoryID = cube.Category.Identity;
            model.CubeID = cube.Identity;
            model.OptionID = categoryOption.Identity;
            model.SeriesId = serieId;
            model.CategoryName = cube.Category.Name;
            model.CategoryOptionName = categoryOption.Name;
            model.CubeDescription = string.Format("{0} {1} {2}", cube.Manufacturer.Name, cube.ModelName, cube.PlasticColor.Name);
            model.CurrentScramble = await _scrambleService.GetDefaultCompleteScrambleAsync(cube.Category);

            if(model.CurrentScramble?.Scramble?.MovesAmount > 0)
            {
                var allScrambles = (await _scrambleService.GetAllDefinedScramblesForCategoryAsync(cube.Category.Identity))
                    .Where(s => s.Disabled == false && s.MovesAmount > 0)
                    .OrderBy(s => s.Name)
                    .ToList();

                foreach (var scramble in allScrambles)
                {
                    model.ScrambleNames.Add(scramble.Identity, scramble.Name);
                }
            }

            var serie = await _seriesService.GetSpecificSerieAsync(serieId);
            var solves = (await _seriesService.GetAllSolvesOfSerieAsync(serieId))
                .OrderBy(s => s.Identity)
                .ToList();

            if (serieId > 0 && serie != null && solves != null)
            {
                model.StartTime = serie.StartTimeStamp;
                model.SerieFinished = serie.SerieFinished;
                model.AtLeastOneDNF = serie.AtLeastOneDNF;
                model.LongestResult = serie.LongestResult;
                model.ShortestResult = serie.ShortestResult;
                model.AverageTime = serie.AverageTime;
                model.MeanOf3 = serie.MeanOf3;
                model.AverageOf5 = serie.AverageOf5;

                foreach (var solve in solves)
                {
                    model.Solves.Add(new TimerViewModel.SingleTimeViewModel()
                    {
                        Number = solves.IndexOf(solve) + 1,
                        Duration = solve.Duration,
                        TimeSpan = solve.FinishTimeSpan,
                        DNF = solve.DNF,
                        Penalty = solve.PenaltyTwoSeconds,
                    });
                }
            }
            else
            {
                model.SerieFinished = false;
                model.AtLeastOneDNF = false;
                model.LongestResult = TimeSpan.FromMilliseconds(0);
                model.ShortestResult = TimeSpan.FromMilliseconds(0);
                model.AverageTime = TimeSpan.FromMilliseconds(0);
                model.MeanOf3 = TimeSpan.FromMilliseconds(0);
                model.AverageOf5 = TimeSpan.FromMilliseconds(0);
            }
        }
    }
}
