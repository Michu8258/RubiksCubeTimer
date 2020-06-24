using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Results.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimerRequestsDataBase.Abstractions;
using TimerRequestsDataBase.TableModels;
using WebRubiksCubeTimer.Extensions;
using WebRubiksCubeTimer.Models.Users;
using WebRubiksCubeTimer.ViewModels.Requests;

namespace WebRubiksCubeTimer.Controllers
{
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly UserManager<UserModel> _userManager;
        private readonly IConfiguration _configuration;

        public RequestsController(IRequestService requestService,
            UserManager<UserModel> userManager,
            IConfiguration configuration)
        {
            _requestService = requestService;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRequestsAmount()
        {
            var model = new RequestViewComponentViewModel()
            {
                Display = false,
                AdminOrMod = false,
                ControllerName = string.Empty,
            };

            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user != null)
            {
                model.Display = true;
                model.AdminOrMod = User.IsInRole("Administrator") || User.IsInRole("Moderator");
                model.AmountOfNewStatesUser = await _requestService.GetAMountOfModifiedRequestUserAsync(user.Id);
            }

            if (model.AdminOrMod)
            {
                model.AmountOfNewStatesAdmin = await _requestService.GetAmountOfModifiedRequestsAsync();
            }
            else
            {
                model.AmountOfNewStatesAdmin = 0;
            }

            return Json(model);
        }

