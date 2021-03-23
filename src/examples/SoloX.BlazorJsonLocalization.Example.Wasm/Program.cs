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
                        .UseEmbeddedJson(options =>
                        {
                            options.ResourcesPath = "Resources";
                        })
                        .UseHttpHostedJson(options =>
                        {
                            options.ApplicationAssembly = typeof(Program).Assembly;
                            options.ResourcesPath = "lang";
                        });
                });

            await builder.Build().RunAsync();
        }
    }
}
