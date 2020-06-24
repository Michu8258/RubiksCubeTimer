using Microsoft.EntityFrameworkCore;
using Results.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TimerRequestsDataBase.Abstractions;
using TimerRequestsDataBase.DataBaseContext;
using TimerRequestsDataBase.TableModels;

namespace TimerRequestsDataBase.Services
{
    public class RequestService : IRequestService
    {
        private readonly RequestsDBContext _dbContext;

        public RequestService(RequestsDBContext context)
        {
            _dbContext = context;
        }

        public async Task<Result> CreateNewRequestAsync(string userId, string userName,
            string topic, string message, bool isPrivate)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName)
                || string.IsNullOrEmpty(topic) || string.IsNullOrEmpty(message))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (message.Length > Request.MessageMaxLength)
            {
                return new Result(string.Format(ApplicationResources.RequestsDB.Messages.MessageTooLong, Request.MessageMaxLength));
            }

            if (topic.Length > Request.TopicMaxLength)
            {
                return new Result(string.Format(ApplicationResources.RequestsDB.Messages.TopicTooLong, Request.TopicMaxLength));
            }

            _dbContext.Requests.Add(new Request()
            {
                UserIdentity = userId,
                UserName = userName,
                CreationTime = DateTime.Now,
                Topic = topic,
                Message = message,
                NewChangesByUser = true,
                NewChangesByAdmin = false,
                CaseClosed = false,
                PrivateRequest = isPrivate,
                RepliesAmount = 0,
            });

            if ((await _dbContext.SaveChangesAsync()) == 1)
            {
                return new Result(string.Empty, false);
            }

            return new Result(ApplicationResources.RequestsDB.Messages.RequestAddFailed);
        }

        public async Task<IEnumerable<Request>> GetAllActiveRequestsUserAsync(string userId,
            string topicContains = null, DateTime? startTime = null, DateTime? endTime = null)
        {
            var requests = await GetRequestsAsync(s => s.UserIdentity == userId && s.CaseClosed == false);
            requests = ProcessRequestsFiltering(requests, topicContains, startTime, endTime);
            return requests;
        }

        public async Task<IEnumerable<Request>> GetActivePublicRequestsAsync(string topicContains = null,
            DateTime? startTime = null, DateTime? endTime = null)
        {
            var requests = await GetRequestsAsync(s => s.PrivateRequest == false && s.CaseClosed == false);
            requests = ProcessRequestsFiltering(requests, topicContains, startTime, endTime);
            return requests;
        }

        public async Task<long> GetAMountOfModifiedRequestUserAsync(string userId)
        {
            return (await GetRequestsAsync(s => s.UserIdentity == userId &&
            s.CaseClosed == false && s.NewChangesByAdmin == true)).Count();
        }

        public async Task<IEnumerable<Request>> GetAllActiveRequestsAsync(string topicContains = null,
            DateTime? startTime = null, DateTime? endTime = null)
        {
            var requests = await GetRequestsAsync(s => s.CaseClosed == false);
            requests = ProcessRequestsFiltering(requests, topicContains, startTime, endTime);
            return requests;
        }

        public async Task<long> GetAmountOfModifiedRequestsAsync()
        {
            return (await GetRequestsAsync(s => s.CaseClosed == false && s.NewChangesByUser == true)).Count();
        }

        public async Task<IEnumerable<Request>> GetArchivedRequestsAsync(string topicContains = null,
            DateTime? startTime = null, DateTime? endTime = null)
        {
            var requests = await GetRequestsAsync(s => s.CaseClosed == true);
            requests = ProcessRequestsFiltering(requests, topicContains, startTime, endTime);
            return requests;
        }

        public async Task<IEnumerable<Response>> GetAllResponsesForRequestAsync(long requestId)
        {
            var request = await GetRequestByIdAsync(requestId);

            if (request != null)
            {
                return await _dbContext.Responses
                    .Include(r => r.Request)
                    .Where(r => r.Request == request)
                    .OrderBy(r => r.ResponseTime)
                    .ToListAsync();
            }

            return new List<Response>();
        }

        public async Task<Result> AddResponseToRequestAsync(long requestId, string userId,
            string userName, string message, bool admin = true)
        {
            if (requestId < 1 || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(message))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (message.Length > Request.MessageMaxLength)
            {
                return new Result(string.Format(ApplicationResources.RequestsDB.Messages.MessageTooLong, Request.MessageMaxLength));
            }

            var request = await GetRequestByIdAsync(requestId);

            if (request == null)
            {
                return new Result(ApplicationResources.RequestsDB.Messages.RequestIdInvalid);
            }
            else
            {
                if(request.CaseClosed)
                {
                    return new Result(ApplicationResources.RequestsDB.Messages.ResponseRequestClosed);
                }

                if (request.UserIdentity != userId && !admin)
                {
                    return new Result(ApplicationResources.RequestsDB.Messages.RequestNotBelongsToUser);
                }

                _dbContext.Responses.Add(new Response()
                {
                    Request = request,
                    UserIdentity = userId,
                    UserName = userName,
                    ResponseTime = DateTime.Now,
                    Message = message,
                });

                if ((await _dbContext.SaveChangesAsync()) == 1)
                {
                    var requestUpdated = await UpdateRequestDataAsync(request, admin);
                    if (requestUpdated)
                    {
                        return new Result(string.Empty, false);
                    }
                    return new Result(ApplicationResources.RequestsDB.Messages.RequestAmountUpdateFailed);
                }

                return new Result(ApplicationResources.RequestsDB.Messages.ResponseAddFailed);
            }
        }

        public async Task<Result> ChangeRequestStateAsync(long requestId, bool caseClosed)
        {
            var requests = await _dbContext.Requests
                .Where(r => r.Identity == requestId)
                .ToListAsync();

            if (requests.Any())
            {
                var changed = 0;
                foreach (var request in requests)
                {
                    if (request.CaseClosed != caseClosed)
                    {
                        request.CaseClosed = caseClosed;
                        request.NewChangesByAdmin = !caseClosed;
                        request.NewChangesByUser = false;
                        changed++;
                    }
                }

                var result = await _dbContext.SaveChangesAsync();

                if (result >= changed)
                {
                    return new Result(string.Empty, false);
                }

                return new Result(ApplicationResources.RequestsDB.Messages.RequestCloseFailed);
            }

            return new Result(ApplicationResources.RequestsDB.Messages.RequestIdInvalid);
        }

        public async Task<Result> DeleteAllRequestsOfUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            var requests = await GetRequestsAsync(r => r.UserIdentity == userId);
            var requestsAmount = requests.Count();

            if (!requests.Any())
            {
                return new Result(string.Empty, false);
            }
            else
            {
                var responsesAmount = 0;
                var result = 0;
                foreach (var request in requests)
                {
                    var responses = await GetAllResponsesForRequestAsync(request.Identity);
                    responsesAmount += responses.Count();
                    _dbContext.Responses.RemoveRange(responses);
                    result += await _dbContext.SaveChangesAsync();
                }

                _dbContext.Requests.RemoveRange(requests);
                result += await _dbContext.SaveChangesAsync();

                if (result >= requestsAmount + responsesAmount)
                {
                    return new Result(string.Empty, false);
                }

                return new Result(string.Format(ApplicationResources.RequestsDB.Messages.DeletingUserRequestsFailed,
                    requestsAmount, responsesAmount, result));
            }
        }

        public async Task<Request> GetSingleRequest(long requestId)
        {
            return await GetRequestByIdAsync(requestId);
        }

        private IEnumerable<Request> ProcessRequestsFiltering(IEnumerable<Request> requests, string topicContains = null,
            DateTime? startTime = null, DateTime? endTime = null)
        {
            if (requests.Any())
            {
                if (startTime.HasValue)
                {
                    requests = requests.Where(r => r.CreationTime >= startTime).ToList();
                }

                if (endTime.HasValue)
                {
                    requests = requests.Where(r => r.CreationTime <= endTime).ToList();
                }

                if (!string.IsNullOrEmpty(topicContains))
                {
                    requests = requests.Where(r => r.Topic.Contains(topicContains)).ToList();
                }
            }

            return requests;
        }

        private async Task<bool> UpdateRequestDataAsync(Request request, bool changesByAdmin = true)
        {
            if (request == null)
            {
                return false;
            }

            var amount = (await GetAllResponsesForRequestAsync(request.Identity)).Count();
            request.RepliesAmount = amount;
            request.NewChangesByUser = !changesByAdmin;
            request.NewChangesByAdmin = changesByAdmin;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task<IEnumerable<Request>> GetRequestsAsync(Expression<Func<Request, bool>> whereMatch)
        {
            return await _dbContext.Requests
                .Where(whereMatch)
                .OrderByDescending(r => r.CreationTime)
                .ToListAsync();
        }

        private async Task<Request> GetRequestByIdAsync(long requestId)
        {
            return await _dbContext.Requests
                .FirstOrDefaultAsync(r => r.Identity == requestId);
        }
    }
}