        public IActionResult CreateRequest()
        {
            return View(new CreateRequestViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(CreateRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
                if (user != null)
                {
                    var result = await _requestService.CreateNewRequestAsync(user.Id, user.UserName,
                        model.Topic, model.Message, !model.PublicRequest);

                    if (result.IsFaulted)
                    {
                        ModelState.AddModelError(string.Empty, result.Messages.FirstOrDefault());
                    }
                    else
                    {
                        //TODO - here whould be redirect to user requests page.
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, ApplicationResources.UserInterface.Common.UserDoNotExist);
            }

            return View(model);
        }

        public IActionResult PublicRequests()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPublicRequestsData(string startDate,
            string endDate, string topicContains, int page = 1)
        {
            DateTime start = DateTime.Now;
            DateTime stop = DateTime.Now;
            start = start.ConvertFromString(startDate);
            stop = stop.ConvertFromString(endDate, true);

            var pageSize = Convert.ToInt32(_configuration["Paginations:RequestsPublicExplorer"]);

            var rawRequests = (await _requestService.GetActivePublicRequestsAsync(topicContains, start, stop))
                .OrderByDescending(r => r.CreationTime)
                .ToList();

            return await GetRequestsDataInternal(page, pageSize, rawRequests);
        }

        public async Task<IActionResult> PrivateRequests()
        {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return RedirectToAction("StatusCodePage", "Error", new { code = 404 });
            }
            else
            {
                return View("PrivateRequests", user.Id);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserRequestData(string userId,
            string startDate, string endDate, string topicContains, int page = 1)
        {
            DateTime start = DateTime.Now;
            DateTime stop = DateTime.Now;
            start = start.ConvertFromString(startDate);
            stop = stop.ConvertFromString(endDate, true);

            var pageSize = Convert.ToInt32(_configuration["Paginations:RequestsPrivateExplorer"]);

            var rawRequests = (await _requestService.GetAllActiveRequestsUserAsync(userId, topicContains, start, stop))
                .OrderByDescending(r => r.CreationTime)
                .ToList();

            return await GetRequestsDataInternal(page, pageSize, rawRequests);
        }

        [HttpPost]
        public async Task<IActionResult> AddResponseUser(long requestId, string userId,
            string message)
        {
            Result response;

            if (string.IsNullOrEmpty(userId))
            {
                response = new Result(ApplicationResources.UserInterface.Common.UserDoNotExist);
            }
            else
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    response = await _requestService.AddResponseToRequestAsync(requestId,
                        user.Id, user.UserName, message, false);
                }
                else
                {
                    response = new Result(ApplicationResources.UserInterface.Common.UserDoNotExist);
                }
            }

            return Json(response);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> RequestsManager()
        {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return RedirectToAction("StatusCodePage", "Error", new { code = 404 });
            }
            else if(user != null && (User.IsInRole("Administrator") || User.IsInRole("Moderator")))
            {
                return View();
            }
            else
            {
                return RedirectToAction("AccesDenied", "Login", new { returnUrl = "/Home/Index/" });
            }
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetRequestsData(string startDate,
            string endDate, string topicContains, int page = 1)
        {
            DateTime start = DateTime.Now;
            DateTime stop = DateTime.Now;
            start = start.ConvertFromString(startDate);
            stop = stop.ConvertFromString(endDate, true);

            var pageSize = Convert.ToInt32(_configuration["Paginations:RequestsManagerPagination"]);

            var rawRequests = (await _requestService.GetAllActiveRequestsAsync(topicContains, start, stop))
                .OrderByDescending(r => r.CreationTime)
                .ToList();

            return await GetRequestsDataInternal(page, pageSize, rawRequests);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [HttpPost]
        public async Task<IActionResult> AddResponseAdmin(long requestId, string message)
        {
            Result response;

            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user != null)
            {
                if (User.IsInRole("Administrator") == true || User.IsInRole("Moderator") == true)
                {
                    response = await _requestService.AddResponseToRequestAsync(requestId,
                        user.Id, user.UserName, message, true);
                }
                else
                {
                    response = new Result(ApplicationResources.UserInterface.Common.UserNoAccess);
                }
            }
            else
            {
                response = new Result(ApplicationResources.UserInterface.Common.UserDoNotExist);
            }

            return Json(response);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [HttpPost]
        public async Task<IActionResult> ChangeRequestStatus(long requestId, bool closed)
        {
            Result response;

            if (User.IsInRole("Administrator") == true || User.IsInRole("Moderator") == true)
            {
                response = await _requestService.ChangeRequestStateAsync(requestId, closed);
            }
            else
            {
                response = new Result(ApplicationResources.UserInterface.Common.UserNoAccess);
            }

            return Json(response);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> RequestsArchive()
        {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return RedirectToAction("StatusCodePage", "Error", new { code = 404 });
            }
            else if (user != null && (User.IsInRole("Administrator") || User.IsInRole("Moderator")))
            {
                return View();
            }
            else
            {
                return RedirectToAction("AccesDenied", "Login", new { returnUrl = "/Home/Index/" });
            }
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetArchivedRequests(string startDate,
            string endDate, string topicContains, int page = 1)
        {
            DateTime start = DateTime.Now;
            DateTime stop = DateTime.Now;
            start = start.ConvertFromString(startDate);
            stop = stop.ConvertFromString(endDate, true);

            var pageSize = Convert.ToInt32(_configuration["Paginations:RequestsArchiveExplorer"]);

            var rawRequests = (await _requestService.GetArchivedRequestsAsync(topicContains, start, stop))
                .OrderByDescending(r => r.CreationTime)
                .ToList();

            return await GetRequestsDataInternal(page, pageSize, rawRequests);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetResponses(long requestId)
        {
            Result result;

            var request = await _requestService.GetSingleRequest(requestId);
            if (request == null)
            {
                result = new Result(ApplicationResources.RequestsDB.Messages.RequestIdInvalid);
            }
            else
            {
                var responses = await _requestService.GetAllResponsesForRequestAsync(request.Identity);
                if (responses != null)
                {
                    result = new Result(string.Empty, false)
                    {
                        JsonData = new RequestsExplorerViewModel.RequestItem()
                        {
                            Request = request,
                            Responses = responses,
                        }
                    };
                }
                else
                {
                    result = new Result(ApplicationResources.RequestsDB.Messages.RequestIdInvalid);
                }
            }

            return Json(result);
        }

        private async Task<IActionResult> GetRequestsDataInternal(int page, int pageSize,
            IEnumerable<Request> rawRequests)
        {
            if (page < 1)
            {
                page = 1;
            }

            var requests = rawRequests
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var model = new RequestsExplorerViewModel()
            {
                Pagination = this.GetPaginationData(rawRequests.Count(), pageSize, page),
                MaxResponseLength = TimerRequestsDataBase.TableModels.Request.MessageMaxLength,
            };

            foreach (var request in requests)
            {
                model.Requests.Add(new RequestsExplorerViewModel.RequestItem()
                {
                    Request = request,
                    Responses = await _requestService.GetAllResponsesForRequestAsync(request.Identity),
                });
            }

            return Json(new Result(string.Empty, false)
            {
                JsonData = model,
            });
        }
    }
}
