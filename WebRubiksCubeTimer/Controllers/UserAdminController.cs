using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Results.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimerDataBase.Abstractions;
using TimerRequestsDataBase.Abstractions;
using WebRubiksCubeTimer.Extensions;
using WebRubiksCubeTimer.Models.Users;

namespace WebRubiksCubeTimer.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserAdminController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManger;
        private readonly IUserValidator<UserModel> _userValidator;
        private readonly IPasswordValidator<UserModel> _passwordValidator;
        private readonly IPasswordHasher<UserModel> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly ICubeCollectionService _cubeCollectionService;
        private readonly ISeriesService _seriesService;
        private readonly ICubeService _cubeService;
        private readonly IRequestService _requestService;

        public UserAdminController(UserManager<UserModel> userMgr, IUserValidator<UserModel> userValidator,
            IPasswordValidator<UserModel> passwordValidator, IPasswordHasher<UserModel> passwordHasher,
            IConfiguration configuration, ICubeCollectionService cubeCollectionService,
            ISeriesService seriesService, ICubeService cubeService,
            IRequestService requestService, RoleManager<IdentityRole> roleMgr)
        {
            _userManager = userMgr;
            _userValidator = userValidator;
            _passwordValidator = passwordValidator;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _cubeCollectionService = cubeCollectionService;
            _seriesService = seriesService;
            _cubeService = cubeService;
            _requestService = requestService;
            _roleManger = roleMgr;
        }

        public async Task<IActionResult> UsersList()
        {
            return View(new UsersListModel(ModelState)
            {
                DefaultRoleName = GetDefaultRoleName(),
                Roles = await GetCurrentRolesAsync(),
                FilteredMail = string.Empty,
                FilteredPhoneNumber = string.Empty,
                FilteredUserName = string.Empty,
                FilterRoles = string.Empty,
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetUsersData(string userName, string email,
            string phoneNumber, string role, int page = 1)
        {
            var users = await _userManager.Users.ToListAsync();

            if (! string.IsNullOrEmpty(userName))
            {
                users.RemoveAll(u => !u.UserName.Contains(userName));
            }

            if (! string.IsNullOrEmpty(email))
            {
                users.RemoveAll(u => !u.Email.Contains(email));
            }

            if (! string.IsNullOrEmpty(phoneNumber))
            {
                users.RemoveAll(u => !u.PhoneNumber.Contains(phoneNumber));
            }

            if (! string.IsNullOrEmpty(role))
            {
                if (await _roleManger.RoleExistsAsync(role))
                {
                    var usersInRole = new List<UserModel>();
                    foreach (var user in users)
                    {
                        if (await _userManager.IsInRoleAsync(user, role))
                        {
                            usersInRole.Add(user);
                        }
                    }

                    users = usersInRole;
                }
            }

            int pageSize = Convert.ToInt32(_configuration["Paginations:UserListPagination"]);

            var outputlist = users
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var model = new UsersListModel(ModelState)
            {
                Users = outputlist,
                DefaultRoleName = GetDefaultRoleName(),
                FilteredMail = email,
                FilteredPhoneNumber = phoneNumber,
                FilteredUserName = userName,
                FilterRoles = role,
                Pagination = this.GetPaginationData(users.Count(), pageSize, page),
            };

            return Json(new Result(string.Empty, false)
            {
                JsonData = model,
            });
        }

        [HttpPost]
        public async Task<JsonResult> PinUser(string id, string userName)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                HttpContext.Session.SetString(ApplicationResources.Static.SessionKeys.PinnedUserKey, id);
                return Json(new Result(string.Format(ApplicationResources.UserInterface.Common.UserPinned, userName), false));
            }

            return Json(new Result(ApplicationResources.UserInterface.Common.UserPinError, true));
        }

        [HttpPost]
        public JsonResult UnpinUser()
        {
            HttpContext.Session.Remove(ApplicationResources.Static.SessionKeys.PinnedUserKey);
            return Json(new Result(ApplicationResources.UserInterface.Common.UserUnpinned, false));
        }

        public IActionResult CreateNewUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewUser(AddUserModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel user = new UserModel()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = model?.ConfirmEmail == true,
                    EmailVerificationKey = model.EmailConfirmationString.ToUpper(),
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("UsersList");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> EditUser(string id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            UserModel user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToRoute(returnUrl);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(string id, string userName, string email, string password,
            string emailVerificationKey, string phoneNumber, bool emailConfirmed, string returnUrl)
        {
            UserModel user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = email;
                IdentityResult validEmail = await _userValidator.ValidateAsync(_userManager, user);
                if (!validEmail.Succeeded)
                {
                    this.AddErrorsFromResult(validEmail);
                }

                IdentityResult validPassword = null;

                if (!string.IsNullOrEmpty(password))
                {
                    validPassword = await _passwordValidator.ValidateAsync(_userManager, user, password);
                    if (validPassword.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, password);
                    }
                    else
                    {
                        this.AddErrorsFromResult(validPassword);
                    }
                }

                if ((validEmail.Succeeded && validPassword == null) || (validEmail.Succeeded && password != string.Empty && validPassword.Succeeded))
                {
                    user.UserName = userName;
                    user.EmailVerificationKey = emailVerificationKey;
                    user.PhoneNumber = phoneNumber;
                    user.EmailConfirmed = emailConfirmed;

                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        this.AddErrorsFromResult(result);
                    }
                }
            }

            return View(user);
        }

        public async Task<IActionResult> ConfirmUserDeletion(string id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            UserModel user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                UserDeletionModel model = new UserDeletionModel()
                {
                    Id = user.Id,
                    DeleteUserDataStorage = false,
                    UserName = user.UserName,
                    EmailAddress = user.Email,
                };
                return View(model);
            }
            else
            {
                ModelState.AddModelError("", ApplicationResources.UserInterface.Common.UserDoNotExist);
            }

            return RedirectToAction("UsersList");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(UserDeletionModel deleteModel)
        {
            var userId = deleteModel.Id;
            UserModel user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    if (deleteModel.DeleteUserDataStorage)
                    {
                        await _cubeCollectionService.DeleteAllCubesFromUserCollectionAsync(userId);
                        await _seriesService.DeleteAllSeriesOfUserAsync(userId);
                        await _cubeService.DeleteAllRatingsOfUser(userId);
                        await _requestService.DeleteAllRequestsOfUserAsync(userId);
                    }
                    return Redirect(deleteModel.ReturnUrl);
                }
                else
                {
                    this.AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", ApplicationResources.UserInterface.Common.UserDoNotExist);
            }

            return RedirectToAction("UsersList");
        }

        private string GetDefaultRoleName()
        {
            return _configuration["DefaultRoleName"];
        }

        private async Task<IEnumerable<IdentityRole>> GetCurrentRolesAsync()
        {
            var roles = await _roleManger.Roles.ToListAsync();
            roles.RemoveAll(r => string.Equals(r.Name, GetDefaultRoleName(), StringComparison.OrdinalIgnoreCase));
            return roles;
        }
    }
}
