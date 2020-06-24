using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Results.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimerDataBase.Abstractions;
using TimerDataBase.TableModels;
using WebRubiksCubeTimer.ViewModels.Categories;
using WebRubiksCubeTimer.ViewModels.CategoriesManagement;

namespace WebRubiksCubeTimer.Controllers
{
    [Authorize(Roles = "Administrator, Moderator")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IScrambleService _scrambleService;
        private readonly IConfiguration _configuration;

        public CategoriesController(
            ICategoryService categoryService,
            IScrambleService scrambleService,
            IConfiguration configuration)
        {
            _categoryService = categoryService;
            _scrambleService = scrambleService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            return View(new CategoriesPageViewModel()
            {
                CategoryOptions = await _categoryService.GetAllCategoryOptionsAsync(),
                Categories = await _categoryService.GetAllCategoriesAsync(),
                OptionsModifyPermission = User?.IsInRole("Administrator") == true,
            });
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddCategoryOption([FromBody] string name)
        {
            return Json(await _categoryService.AddCategoryOptionAsync(name));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCategoryOption([FromBody] string name)
        {
            return Json(await _categoryService.DeleteCategoryOptionAsync(name));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateCategoryOption(int identity, string name)
        {
            return Json(await _categoryService.UpdateCategoryOptionAsync(identity, name));
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(string name, IEnumerable<string> options, string minimalDuration)
        {
            List<CategoryOption> optionsList = new List<CategoryOption>();
            foreach (var option in options)
            {
                var opt = await _categoryService.GetSingleCategoryOptionAsync(option);
                if (opt != null)
                {
                    optionsList.Add(opt);
                }
                else
                {
                    return Json(new Result(string.Format(ApplicationResources.TimerDB.Messages.CategoryOptionNotExist, option)));
                }
            }

            return Json(await _categoryService.AddCategoryAsync(name, optionsList, ParseTimeSpanFromString(minimalDuration)));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int identity)
        {
            var category = await _categoryService.GetCategoryAsync(identity);
            var result = await _categoryService.DeleteCategoryAsync(category.Identity);
            if (result.IsFaulted)
            {
                return Json(result);
            }
            else
            {
                var scramble = await _scrambleService.GetDefaultCompleteScrambleAsync(category);
                if (scramble?.Scramble?.Identity > 0)
                {
                    return Json(await _scrambleService.DeleteScrambleDefinitionAsync(scramble.Scramble.Identity));
                }

                return Json(new Result(string.Empty, false));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryOptions(int identity)
        {
            return Json(((await _categoryService
                .GetCategoryAsync(identity))
                .OptionsSet.Select(o => o.Name)
                .ToList()));
        }

        [HttpPost]
        public async Task<IActionResult> ModifyCategory(int identity, string newName, IEnumerable<string> options, string minimalDuration)
        {
            IList<CategoryOption> optionsList = new List<CategoryOption>();

            foreach (var opt in options)
            {
                optionsList.Add((await _categoryService.GetAllCategoryOptionsAsync()).FirstOrDefault(o => o.Name == opt));
            }

            return Json(await _categoryService.UpdateCategoryAsync(identity, newName, optionsList, ParseTimeSpanFromString(minimalDuration)));
        }

        public async Task<IActionResult> Scramble(int id)
        {
            Category category;

            if (id > 0)
            {
                category = await _categoryService.GetCategoryAsync(id);
            }
            else
            {
                category = (await _categoryService.GetAllCategoriesAsync()).FirstOrDefault();
            }

            if (category?.Identity > 0)
            {
                return View(await CreateScrambleManagerViewModelAsync(category));
            }

            return View(new ScrambleManagerViewModel());
        }

        public async Task<IActionResult> AddScramble(int id)
        {
            var category = await _categoryService.GetCategoryAsync(id);
            
            if (category == null)
            {
                return RedirectToAction(nameof(this.Scramble));
            }

            return await Task.FromResult(View("UpdateScramble", CreateScrambleUpdateModel(category)));
        }

        [HttpPost]
        public async Task<IActionResult> AddScramble(ScrambleUpdateViewModel model)
        {
            if(ModelState.IsValid)
            {
                var addingResult = await _scrambleService.AddScrambleDefinitionAsync(
                    model.CategoryId, model.MinimumScrambleLength, model.MaximumScrambleLength,
                    model.DefaultScrambleLength, model.EliminateDuplicates, model.AllowRegenerate,
                    model.Disabled, model.TopColor, model.FrontColor, model.ScrambleName);

                if(addingResult.IsFaulted)
                {
                    foreach (var errorMessage in addingResult.Messages)
                    {
                        ModelState.AddModelError(string.Empty, errorMessage);
                    }
                }
                else
                {
                    var category = await _categoryService.GetCategoryAsync(model.CategoryId);
                    return RedirectToAction("Scramble", new { id = category.Identity });
                }
            }

            return View("UpdateScramble", model);
        }

        public async Task<IActionResult> ModifyScramble(long id)
        {
            var fullScramble = await _scrambleService.GetCompleteScrambleAsync(id);
            var category = await _categoryService.GetCategoryAsync(fullScramble.Scramble.Category.Identity);

            if (category == null)
            {
                return RedirectToAction(nameof(this.Scramble));
            }

            if (category != null && fullScramble == null)
            {
                return RedirectToAction(nameof(this.Scramble), new { id = category.Identity });
            }

            return await Task.FromResult(View("UpdateScramble", CreateScrambleUpdateModel(category, fullScramble.Scramble)));
        }

        [HttpPost]
        public async Task<IActionResult> ModifyScramble(ScrambleUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var modificationResult = await _scrambleService.UpdateScrambleDefinitionAsync(
                    model.Identity, model.MinimumScrambleLength, model.MaximumScrambleLength,
                    model.DefaultScrambleLength, model.EliminateDuplicates, model.AllowRegenerate,
                    model.Disabled, model.TopColor, model.FrontColor, model.ScrambleName);

                if (modificationResult.IsFaulted)
                {
                    foreach (var errorMessage in modificationResult.Messages)
                    {
                        ModelState.AddModelError(string.Empty, errorMessage);
                    }
                }
                else
                {
                    var category = await _categoryService.GetCategoryAsync(model.CategoryId);
                    return RedirectToAction("Scramble", new { id = category.Identity });
                }
            }

            return View("UpdateScramble", model);
        }

        [HttpPost]
        public async Task<JsonResult> SetScrambleAsDefault(int categoryId, long scrambleId)
        {
            var setResult = await _scrambleService.SetScrambleAsDefaultAsync(categoryId, scrambleId);
            return Json(new Result(setResult.Messages.FirstOrDefault(), setResult.IsFaulted)
            {
                JsonData = !setResult.IsFaulted,
            });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteScrambleDefinition(long scrambleId)
        {
            var deletionResult = await _scrambleService.DeleteScrambleDefinitionAsync(scrambleId);
            return Json(new Result(deletionResult.Messages.FirstOrDefault(), deletionResult.IsFaulted)
            {
                JsonData = !deletionResult.IsFaulted,
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetScrambleDefinition(long scrambleId)
        {
            var completeDefinition = await _scrambleService.GetCompleteScrambleAsync(scrambleId);

            if (completeDefinition?.Scramble?.Identity > 0)
            {
                return Json(new Result(string.Empty, false)
                {
                    JsonData = completeDefinition,
                });
            }

            return Json(new Result(ApplicationResources.TimerDB.Messages.ScrambleDoesNotExist));
        }

        [HttpPost]
        public async Task<JsonResult> ModifyMoves(long scrambleId, string moves)
        {
            if (string.IsNullOrEmpty(moves))
            {
                return Json(new Result(ApplicationResources.TimerDB.Messages.ScrambleNoMove)
                {
                    JsonData = ApplicationResources.TimerDB.Messages.ScrambleNoMove
                });
            }

            var listOfMoves = moves.Split(' ').ToList();

            var modifyResult = await _scrambleService.UpdateScrambleMovesAsync(scrambleId, listOfMoves);

            return Json(new Result(modifyResult.Messages.FirstOrDefault(), modifyResult.IsFaulted)
            {
                JsonData = modifyResult.Messages.FirstOrDefault(),
            });
        }

        private async Task<ScrambleManagerViewModel> CreateScrambleManagerViewModelAsync(Category category)
        {
            return new ScrambleManagerViewModel()
            {
                CategoryName = category == null ? string.Empty : category.Name,
                CategoryId = category == null ? 0 : category.Identity,
                Scrambles = category == null ? new List<Scramble>() :
                    await _scrambleService.GetAllDefinedScramblesForCategoryAsync(category.Identity),
                AllCategories = await _categoryService.GetAllCategoriesAsync(),
                CategoriesWithNoScramble = await _scrambleService.GetCategoriesWithoutScrambleAsync(),
            };
        }

        private ScrambleUpdateViewModel CreateScrambleUpdateModel(Category category, Scramble scramble = null)
        {
            if (scramble != null)
            {
                return new ScrambleUpdateViewModel()
                {
                    IsModification = true,
                    Identity = scramble.Identity,
                    CategoryId = category.Identity,
                    CategoryName = category.Name,
                    ScrambleName = scramble.Name,
                    DefaultScrambleLength = scramble.DefaultScrambleLength,
                    MinimumScrambleLength = scramble.MinimumScrambleLength,
                    MaximumScrambleLength = scramble.MaximumScrambleLength,
                    EliminateDuplicates = scramble.EliminateDuplicates,
                    AllowRegenerate = scramble.AllowRegenerate,
                    Disabled = scramble.Disabled,
                    TopColor = scramble.TopColor,
                    FrontColor = scramble.FrontColor,
                };
            }

            return new ScrambleUpdateViewModel()
            {
                IsModification = false,
                Identity = 0,
                CategoryId = category.Identity,
                CategoryName = category.Name,
                ScrambleName = _configuration["ScrambleDefinitionDefaults:DefaultName"],
                DefaultScrambleLength = Convert.ToInt32(_configuration["ScrambleDefinitionDefaults:DefaultLength"]),
                MinimumScrambleLength = Convert.ToInt32(_configuration["ScrambleDefinitionDefaults:MinLength"]),
                MaximumScrambleLength = Convert.ToInt32(_configuration["ScrambleDefinitionDefaults:MaxLength"]),
                EliminateDuplicates = Convert.ToBoolean(_configuration["ScrambleDefinitionDefaults:EliminateDuplicates"]),
                AllowRegenerate = Convert.ToBoolean(_configuration["ScrambleDefinitionDefaults:AllowRegenerate"]),
                Disabled = Convert.ToBoolean(_configuration["ScrambleDefinitionDefaults:Disabled"]),
                TopColor = _configuration["ScrambleDefinitionDefaults:TopColor"],
                FrontColor = _configuration["ScrambleDefinitionDefaults:FrontColor"],
            };
        }

        private TimeSpan ParseTimeSpanFromString(string timeSpan)
        {
            var spanParsed = TimeSpan.TryParse(timeSpan, out TimeSpan span);
            if (!spanParsed)
            {
                span = new TimeSpan(0, 0, 0, 0, 0);
            }

            return span;
        }
    }
}
