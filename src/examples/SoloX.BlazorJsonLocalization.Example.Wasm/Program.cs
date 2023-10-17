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
using SoloX.BlazorJsonLocalization.Http;
using SoloX.BlazorJsonLocalization.Example.Components.Embedded;
using SoloX.BlazorJsonLocalization.Example.Components.StaticAssets;
using SoloX.BlazorJsonLocalization.Example.Components.SharedLocalization;
using An.Other.Name.Embedded;
using SoloX.BlazorJsonLocalization.Example.Components.Embedded3;

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
