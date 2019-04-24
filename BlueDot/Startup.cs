using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Republish.Models;
using Republish.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Republish.Data;
using Republish.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Impls;
using Services;
using Microsoft.Extensions.Logging;

namespace Republish
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // The Tempdata provider cookie is not essential. Make it essential
            // so Tempdata is functional when tracking is disabled.
            services.Configure<CookieTempDataProviderOptions>(options => {
                options.Cookie.IsEssential = true;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("RepublishContextConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options => {
                options.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
                options.LoginPath = new PathString("/Identity/Account/Logout"); 
                options.LogoutPath = new PathString("/Identity/Account/Logout");
            });

            services.AddSingleton<IEmailTemplate, RepublishEmailTemplate>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ICustomSignInManger, ICustomSignInManger>();
            services.AddTransient<IAdministratorsService, AdministratorsService>();
            services.AddTransient<IStoredProcedureService, StoredProcedureService>();
            services.AddTransient<IForgotPasswordService, ForgotPasswordService>();
            services.AddTransient<IUpdatePasswordService, UpdatePasswordService>();
            services.AddTransient<IGrupoService, GrupoService>();
            services.AddTransient<IAnuncioService, AnuncioService>();
            services.AddTransient<ITemporizadorService, TemporizadorService>();
            services.AddTransient<IChequerService, ChequerService>();
            
            services.AddMvc().AddRazorPagesOptions(opts => {
                opts.Conventions.AddAreaPageRoute("Identity", "/Identity/Account/Login", "");
                opts.Conventions.AddAreaPageRoute("Identity", "/Account/Login","");
                opts.Conventions.AddAreaPageRoute("Identity", "/Login","");
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext db, SignInManager<IdentityUser> s, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            loggerFactory.AddFile("Logs/myapp-{Date}.txt");

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Default}/{action=Index}/{id?}"
                );
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureIdentityOptions(IdentityOptions options)
        {
            // Set lock out time to a large value so that the user is locked out indefinitely.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(365 * 200);

            // This is needed to prevent the user from being locked out due to an expired two factor authentication code.
            AppContext.SetSwitch("Microsoft.AspNetCore.Identity.CheckPasswordSignInAlwaysResetLockoutOnSuccess", true);
        }
    }
}
