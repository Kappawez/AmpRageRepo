﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AmpRageRepo.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using AmpRageRepo.Controllers;

namespace AmpRageRepo
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

            //var secret = new SecretController();
          
            //var dbConnection = secret.GetSecret("amprageDBKey").Result;
            var dbConnection = Configuration.GetConnectionString("DbConnection");
            //var dbConnection = "Server=tcp:amprage.database.windows.net,1433;Initial Catalog=AmpRageDB;Persist Security Info=False;User ID=Shadowacademy;Password=PatrikWiksten2019;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //var dbConnection = "Server = (localdb)\\mssqllocaldb; Database = LocalAmpRageDb; Trusted_Connection = True; ";

            //services.AddSingleton(secret);
            services.AddDbContext<AmpContext>(options => options.UseSqlServer(dbConnection));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("en-GB")),
                SupportedCultures = new List<CultureInfo>
              {
                  new CultureInfo("en-GB")
              }
              ,
                SupportedUICultures = new List<CultureInfo>
              {
                  new CultureInfo("GB")
              }
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Path}/{action=CreatePath}/{id?}");
            });
        }
    }
}
