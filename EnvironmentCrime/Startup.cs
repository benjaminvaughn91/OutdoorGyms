using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace EnvironmentCrime
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config) => Configuration = config;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                services.AddDbContext<ApplicationDbContext> (options => 
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionProd")));
            else
                services.AddDbContext<ApplicationDbContext>(options =>
                   options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Home/Login";
                options.AccessDeniedPath = "/Home/AccessDenied";
            });

            services.AddTransient<ICrimeRepository, EFCrimeRepository>();
            services.AddControllersWithViews();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
