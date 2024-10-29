using SoloX.BlazorJsonLocalization.Example.WebApp.Components;
using SoloX.BlazorJsonLocalization.ServerSide;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddServerSideJsonLocalization(builder =>
{
    builder
#if DEBUG
        .EnableLogger()
#endif
        .UseHttpHostedJson(options =>
        {
            options.ResourcesPath = "Resources";
            options.ApplicationAssemblies = [typeof(_Imports).Assembly, typeof(SoloX.BlazorJsonLocalization.Example.WebApp.Client._Imports).Assembly];
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SoloX.BlazorJsonLocalization.Example.WebApp.Client._Imports).Assembly);

app.Run();
