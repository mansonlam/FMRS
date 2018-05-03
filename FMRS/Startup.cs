using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using FMRS.Models;
using FMRS.Service;
using FMRS.Common.DataSource;
using FMRS.DAL;
using FMRS.DAL.Repository;
using FMRS.Model.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FMRS
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
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            services.RegisterServices();
            // allow large form submission
            services.Configure<FormOptions>(x => x.ValueCountLimit = 4096);
            services.AddMvc()
                .AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization();
            // Register appsettings.json "AppConfiguration" section
            services.Configure<AppConfiguration>(Configuration.GetSection("AppConfiguration"));
            // Register Connection string for FMRS.Model > DTO > FMRSContext
            services.AddDbContext<FMRSContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ADOConnectionString")));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.LoginPath = "/Account/Login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.SlidingExpiration = true;
                });

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.Cookie.Name = ".FMRS.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            //app.UseExceptionHandler("/Home/Error");
            app.UseAuthentication();

            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US")
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            };

            app.UseRequestLocalization(options);
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "area",
                    template: "{area:exists}/{controller=Account}/{action=Login}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });
        }

        
    }
    public static class ServiceExtensions
    {
        // Register all services
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IFMRSUserPrincipal, FMRSUserPrincipal>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
            //Services
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<IAdminService, AdminService>();
            services.AddTransient<IDonationRemarkService, DonationRemarkService>();
            services.AddTransient<IDonationRecNExpReconService, DonationRecNExpReconService>();
            services.AddTransient<IDonationRecNExpService, DonationRecNExpService>();
            services.AddTransient<IDonationReserveService, DonationReserveService>();
            services.AddTransient<IDonationUploadService, DonationUploadService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IASOIService, ASOIService>();

            //Repository
            services.AddTransient<IAccessRightRepository, AccessRightRepository>();
            services.AddTransient<IAccessRightDRepository, AccessRightDRepository>();
            services.AddTransient<IAccessRightMRepository, AccessRightMRepository>();
            services.AddTransient<IAccessRightYRepository, AccessRightYRepository>();
            services.AddTransient<IUserInfoRepository, UserInfoRepository>();
            services.AddTransient<IUserSpecialtyRespository, UserSpecialtyRespository>();
            services.AddTransient<IDonTypeRepository, DonTypeRepository>();
            services.AddTransient<IDonDonorRepository, DonDonorRepository>();
            services.AddTransient<IDonPurposeRepository, DonPurposeRepository>();
            services.AddTransient<IDonSupercatRepository, DonSupercatRepository>();
            services.AddTransient<IDonationDetailRepository, DonationDetailRepository>();
            services.AddTransient<IDonationBalanceRepository, DonationBalanceRepository>();
            services.AddTransient<IDonationHistoryRepository, DonationHistoryRepository>();
            services.AddTransient<IDonationRemarkRepository, DonationRemarkRepository>();
            services.AddTransient<IDonSupercatRepository, DonSupercatRepository>();
            services.AddTransient<IDonCatRepository, DonCatRepository>();
            services.AddTransient<IDonSubcatRepository, DonSubcatRepository>();
            services.AddTransient<IDonSubsubcatRepository, DonSubsubcatRepository>();
            services.AddTransient<IFinancialClosingRepository, FinancialClosingRepository>();
            services.AddTransient<IFlashRptHospGpRepository, FlashRptHospGpRepository>();
            services.AddTransient<IUserGroupHospRespository, UserGroupHospRespository>();
            services.AddTransient<IUserGroupRespository, UserGroupRespository>();
            services.AddTransient<IHospitalRepository, HospitalRepository>();
            services.AddTransient<IDonationReserveRepository, DonationReserveRepository>();
            services.AddTransient<IReportRepository, ReportRepository>();
            services.AddTransient<IReportGroupDescRepository, ReportGroupDescRepository>();
            services.AddTransient<IReportNotAccessRepository, ReportNotAccessRepository>();
            services.AddTransient<IReportDetailNotAccessRepository, ReportDetailNotAccessRepository>();
            services.AddTransient<ICwrfAccessControlRepository, CwrfAccessControlRepository>();
            services.AddTransient<IFRMSModelRepository, FRMSModelRepository>();
            services.AddTransient<ICIDRepository, CIDRepository>();
            services.AddTransient<IAsNatureIncomeRepository, AsNatureIncomeRepository>();
            services.AddTransient<IAsProgSubcatRepository, AsProgSubcatRepository>();
            services.AddTransient<IAsServiceProvidedRepository, AsServiceProvidedRepository>();
            services.AddTransient<IAsCatInfoRepository, AsCatInfoRepository>();
            services.AddTransient<IAsASOIProgrammesRepository, AsASOIProgrammesRepository>();
            services.AddTransient<IAsGLRepository, AsGLRepository>();
            services.AddTransient<IAsDetailSAOIRepository, AsDetailSAOIRepository>();

            // Add all other services, repository here.
            // Syntax: services.AddTransient<Interface, Service>();
            return services;
        }
    }
}
