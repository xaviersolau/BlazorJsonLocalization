using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SoloX.BlazorJsonLocalization.Generator.Sample.Components;

namespace SoloX.BlazorJsonLocalization.Generator.Sample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging(b => b.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Warning));

            services.AddJsonLocalization(
                builder => builder.UseEmbeddedJson(
                    options => options.ResourcesPath = "Resources"));

            using var resolver = services.BuildServiceProvider();

            var logger = resolver.GetRequiredService<ILogger<Program>>();

            var localizer = resolver.GetRequiredService<IStringLocalizer<MyComponent>>();

            var typedLocalizer = localizer.ToMyComponentStringLocalizer();

            logger.LogInformation("typedLocalizer.MyProperty: " + typedLocalizer.MyProperty);
            logger.LogInformation("typedLocalizer.SubLocalizer.SubProperty: " + typedLocalizer.SubLocalizer.SubProperty);
            logger.LogInformation("typedLocalizer.MyStringWithArgs: " + typedLocalizer.MyStringWithArgs(5, "five"));
        }
    }
}
