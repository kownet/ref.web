using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ref.Core.Data;
using Ref.Core.Notifications;
using Ref.Core.Repositories;
using Ref.Core.Services;
using Ref.Web.Settings;

namespace Ref.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // configure strongly typed settings objects
            var notifySettingsSection = Configuration.GetSection("NotificationSettings");
            services.Configure<NotificationSettings>(notifySettingsSection);

            var notifySettings = notifySettingsSection.Get<NotificationSettings>();

            services.AddScoped<IDbAccess>(
                db => new DbAccess(Configuration.GetConnectionString("RefDb")));

            services.AddScoped<IPushOverNotification>(
                db => new PushOverNotification(
                    token: notifySettings.Token,
                    recipients: notifySettings.Recipients,
                    endpoint: notifySettings.Endpoint));

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }
    }
}