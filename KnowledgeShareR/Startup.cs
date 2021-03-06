using KnowledgeShareR.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using KnowledgeShareR.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace KnowledgeShareR
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddSignalR();

            services.AddDbContext<KnowledgeShareDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("KnowledgeShareDbContext")));

            services.AddDefaultIdentity<IdentityUser>(options =>
                                                  options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<KnowledgeShareDbContext>();

            services.AddAuthentication()
            .AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection =
                    Configuration.GetSection("Authentication:Google");

                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
                options.CallbackPath = "/google-signin";

                options.Scope.Add("profile");
                options.Events.OnCreatingTicket = (context) =>
                {
                    var email = context.User.GetProperty("email").GetString();
                    var picture = context.User.GetProperty("picture").GetString();

                    var db = context.HttpContext.RequestServices.GetRequiredService<KnowledgeShareDbContext>();
                    var isExistingUser = db.UserInfos.Any(x => x.UserName == email);

                    if (!isExistingUser)
                    {
                        db.UserInfos.Add(new Models.UserInfo() { UserName = email, ProfilePicture = picture });
                        db.SaveChanges();
                    }

                    return Task.CompletedTask;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<KnowledgeShareRHub>("/knowledgeShareRHub");
            });
        }
    }
}
