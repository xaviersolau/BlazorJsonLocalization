
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SoloX.BlazorJsonLocalization.Example.WebApp.Client;
using SoloX.BlazorJsonLocalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient(
    "Host",
    (sp, c) =>
    {
        var hostEnv = sp.GetRequiredService<IWebAssemblyHostEnvironment>();
        c.BaseAddress = new Uri(hostEnv.BaseAddress);
    });

//builder.Services.AddScoped(sp =>
//    new HttpClient
//    {
//        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
//    });

builder.Services.AddJsonLocalization(b =>
{
    b
#if DEBUG
        .EnableLogger()
#endif
        .AddFallback("Fallback", typeof(_Imports).Assembly)
        .UseHttpClientJson(options =>
        {
            options.ResourcesPath = "Resources";
            options.NamingPolicy = HttpHostedJsonNamingPolicy;
            options.ApplicationAssembly = typeof(_Imports).Assembly;
            options.HttpClientBuilder = sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                return httpClientFactory.CreateClient("Host");
            };
        });
});

await builder.Build().RunAsync();


static Uri HttpHostedJsonNamingPolicy(string basePath, string cultureName)
{
    return string.IsNullOrEmpty(cultureName)
        ? new Uri($"{basePath}.json?v=1.0.0", UriKind.Relative)
        : new Uri($"{basePath}.{cultureName}.json?v=1.0.0", UriKind.Relative);
}