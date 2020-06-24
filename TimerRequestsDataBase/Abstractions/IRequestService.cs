using Results.UserInterface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimerRequestsDataBase.TableModels;

namespace TimerRequestsDataBase.Abstractions
{
    public interface IRequestService
    {
        Task<Result> CreateNewRequestAsync(string userId, string userName,
            string topic, string message, bool isPrivate);
        Task<IEnumerable<Request>> GetAllActiveRequestsUserAsync(string userId,
            string topicContains = null, DateTime? startTime = null, DateTime? endTime = null);
        Task<IEnumerable<Request>> GetActivePublicRequestsAsync(string topicContains = null,
            DateTime? startTime = null, DateTime? endTime = null);
        Task<long> GetAMountOfModifiedRequestUserAsync(string userId);
        Task<IEnumerable<Request>> GetAllActiveRequestsAsync(string topicContains = null,
            DateTime? startTime = null, DateTime? endTime = null);
        Task<long> GetAmountOfModifiedRequestsAsync();
        Task<IEnumerable<Request>> GetArchivedRequestsAsync(string topicContains = null,
            DateTime? startTime = null, DateTime? endTime = null);
        Task<IEnumerable<Response>> GetAllResponsesForRequestAsync(long requestId);
        Task<Result> AddResponseToRequestAsync(long requestId, string userId,
            string userName, string message, bool admin = true);
        Task<Result> ChangeRequestStateAsync(long requestId, bool caseClosed);
        Task<Result> DeleteAllRequestsOfUserAsync(string userId);
        Task<Request> GetSingleRequest(long requestId);
    }
}
