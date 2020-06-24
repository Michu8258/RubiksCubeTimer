using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Results.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimerDataBase.Abstractions;
using TimerDataBase.TableModels;
using WebRubiksCubeTimer.Extensions;
using WebRubiksCubeTimer.Models.Users;
using WebRubiksCubeTimer.ViewModels.User;

namespace WebRubiksCubeTimer.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly ICubeCollectionService _cubeCollectionsService;
        private readonly ICubeService _cubeService;
        private readonly IConfiguration _configuration;
        private readonly ICategoryService _categoryService;
        private readonly ISeriesService _seriesService;
        private readonly IPasswordValidator<UserModel> _passwordValidator;
        private readonly IPasswordHasher<UserModel> _passwordHasher;

        public UserController(
            UserManager<UserModel> userManager,
            ICubeCollectionService cubeCollections,
            ICubeService cubeService,
            IConfiguration configuration,
            ICategoryService categoryService,
            ISeriesService seriesService,
            IPasswordValidator<UserModel> passwordValidator,
            IPasswordHasher<UserModel> passwordHasher)
        {
            _userManager = userManager;
            _cubeCollectionsService = cubeCollections;
            _cubeService = cubeService;
            _configuration = configuration;
            _categoryService = categoryService;
            _seriesService = seriesService;
            _passwordValidator = passwordValidator;
            _passwordHasher = passwordHasher;
        }

        public async Task<IActionResult> ShowMyAccount(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    return RedirectToAction("Account", "User", new { id = user.Id });
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Account(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("StatusCodePage", "Error", new { code = 404 });
            }
            else if (user != null && ((user.UserName == User.Identity.Name) || User.IsInRole("Administrator")))
            {
                var model = new UserMainViewModel()
                {
                    UserId = user.Id,
                };

                await CollectUserRakings(user, model);

                return View(model);
            }
            else
            {
                var properUser = await _userManager.FindByNameAsync(User.Identity.Name);
                return RedirectToAction("AccesDenied", "Login", new { returnUrl = "/User/Account/" + properUser.Id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetChartsData(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var cubes = await _cubeCollectionsService.GetAllCubesOfUserAsync(id);
                var series = await _seriesService.GetAllSeriesOfUserAsync(user.Id);
                var model = new UserChartsDataViewModel();

                var categories = cubes
                    .Select(c => c.Category)
                    .Distinct()
                    .ToList();

                if (cubes.Count() > 0)
                {
                    foreach (var cube in cubes)
                    {
                        var amount = series.Count(s => s.Cube == cube);

                        model.CubeChartData.Add(new UserChartsDataViewModel.CubeChartItem()
                        {
                            Cube = cube,
                            Amount = amount,
                        });
                    }

                    model.CubeChartData = model.CubeChartData
                        .OrderByDescending(d => d.Amount)
                        .ToList();
                }

                if (categories.Count() > 0)
                {
                    foreach (var category in categories)
                    {
                        var amount = series
                            .Count(s => s.Cube.Category == category);

                        model.CategoryChartData.Add(new UserChartsDataViewModel.CategoryChartItem()
                        {
                            Category = category,
                            Amount = amount,
                        });
                    }

                    model.CategoryChartData = model.CategoryChartData
                        .OrderByDescending(d => d.Amount)
                        .ToList();
                }

                return Json(new Result(string.Empty, false)
                {
                    JsonData = model,
                });
            }

            return Json(new Result(ApplicationResources.UserInterface.Common.UserDoNotExist));
        }

        public async Task<IActionResult> ShowMyCubesCollection(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    return RedirectToAction("CubesCollection", "User", new { userId = user.Id, pageNumber = 1 });
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CubesCollection(string userId, int pageNumber = 1)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("StatusCodePage", "Error", new { code = 404 });
            }
            else if(user != null && ((user.UserName == User.Identity.Name) || User.IsInRole("Administrator")))
            {
                int pageSize = Convert.ToInt32(_configuration["Paginations:UserCubesCollectionPagination"]);

                return View(new CubesCollectionViewModel()
                {
                    UserCubes = (await CollectUserCubesData(user.Id))
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize),
                    AvailableCategories = (await CollectCubesUserDoNotHave(user.Id))
                            .Select(c => c.Category)
                            .Distinct()
                            .OrderBy(c => c.Name)
                            .ToList(),
                    UserId = user.Id,
                    UserName = user.UserName,
                });
            }
            else
            {
                return RedirectToAction("AccesDenied", "Login", new { returnUrl = "/User/MyCubesCollection/" + User?.Identity?.Name });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableToAddManufacturers(int categoryId, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new Result(ApplicationResources.UserInterface.Common.UserDoNotExist));
            }

            var manufacturers = (await CollectCubesUserDoNotHave(user.Id))
                .Where(c => c.Category.Identity == categoryId)
                .Select(c => c.Manufacturer)
                .Distinct()
                .OrderBy(m => m.Name)
                .ToList();

            return Json(new Result(string.Empty, false)
            {
                JsonData = manufacturers
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableToAddCubes(int categoryId, int manufacturerId, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new Result(ApplicationResources.UserInterface.Common.UserDoNotExist));
            }

            var cubes = (await CollectCubesUserDoNotHave(user.Id))
                .Where(c => c.Category.Identity == categoryId && c.Manufacturer.Identity == manufacturerId)
                .Distinct()
                .OrderBy(c => c.ModelName)
                .ToList();

            return Json(new Result(string.Empty, false)
            {
                JsonData = cubes
            });
        }

        public async Task<JsonResult> CubesCollectionPagination(int currentPageNumber = 1)
        {
            int pageSize = Convert.ToInt32(_configuration["Paginations:UserCubesCollectionPagination"]);
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            long cubesAmount = (await CollectUserCubesData(user?.Id)).Count();

            return Json(this.GetPaginationData(cubesAmount, pageSize, currentPageNumber));
        }

        public async Task<IActionResult> AddCubeTouserCollection(string userId, long cubeId)
        {
            var cube = await _cubeService.GetCubeAsync(cubeId);
            var user = await _userManager.FindByIdAsync(userId);
            if (cube != null && user != null)
            {
                var result = await _cubeCollectionsService.AddCubeToCollectionAsync(userId, cube);
                if (result.IsFaulted)
                {
                    ModelState.AddModelError(string.Empty, result.Messages.FirstOrDefault());
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, ApplicationResources.TimerDB.Messages.NullArgument);
            }

            return RedirectToAction("CubesCollection", "User", new { userId = user?.Id, pageNumber = 1 });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCubeFromCollection(string userId, long cubeId)
        {
            Result result;

            if(string.IsNullOrEmpty(userId) || cubeId < 1)
            {
                result = new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }
            else
            {
                var cube = await _cubeService.GetCubeAsync(cubeId);
                if (cube == null)
                {
                    result = new Result(ApplicationResources.TimerDB.Messages.CubeNotExists);
                }
                else
                {
                    result = await _cubeCollectionsService.DeleteCubeFromCollectionAsync(userId, cube);
                }
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddCubeRatingForUser(string userId, long cubeId, ushort rating)
        {
            return await CubeRatingInternal(userId, cubeId, rating);
        }

        [HttpPost]
        public async Task<IActionResult> ModifyCubeRatingForUser(string userId, long cubeId, ushort newRating)
        {
            return await CubeRatingInternal(userId, cubeId, newRating, false);
        }

        [HttpGet]
        public async Task<IActionResult> GetCubeInfo(long cubeId)
        {
            CubeInfoViewModel model = new CubeInfoViewModel();
            Result result;

            var cube = await _cubeService.GetCubeAsync(cubeId);
            try
            {
                model.ManufacturerCountry = cube.Manufacturer.Country;
                model.ManufacturerFoundationYear = cube.Manufacturer.FoundationYear;
                model.UsersUsingThisCube = await _cubeCollectionsService.CountUsersWithCubeAsync(cube);
                model.PermittedCategoryOptions = (await _categoryService.GetOptionsForCubeAsync(cube)).Select(o => o.Name);
                model.ModelError = false;

                result = new Result(string.Empty, false)
                {
                    JsonData = model,
                };
            }
            catch
            {
                model.ModelError = true;

                result = new Result("Error")
                {
                    JsonData = model,
                };
            }

            return Json(result);
        }

        public async Task<IActionResult> CreateSeries(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                var model = new CreateNewSeriesViewModel()
                {
                    UserIdentity = user.Id,
                    SeriesId = 0,
                    SelectedCubeId = 0,
                    CategoryOptionId = 0,
                    AvailableCategories = (await _cubeCollectionsService.GetAllCubesOfUserAsync(user.Id))
                        .Select(c => c.Category)
                        .Distinct()
                        .OrderBy(c => c.Name)
                        .ToList(),
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetManufacturersOfUserCubes(string userId, int categoryId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new Result(ApplicationResources.UserInterface.Common.UserDoNotExist));
            }

            var manufacturers = (await _cubeCollectionsService.GetAllCubesOfUserAsync(user.Id))
                .Where(c => c.Category.Identity == categoryId)
                .Select(c => c.Manufacturer)
                .Distinct()
                .OrderBy(m => m.Name)
                .ToList();

            return Json(new Result(string.Empty, false)
            {
                JsonData = manufacturers
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCubesOfUserCubes(string userId, int categoryId, int manufacturerId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new Result(ApplicationResources.UserInterface.Common.UserDoNotExist));
            }

            var cubes = (await _cubeCollectionsService.GetAllCubesOfUserAsync(user.Id))
                .Where(c => c.Category.Identity == categoryId && c.Manufacturer.Identity == manufacturerId)
                .Distinct()
                .OrderBy(m => m.ModelName)
                .ToList();

            return Json(new Result(string.Empty, false)
            {
                JsonData = cubes
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryOptionsForCube(string userId, int categoryId, int manufacturerId, long cubeId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new Result(ApplicationResources.UserInterface.Common.UserDoNotExist));
            }

            var cube = (await _cubeCollectionsService.GetAllCubesOfUserAsync(user.Id))
                .FirstOrDefault(c => c.Category.Identity == categoryId && c.Manufacturer.Identity == manufacturerId && c.Identity == cubeId);

            var options = (await _categoryService.GetOptionsForCubeAsync(cube))
                .OrderBy(o => o.Name)
                .ToList();

            return Json(new Result(string.Empty, false)
            {
                JsonData = options
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSeries(CreateNewSeriesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var cube = await _cubeService.GetCubeAsync(model.SelectedCubeId);
                var option = await _categoryService.GetSingleCategoryOptionAsync(model.CategoryOptionId);
                var result = await _seriesService.CheckIfSerieCanBeCreatedAsync(model.UserIdentity, cube, option);

                if (result.IsFaulted)
                {
                    foreach (var error in result.Messages)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                else
                {
                    return RedirectToAction("Series", "Timer", new { userId = model.UserIdentity,
                        serieId = model.SeriesId, cubeId = model.SelectedCubeId, categoryOptionId = model.CategoryOptionId });
                }
            }

            model.AvailableCategories = (await _cubeCollectionsService.GetAllCubesOfUserAsync(model.UserIdentity))
                .Select(c => c.Category)
                .Distinct()
                .OrderBy(c => c.Name)
                .ToList();

            return await Task.FromResult(View(model));
        }

        public async Task<IActionResult> ModifyAccount()
        {
            var userName = User?.Identity?.Name;
            if(! string.IsNullOrEmpty(userName))
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    var model = new UserSelfModifViewModel()
                    {
                        UserIdentity = user.Id,
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                    };

                    return View(model);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ModifyAccount(UserSelfModifViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserIdentity);
                if (user != null)
                {
                    var passwordOK = true;
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        passwordOK = await _userManager.CheckPasswordAsync(user, model.Password);
                    }

                    if (passwordOK)
                    {
                        if ((model.NewPassword == model.NewPassword2) || (string.IsNullOrEmpty(model.NewPassword) && string.IsNullOrEmpty(model.NewPassword2)))
                        {
                            IdentityResult validPassword = null;

                            if (!string.IsNullOrEmpty(model.NewPassword))
                            {
                                validPassword = await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                                if (validPassword.Succeeded)
                                {
                                    user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                                }
                                else
                                {
                                    this.AddErrorsFromResult(validPassword);
                                }
                            }

                            if (validPassword == null || validPassword.Succeeded)
                            {
                                if (!string.IsNullOrEmpty(model.UserName)) user.UserName = model.UserName;
                                if (!string.IsNullOrEmpty(model.PhoneNumber)) user.PhoneNumber = Convert.ToInt32(model.PhoneNumber).ToString();

                                IdentityResult result = await _userManager.UpdateAsync(user);
                                if (result.Succeeded)
                                {
                                    return RedirectToAction("Logout", "Login", new { id = user.Id });
                                }
                                else
                                {
                                    this.AddErrorsFromResult(result);
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", ApplicationResources.UserInterface.Common.NewPasswordsNotEqual);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", ApplicationResources.UserInterface.Common.WrongCurrentPassword);
                    }
                }
                else
                {
                    ModelState.AddModelError("", ApplicationResources.UserInterface.Common.UserDoNotExist);
                }
            }

            return View(model);
        }

        private async Task<IActionResult> CubeRatingInternal(string userId, long cubeId, ushort rating, bool add = true)
        {
            Result result;

            if (string.IsNullOrEmpty(userId) || cubeId < 1 || rating < 1)
            {
                result = new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }
            else
            {
                var cube = await _cubeService.GetCubeAsync(cubeId);
                if (cube == null)
                {
                    result = new Result(ApplicationResources.TimerDB.Messages.CubeNotExists);
                }
                else
                {
                    if (add)
                    {
                        result = await _cubeService.AddCubeRatingAsync(cube, userId, rating);
                    }
                    else
                    {
                        result = await _cubeService.UpdateCubeRatingAsync(cube, userId, rating);
                    }
                }
            }

            return Json(result);
        }

        private async Task<IEnumerable<Cube>> CollectCubesUserDoNotHave(string userId)
        {
            var output = new List<Cube>();
            var userCubes = await _cubeCollectionsService.GetAllCubesOfUserAsync(userId);
            var allCubes = await _cubeService.GetAllCubesAsync();

            foreach (var cube in allCubes)
            {
                if (! userCubes.Contains(cube))
                {
                    output.Add(cube);
                }
            }

            return output;
        }

        private async Task<IEnumerable<UserCubeViewModel>> CollectUserCubesData(string userId)
        {
            var output = new List<UserCubeViewModel>();
            var cubes = await _cubeCollectionsService.GetAllCubesOfUserAsync(userId);

            foreach (var cube in cubes)
            {
                output.Add(new UserCubeViewModel()
                {
                    Identity = cube.Identity,
                    Category = cube.Category.Name,
                    Manufacturer = cube.Manufacturer.Name,
                    ModelName = cube.ModelName,
                    PlasticColor = cube.PlasticColor.Name,
                    ReleaseYear = cube.ReleaseYear,
                    TotalRating = cube.Rating,
                    UserRate = GetCubeRateOfUser(cube, userId),
                    WcaPermission = cube.WcaPermission,
                });
            }

            return output;
        }

        private ushort GetCubeRateOfUser(Cube cube, string userId)
        {
            var rates = _cubeService.GetRatingsForCubesOfUser(new List<Cube>() { cube }, userId);

            if (rates.Count() == 1)
            {
                return rates.First().RateValue;
            }

            return 0;
        }

        private async Task CollectUserRakings(UserModel user, UserMainViewModel model)
        {
            var cubes = await _cubeCollectionsService.GetAllCubesOfUserAsync(user.Id);
            var categories = cubes
                .Select(c => c.Category)
                .Distinct()
                .ToList();

            foreach (var category in categories)
            {
                model.Categoreis.Add(new UserMainViewModel.CategoriesTableItem()
                {
                    Category = category,
                    Position = await _seriesService.GetUserRankedPositionCategoryAsync(user.Id, category),
                    SolvesAmount = await _seriesService.GetAmountOfSolvesForUserAndCategoryAsync(user.Id, category),
                });
            }

            foreach (var cube in cubes)
            {
                model.Cubes.Add(new UserMainViewModel.CubesTableItem()
                {
                    Cube = cube,
                    Position = await _seriesService.GetUserRankedPositionCubeAsync(user.Id, cube),
                    SolvesAmount = await _seriesService.GetAmountOfSolvesForUserAndCubeAsync(user.Id, cube),
                });
            }

            model.Categoreis = model.Categoreis
                .OrderBy(c => c.Category.Name)
                .ToList();

            model.Cubes = model.Cubes
                .OrderBy(c => c.Cube.Manufacturer.Name)
                .ThenBy(c => c.Cube.ModelName)
                .ThenBy(c => c.Cube.PlasticColor.Name)
                .ToList();
        }
    }
}