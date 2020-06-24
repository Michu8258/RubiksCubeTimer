using Microsoft.EntityFrameworkCore;
using TimerRequestsDataBase.TableModels;

namespace TimerRequestsDataBase.DataBaseContext
{
    public class RequestsDBContext : DbContext
    {
        public RequestsDBContext(DbContextOptions<RequestsDBContext> options) : base(options) { }

        public DbSet<Request> Requests { get; set; }
        public DbSet<Response> Responses { get; set; }
    }
}
