using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Results.UserInterface;
using System;
using System.Linq;
using System.Threading.Tasks;
using TimerDataBase.Abstractions;
using WebRubiksCubeTimer.Extensions;
using WebRubiksCubeTimer.ViewModels.CubeManagement;

namespace WebRubiksCubeTimer.Controllers
{
    [Authorize(Roles = ("Moderator"))]
    public class CubesController : Controller
    {
        private readonly ICubeService _cubeSrvice;
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _configuration;

        public CubesController(ICubeService cubeService, ICategoryService categoryService,
            IConfiguration configuration)
        {
            _cubeSrvice = cubeService;
            _categoryService = categoryService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Summary(int pageNumber = 1)
        {
            int pageSize = Convert.ToInt32(_configuration["Paginations:TotalCubesListPagination"]);

            return View(new CubesPageViewModel()
            {
                PlasticColors = await _cubeSrvice.GetAllPlasticColorsAsync(),
                Manufacturers = await _cubeSrvice.GetAllManufacturersAsync(),
                Cubes = (await _cubeSrvice.GetAllCubesAsync())
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize),
            });
        }

        public async Task<JsonResult> SummaryPagination(int currentPageNumber = 1)
        {
            int pageSize = Convert.ToInt32(_configuration["Paginations:TotalCubesListPagination"]);
            long cubesAmount = (await _cubeSrvice.GetAllCubesAsync()).Count();

            return Json(this.GetPaginationData(cubesAmount, pageSize, currentPageNumber));
        }

        [HttpPost]
        public async Task<IActionResult> AddCubePlasticColor([FromBody] string colorName)
        {
            var result = await _cubeSrvice.AddPlasticColorAsync(colorName);
            if (!result.IsFaulted)
            {
                result.JsonData = await _cubeSrvice.GetSinglePlasticColorAsync(colorName);
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCubePlasticColor([FromBody] string colorName)
        {
            var result = await _cubeSrvice.DeletePlasticColorAsync(colorName);
            if (!result.IsFaulted)
            {
                result.JsonData = await _cubeSrvice.GetAllPlasticColorsAsync();
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddManufacturer([FromBody] AddManufacturerViewModel model)
        {
            if (model == null)
            {
                return Json(new Result(ApplicationResources.UserInterface.Common.AddManufacturerError));
            }

            var result = await _cubeSrvice.AddNewManufacturerAsync(model.Name, model.Country, Convert.ToUInt16(model.Year));
            if (!result.IsFaulted)
            {
                result.JsonData = await _cubeSrvice.GetManufacturerAsync(model.Name);
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteManufacturer([FromBody] string name)
        {
            var result = await _cubeSrvice.DeleteManufacturerAsync(name);
            if (!result.IsFaulted)
            {
                result.JsonData = await _cubeSrvice.GetAllManufacturersAsync();
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> ModifyManufacturer([FromBody] EditManufacturerViewModel model)
        {
            if (model == null)
            {
                return Json(new Result(ApplicationResources.UserInterface.Common.ModifyManufacturerError));
            }

            var result = await _cubeSrvice.UpdateManufacturerAsync(Convert.ToInt32(model.Identity),
                model.Name, model.Country, Convert.ToUInt16(model.Year));
            if (!result.IsFaulted)
            {
                result.JsonData = await _cubeSrvice.GetAllManufacturersAsync();
            }

            return Json(result);
        }

        public async Task<IActionResult> AddCube()
        {
            return View(new CrudCubeModel()
            {
                PlasticColors = await _cubeSrvice.GetAllPlasticColorsAsync(),
                Manufacturers = await _cubeSrvice.GetAllManufacturersAsync(),
                Categories = await _categoryService.GetAllCategoriesAsync(),
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddCube(CrudCubeModel model)
        {
            if (ModelState.IsValid)
            {
                var manufacturer = await _cubeSrvice.GetManufacturerAsync(model.ManufacturerID);
                var plasticColor = (await _cubeSrvice.GetAllPlasticColorsAsync()).FirstOrDefault(pc => pc.Identity == model.PlasticColorID);
                var category = await _categoryService.GetCategoryAsync(model.CategoryID);

                if (manufacturer != null && plasticColor != null && category != null)
                {
                    var result = await _cubeSrvice.AddNewCubeAsync(category, plasticColor, manufacturer,
                    model.ModelName, model.ReleaseYear, model.WcaPermission);

                    if (result.IsFaulted)
                    {
                        foreach (var message in result.Messages)
                        {
                            ModelState.AddModelError(string.Empty, message);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Summary");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ApplicationResources.UserInterface.ModelsValidationMessages.InvalidParameters);
                }
            }

            model.PlasticColors = await _cubeSrvice.GetAllPlasticColorsAsync();
            model.Manufacturers = await _cubeSrvice.GetAllManufacturersAsync();
            model.Categories = await _categoryService.GetAllCategoriesAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCube(long identity)
        {
            return Json(await _cubeSrvice.DeleteCubeAsync(identity));
        }

        public async Task<IActionResult> ModifyCube(long identity)
        {
            var cube = await _cubeSrvice.GetCubeAsync(identity);
            if (cube != null)
            {
                var model = new CrudCubeModel
                {
                    PlasticColors = await _cubeSrvice.GetAllPlasticColorsAsync(),
                    Manufacturers = await _cubeSrvice.GetAllManufacturersAsync(),
                    Categories = await _categoryService.GetAllCategoriesAsync(),

                    CubeID = cube.Identity,
                    CategoryID = cube.Category.Identity,
                    ManufacturerID = cube.Manufacturer.Identity,
                    PlasticColorID = cube.PlasticColor.Identity,
                    ModelName = cube.ModelName,
                    ReleaseYear = cube.ReleaseYear,
                    WcaPermission = cube.WcaPermission
                };

                return View(model);
            }

            return RedirectToAction("Summary");
        }

        [HttpPost]
        public async Task<IActionResult> ModifyCube(CrudCubeModel model)
        {
            if (ModelState.IsValid)
            {
                var manufacturer = await _cubeSrvice.GetManufacturerAsync(model.ManufacturerID);
                var plasticColor = (await _cubeSrvice.GetAllPlasticColorsAsync()).FirstOrDefault(pc => pc.Identity == model.PlasticColorID);
                var category = await _categoryService.GetCategoryAsync(model.CategoryID);

                if (manufacturer != null && plasticColor != null && category != null)
                {
                    var result = await _cubeSrvice.UpdateCubeAsync(model.CubeID, category, plasticColor,
                        manufacturer, model.ModelName, model.ReleaseYear, model.WcaPermission);

                    if (result.IsFaulted)
                    {
                        foreach (var message in result.Messages)
                        {
                            ModelState.AddModelError(string.Empty, message);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Summary");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ApplicationResources.UserInterface.ModelsValidationMessages.InvalidParameters);
                }
            }

            model.PlasticColors = await _cubeSrvice.GetAllPlasticColorsAsync();
            model.Manufacturers = await _cubeSrvice.GetAllManufacturersAsync();
            model.Categories = await _categoryService.GetAllCategoriesAsync();

            return View(model);
        }
    }
}
