using System.Collections.Generic;
using TimerRequestsDataBase.TableModels;
using WebRubiksCubeTimer.Models.Pagination;

namespace WebRubiksCubeTimer.ViewModels.Requests
{
    public class RequestsExplorerViewModel
    {
        public RequestsExplorerViewModel()
        {
            Requests = new List<RequestItem>();
        }

        public PaginationModel Pagination { get; set; }
        public IList<RequestItem> Requests { get; set; }
        public int MaxResponseLength { get; set; }

        public class RequestItem
        {
            public Request Request { get; set; }
            public IEnumerable<Response> Responses { get; set; }
        }
    }
}
