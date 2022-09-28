using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SoloX.BlazorJsonLocalization;
using SoloX.BlazorJsonLocalization.ServerSide;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using SoloX.BlazorJsonLocalization.Http;

namespace SoloX.BlazorJsonLocalisation.Example.ServerSide
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            // Need this to enable the CultureController
            services.AddControllers();

            services.AddServerSideJsonLocalization(
                builder =>
                {
                    builder
                        .EnableDisplayKeysWhileLoadingAsynchronously()
                        // Since we want to use the embedded resources from SoloX.BlazorJsonLocalisation.Example.Components.Embedded
                        .UseEmbeddedJson(options =>
                        {
                            options.ResourcesPath = "Resources";
                        })
                        // Since we want to use the wwwroot resources from SoloX.BlazorJsonLocalisation.Example.Components.StaticAssets
                        .UseHttpHostedJson(options =>
                        {
                            options.ApplicationAssembly = typeof(Program).Assembly;
                            options.ResourcesPath = "Resources";
                        });
                });

            // AspNet core standard localization setup to get culture from the
            // Browser
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en"),
                new CultureInfo("fr")
            };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedUICultures = supportedCultures;
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

            // AspNet core standard localization setup to get culture from the
            // Browser
            app.UseRequestLocalization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // Need MapControllers to enable the CultureController
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
