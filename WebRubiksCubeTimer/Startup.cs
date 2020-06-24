using MailSender.MailSender;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reports.Abstractions;
using Reports.Services;
using TimerDataBase.Abstractions;
using TimerDataBase.DataBaseContext;
using TimerDataBase.Services;
using TimerRequestsDataBase.Abstractions;
using TimerRequestsDataBase.DataBaseContext;
using TimerRequestsDataBase.Services;
using WebRubiksCubeTimer.DBContexts;
using WebRubiksCubeTimer.Models.Users;
using WebRubiksCubeTimer.PasswordResetting;
using WebRubiksCubeTimer.UserManager;

namespace WebRubiksCubeTimer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UserIdentityDBContext>(options =>
                options.UseSqlServer(Configuration["DbContexts:Identity:ConnectionString"]));

            services.AddIdentity<UserModel, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
            }).AddEntityFrameworkStores<UserIdentityDBContext>()
                .AddDefaultTokenProviders();

            services.AddDbContext<TimerDBContext>(options =>
            options.UseSqlServer(Configuration["DbContexts:Results:ConnectionString"]));

            services.AddDbContext<RequestsDBContext>(options =>
            options.UseSqlServer(Configuration["DbContexts:Requests:ConnectionString"]));

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login/Login";
                options.AccessDeniedPath = "/Login/AccesDenied";
            });

            services.AddScoped<IUserManagerExtending, UserManagerExtensions>();
            services.AddScoped<IMessageService, MessageSender>();
            services.AddScoped<ISeriesService, SeriesService>();
            services.AddScoped<IScrambleService, ScrambleService>();
            services.AddScoped<ICubeCollectionService, CubesCollectionService>();
            services.AddScoped<ICubeService, CubeService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IPasswordResetManager, PasswordResetManager>();
            services.AddScoped<IReportService, SeriesReportService>();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddMemoryCache();
            services.AddSession();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/StatusCodePage/500");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseStatusCodePagesWithReExecute("/Error/StatusCodePage/{0}");
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: null,
                    pattern: "{controller}/{action}/{userId?}/Page/{pageNumber:int}",
                    defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
                    name: null,
                    pattern: "{controller}/{action}/{id}/{type:int}",
                    defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
                    name: null,
                    pattern: "{controller}/{action}/{id?}/{returnUrl?}",
                    defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
                    name: null,
                    pattern: "{controller}/{action}/Page/{pageNumber:int}",
                    defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
