using MailSender.MailSender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Results.UserInterface;
using System;
using System.Threading.Tasks;
using WebRubiksCubeTimer.Extensions;
using WebRubiksCubeTimer.Models.Login;
using WebRubiksCubeTimer.Models.Users;
using WebRubiksCubeTimer.PasswordResetting;
using WebRubiksCubeTimer.PasswordResetting.Helpers;
using WebRubiksCubeTimer.UserManager;

namespace WebRubiksCubeTimer.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly IUserManagerExtending _userManagerExtensions;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly IMessageService _messageService;
        private readonly IConfiguration _configuration;
        private readonly IPasswordResetManager _passwordResetManger;
        private readonly IUserValidator<UserModel> _userValidator;
        private readonly IPasswordValidator<UserModel> _passwordValidator;
        private readonly IPasswordHasher<UserModel> _passwordHasher;

        public LoginController(UserManager<UserModel> userMgr, SignInManager<UserModel> signInMgr,
            IMessageService messageService, IConfiguration config, IPasswordResetManager passwordResetManager,
            IUserValidator<UserModel> userValidator, IPasswordValidator<UserModel> passwordValidator,
            IPasswordHasher<UserModel> passwordHasher, IUserManagerExtending userManagerExtensions)
        {
            _userManager = userMgr;
            _userManagerExtensions = userManagerExtensions;
            _signInManager = signInMgr;
            _messageService = messageService;
            _configuration = config;
            _passwordResetManger = passwordResetManager;
            _userValidator = userValidator;
            _passwordValidator = passwordValidator;
            _passwordHasher = passwordHasher;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                UserModel user = await _userManager.FindByEmailAsync(model.EmailAddress);
                if(user != null)
                {
                    await _signInManager.SignOutAsync();

                    if(!user.EmailConfirmed)
                    {
                        return RedirectToAction("NeedVerification", new { userName = user.UserName, email = user.Email });
                    }

                    Microsoft.AspNetCore.Identity.SignInResult result =
                        await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                    if (result.Succeeded)
                    {
                        await _userManagerExtensions.UpdateUserLoginTimeSpan(user.Id);
                        return Redirect(returnUrl ?? "/");
                    }
                }

                ModelState.AddModelError(nameof(model.EmailAddress), ApplicationResources.UserInterface.Common.BadEmailOrPassword);
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult AccesDenied(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAccount(CreateAccountModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel user = new UserModel()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = false,
                    EmailVerificationKey = RandomKeyGenerator.RandomString(25),
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var send = await _messageService.SendNewVerificationCode(user.Email, user.EmailVerificationKey, user.UserName);
                    if (send)
                    {
                        return RedirectToAction("NeedVerification", new { userName = user.UserName, email = user.Email });
                    }
                    return View(model);
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

        [AllowAnonymous]
        public IActionResult NeedVerification(string userName, string email)
        {
            return View(new VerifyEmailModel()
            {
                UserName = userName,
                Email = email,
                VerificationKey = string.Empty,
                Password = string.Empty,
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NeedVerification(VerifyEmailModel model)
        {
            if(ModelState.IsValid)
            {
                UserModel user = await _userManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    if (Equals(user.EmailVerificationKey, model.VerificationKey))
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                        await _signInManager.SignOutAsync(); 

                        Microsoft.AspNetCore.Identity.SignInResult result =
                         await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, _configuration["DefaultRoleName"]);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", ApplicationResources.UserInterface.Common.VerificationKeyDoesNotMatch);
                    }
                }
                else
                {
                    ModelState.AddModelError("", ApplicationResources.UserInterface.Common.UserDoNotExist);
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            var hours = Convert.ToInt32(_configuration["PasswordReset:KeyExpireHours"]);
            var attempts = Convert.ToInt32(_configuration["PasswordReset:MaxAttempts"]);

            return View(new PasswordResetModel()
            {
                EmailAddress = string.Empty,
                Hours = hours,
                Attempts = attempts,
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> AddPasswordResetRequest([FromBody] string emailAddress)
        {
            var result = new Result();
            var user = await _userManager.FindByEmailAsync(emailAddress);
            if(user != null)
            {
                var ok = await _passwordResetManger.AddResetRequestAsync(user.Id);
                if (!ok)
                {
                    result.AddError(ApplicationResources.UserInterface.Common.ResetPasswordRequestNotAdded);
                }
            }
            else
            {
                result.AddError(ApplicationResources.UserInterface.Common.UserDoNotExist);
            }

            return await Task.FromResult(Json(result));
        }

        [AllowAnonymous]
        public IActionResult VerifyCode(string email)
        {
            return View(new PasswordResetCodeModel()
            {
                EmailAddress = email,
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(PasswordResetCodeModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.EmailAddress);
                if (user != null)
                {
                    if(Equals(model.NewPassword, model.NewPassword2))
                    {
                        var ok = await ValidateAttemptOfPasswordReset(model, user.Id);
                        if (ModelState.IsValid && ok)
                        {
                            IdentityResult validEmail = await _userValidator.ValidateAsync(_userManager, user);
                            if (!validEmail.Succeeded)
                            {
                                this.AddErrorsFromResult(validEmail);
                            }
                            else
                            {
                                var validPassword = await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                                if (validPassword.Succeeded)
                                {
                                    user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                                    if (validEmail.Succeeded && model.NewPassword != string.Empty && validPassword.Succeeded)
                                    {
                                        IdentityResult result = await _userManager.UpdateAsync(user);
                                        if (result.Succeeded)
                                        {
                                            return RedirectToAction("Login", "Login");
                                        }
                                        else
                                        {
                                            this.AddErrorsFromResult(result);
                                        }
                                    }
                                }
                                else
                                {
                                    this.AddErrorsFromResult(validPassword);
                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", ApplicationResources.UserInterface.Common.NotEqualPasswords);
                    }
                }
                else
                {
                    ModelState.AddModelError("", ApplicationResources.UserInterface.Common.UserDoNotExist);
                }
            }

            return View(model);
        }

        private async Task<bool> ValidateAttemptOfPasswordReset(PasswordResetCodeModel model, string userId)
        {
            var result = await _passwordResetManger.ValidateRequestAsync(userId, model.Code);

            if (result.NoRequest)
            {
                ModelState.AddModelError("", ApplicationResources.UserInterface.PasswordReset.NoRequest);
                return false;
            }

            if (result.Blocked)
            {
                ModelState.AddModelError("", ApplicationResources.UserInterface.PasswordReset.Blocked);
                return false;
            }

            if (result.Expired)
            {
                ModelState.AddModelError("", ApplicationResources.UserInterface.PasswordReset.Expired);
                return false;
            }

            if (!result.Success)
            {
                ModelState.AddModelError("", ApplicationResources.UserInterface.PasswordReset.InvalidCode);
                return false;
            }

            if (result.Success)
            {
                return true;
            }

            ModelState.AddModelError("", ApplicationResources.UserInterface.PasswordReset.Exception);
            return false;
        }
    }
}
