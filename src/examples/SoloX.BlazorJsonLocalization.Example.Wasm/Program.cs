using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SoloX.BlazorJsonLocalization;
using SoloX.BlazorJsonLocalization.WebAssembly;
using Blazored.LocalStorage;
using System.Globalization;

namespace SoloX.BlazorJsonLocalization.Example.Wasm
{
    public class Program
    {
        internal const string LanguageKey = "language";

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddWebAssemblyJsonLocalization(
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
                            options.NamingPolicy = (basePath, cultureName) => new Uri($"{basePath}{(string.IsNullOrEmpty(cultureName) ? string.Empty : $"-{cultureName}")}.json", UriKind.Relative);
                            options.ResourcesPath = "Resources";
                        });
                });

            builder.Services.AddBlazoredLocalStorage();

            var host = builder.Build();

            // Get the local storage in order to get the current language (if set) as soon as possible.
            var localStorage = host.Services.GetRequiredService<ISyncLocalStorageService>();

            if (localStorage.ContainKey(LanguageKey))
            {
                var ci = CultureInfo.GetCultureInfo(localStorage.GetItemAsString(LanguageKey));

                CultureInfo.DefaultThreadCurrentUICulture = ci;
            }

            await host.RunAsync();
        }
    }
}
