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
using Services.BackgroundTasks;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using BlueDot.Data.UnitsOfWorkInterfaces;
using BlueDot.Data.UnitsOfWork;
using Models;
using Captcha2Api;

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
            ConfigureConnections(services);

            //services.AddDataProtection()
            //        .SetApplicationName("RepublishTool")
            //        .SetDefaultKeyLifetime(TimeSpan.FromDays(14)); ;

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
            
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options => {
                options.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
                options.LoginPath = new PathString("/Identity/Account/Logout"); 
                options.LogoutPath = new PathString("/Identity/Account/Logout");
            });

            

            services.AddHostedService<TemporizadoresTimer>();
            services.AddHostedService<FacturarTimer>();
            services.AddHostedService<CleanerTemporizador>();
            
            services.AddSingleton<IEmailTemplate, RepublishEmailTemplate>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
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
            services.AddTransient<ICaptchaService, CaptchaService>();
            services.AddTransient<IRegistroService, RegistroService>();
            services.AddTransient<IEstadisticasService, EstadisticasService>();
            services.AddTransient<IUserControlService, UserControlService>();
            services.AddTransient<IManejadorFinancieroService, ManejadorFinancieroService>();
            services.AddTransient<IClienteOpcionesService, ClienteOpcionesService>();
            services.AddTransient<IEstadisticaAdminService, EstadisticaAdminService>();
            services.AddTransient<IAgentService, AgentService>();
            services.AddScoped<INotificationsService, NotificationsService>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IEmailRandomService, EmailRandomService>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(RTPolicies.Admin, policy => policy.RequireRole(RTRoles.Admin));
                options.AddPolicy(RTPolicies.Agent, policy => policy.RequireRole(RTRoles.Agent));
            });

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
            loggerFactory.AddFile("logger{Date}");

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

        private void ConfigureConnections(IServiceCollection services)
        {  
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("RepublishContextConnection")));
        }
    }
}
