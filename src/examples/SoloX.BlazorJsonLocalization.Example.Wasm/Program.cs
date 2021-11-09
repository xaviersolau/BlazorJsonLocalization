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

namespace SoloX.BlazorJsonLocalization.Example.Wasm
{
    public class Program
    {
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
                            options.ResourcesPath = "Resources";
                        });
                });

            builder.Services.AddBlazoredLocalStorage();

            await builder.Build().RunAsync();
        }
    }
}
