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
using SoloX.BlazorJsonLocalization.Example.Components.StaticAssets;
using SoloX.BlazorJsonLocalization.Example.Components.Embedded;
using SoloX.BlazorJsonLocalization.Example.Components.SharedLocalization;
using An.Other.Name.Embedded;
using SoloX.BlazorJsonLocalization.Example.Components.Embedded3;

namespace SoloX.BlazorJsonLocalization.Example.ServerSide
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
                        .EnableLogger(false)
                        .EnableDisplayKeysWhileLoadingAsynchronously()
                        // Add a localization fallback.
                        .AddFallback("Fallback", typeof(SharedLocalizationExtensions).Assembly)
                        // Since we want to use the embedded resources from SoloX.BlazorJsonLocalization.Example.Components.Embedded
                        .UseComponentsEmbedded()
                        // Since we want to use the embedded resources from SoloX.BlazorJsonLocalization.Example.Components.Embedded2 (with an assembly name different than the root namespace)
                        .UseComponentsEmbedded2()
                        // Since we want to use the embedded resources from SoloX.BlazorJsonLocalization.Example.Components.Embedded2 (with json files named with .razor extension)
                        .UseComponentsEmbedded3()
                        // Since we want to use the wwwroot resources from SoloX.BlazorJsonLocalization.Example.Components.StaticAssets
                        .UseComponentsStaticAssets()
                        // Use the SharedLocalization
                        .UseSharedLocalization();

                    // We can use other setup here with UseHttpHostedJson or UseEmbeddedJson if we need to.
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
