
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SoloX.BlazorJsonLocalization.Example.WebApp.Client;
using SoloX.BlazorJsonLocalization.WebAssembly;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp =>
    new HttpClient
    {
       BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

builder.Services.AddWebAssemblyJsonLocalization(b =>
{
    b
#if DEBUG
        .EnableLogger()
#endif
        .AddFallback("Fallback", typeof(_Imports).Assembly)
        .UseHttpHostedJson(options =>
        {
            options.ResourcesPath = "Resources";
            options.NamingPolicy = HttpHostedJsonNamingPolicy;
            options.ApplicationAssembly = typeof(_Imports).Assembly;
        });
});

await builder.Build().RunAsync();


static Uri HttpHostedJsonNamingPolicy(string basePath, string cultureName)
{
    return string.IsNullOrEmpty(cultureName)
        ? new Uri($"{basePath}.json?v=1.0.0", UriKind.Relative)
        : new Uri($"{basePath}.{cultureName}.json?v=1.0.0", UriKind.Relative);
}