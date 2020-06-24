using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Results.UserInterface;
using System.Linq;
using System.Threading.Tasks;
using WebRubiksCubeTimer.Extensions;
using WebRubiksCubeTimer.Models.Roles;
using WebRubiksCubeTimer.Models.Users;

namespace WebRubiksCubeTimer.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RolesAdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManger;
        private readonly UserManager<UserModel> _userManager;

        public RolesAdminController(RoleManager<IdentityRole> roleMgr, UserManager<UserModel> userMgr)
        {
            _roleManger = roleMgr;
            _userManager = userMgr;
        }

        public IActionResult RolesList()
        {
            return View(ModelState.ExtractMessages());
        }

        [HttpGet]
        public JsonResult GetAllRoles()
        {
            return Json(_roleManger.Roles.OrderBy(r => r.Name));
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(AddRoleModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManger.CreateAsync(new IdentityRole(model.Name));
                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole([FromBody] string id)
        {
            IdentityRole role = await _roleManger.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await _roleManger.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    AddErrorsFromResult(result);
                }
            }
            return Json(_roleManger.Roles.OrderBy(r => r.Name));
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        public async Task<JsonResult> GetUserRolesUserInfo(string id)
        {
            UserRolesModel response = new UserRolesModel();
            UserModel user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var roles = _roleManger.Roles.OrderBy(r => r.Name);
                var userRoles = await _userManager.GetRolesAsync(user);
                if (roles.Count() > 0)
                {
                    foreach (var role in roles)
                    {
                        response.Roles.Add(new RoleBelongingModel()
                        {
                            RoleName = role.Name,
                            UserBelongs = userRoles.Contains(role.Name) ? true : false,
                        });
                    }

                    response.ValidUser = true;
                    response.UserId = user.Id;
                    response.UserName = user.UserName;
                }
            }

            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateUserRoles([FromBody] UserRolesModel model)
        {
            var result = new Result();

            if (ModelState.IsValid && !string.IsNullOrEmpty(model?.UserId))
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    try
                    {
                        foreach (var roleModel in model.Roles)
                        {
                            var role = await _roleManger.FindByNameAsync(roleModel.RoleName);
                            if (role != null)
                            {
                                if (roleModel.UserBelongs) await _userManager.AddToRoleAsync(user, role.Name);
                                else await _userManager.RemoveFromRoleAsync(user, role.Name);
                            }
                        }

                        result.AddMessage(string.Format(ApplicationResources.UserInterface.Common.RolesAssignmentsChanged, user.UserName));
                    }
                    catch
                    {
                        result.AddMessage(string.Format(ApplicationResources.UserInterface.Common.RolesAssignmentFailed, user.UserName));
                    }
                }
                else
                {
                    result.AddError(ApplicationResources.UserInterface.Common.UserDoNotExist);
                }
            }

            return await Task.FromResult(Json(result));
        }
    }
}
