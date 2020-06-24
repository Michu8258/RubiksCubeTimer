using ApplicationResources.UserInterface;
using MailSender.MailSender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Results.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebRubiksCubeTimer.Models.Users;
using WebRubiksCubeTimer.PasswordResetting;
using WebRubiksCubeTimer.ViewModels.Email;

namespace WebRubiksCubeTimer.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class EmailAdminController : Controller
    {
        private readonly IMessageService _emailService;
        private readonly UserManager<UserModel> _userManager;
        private readonly IPasswordResetManager _passwordResetter;
        private readonly IConfiguration _configuration;

        public EmailAdminController(IMessageService messageService, UserManager<UserModel> userManager,
            IPasswordResetManager passwordResetManager, IConfiguration config)
        {
            _emailService = messageService;
            _userManager = userManager;
            _passwordResetter = passwordResetManager;
            _configuration = config;
        }

        [HttpPost]
        public async Task<JsonResult> ResendEmailVerificationCode([FromBody] string id)
        {
            var result = new Result();
            UserModel user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var verificationCode = user.EmailVerificationKey;
                var emailAddress = user.Email;
                var userName = user.UserName;
                var ok = await _emailService.ResendVerificationCode(emailAddress, verificationCode, userName);
                if (ok) result.AddMessage(string.Format(EmailResources.EmailSend, user.UserName));
                else result.AddError(EmailResources.EmailNotSend);
            }
            else
            {
                result.AddError(Common.UserDoNotExist);
            }

            return await Task.FromResult(Json(result));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> SendPasswordResetEmail([FromBody] string emailAddress)
        {
            var result = new Result();
            UserModel user = await _userManager.FindByEmailAsync(emailAddress);
            if(user != null)
            {
                var request = await _passwordResetter.GetRequestAsync(user.Id);
                if (request != null)
                {
                    var ok = await _emailService.SendPasswordResetCode(user.Email, request.ResetKey, user.UserName,
                    request.RequestTime, request.KeyExpires, Convert.ToInt32(_configuration["PasswordReset:MaxAttempts"]));

                    if (ok)
                    {
                        result.AddMessage(string.Format(ApplicationResources.UserInterface.Common.ResetPasswordEmailSend, user.Email));
                    }
                    else
                    {
                        result.AddError(ApplicationResources.UserInterface.Common.ResetPasswordEmailNotSend);
                    }
                }
                else
                {
                    result.AddError(ApplicationResources.UserInterface.Common.NoPasswordRequest);
                }
            }
            else
            {
                result.AddError(ApplicationResources.UserInterface.Common.UserDoNotExist);
            }

            return await Task.FromResult(Json(result));
        }

        public IActionResult GlobalEmail()
        {
            var model = new GlobalEmailViewModel()
            {
                Subject = string.Empty,
                Message = string.Empty,
            };

            GetDomains(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult GlobalEmail(GlobalEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                IDictionary<string, string> users = new Dictionary<string, string>();
                IList<string> excludedDomains = model.ExcludedDomains.Where(ed => ed.Value).Select(ed => ed.Key).ToList();

                foreach (var user in _userManager.Users)
                {
                    bool send = true;

                    foreach (var domain in excludedDomains)
                    {
                        if (user.Email.Contains(domain))
                        {
                            send = false;
                            break;
                        }
                    }

                    if (send)
                    {
                        users.Add(user.Email, user.UserName);
                    }
                }

                Task.Run(() => _emailService.SendGlobalMessage(users, model.Message.Replace("\r\n", "<br/>"), model.Subject));
                return RedirectToAction("UsersList", "UserAdmin");
            }

            GetDomains(model);

            return View(model);
        }

        private void GetDomains(GlobalEmailViewModel model)
        {
            var excludeDomains = new Dictionary<string, bool>();
            var emails = _userManager.Users
                .Select(u => u.Email)
                .Distinct()
                .ToList();

            foreach (var email in emails)
            {
                var domain = email.Substring(email.LastIndexOf('@') + 1);
                if (!string.IsNullOrEmpty(domain))
                {
                    if (!excludeDomains.ContainsKey(domain))
                    {
                        excludeDomains.Add(domain, false);
                    }
                }
            }

            model.ExcludedDomains = excludeDomains;
        }

        public async Task<IActionResult> CustomEmail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var model = new CustomEmailViewModel()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Message = string.Empty,
                    Subject = string.Empty
                };

                return View(model);
            }

            return RedirectToAction("UsersList", "UserAdmin");
        }

        [HttpPost]
        public async Task<IActionResult> CustomEmail(CustomEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ok = await _emailService.SendCustomEmail(model.UserName, model.Email, model.Subject, model.Message.Replace("\r\n", "<br/>"));

                if (ok)
                {
                    return RedirectToAction("UsersList", "UserAdmin");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ApplicationResources.UserInterface.EmailResources.EmailNotSent);
                }
            }

            return View(model);
        }
    }
}
