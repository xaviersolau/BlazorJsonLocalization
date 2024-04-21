
# LocalizationGen Tool User Documentation

## Introduction

Welcome to the LocalizationGen tool user documentation!
This tool is designed to simplify the localization process. All you need it to write a localization interface and
the tool will assist you by generating interface implementations and associated JSON localization resources for your
.NET projects.
Whether you're working on Blazor Web Apps or other dotnet projects requiring localization, LocalizationGen
has got you covered.

## Installation

To get started with LocalizationGen, you'll need to install it as a dotnet tool. Here's how to install making it globally available on your computer :

```bash
dotnet tool install SoloX.BlazorJsonLocalization.Tools.Command -g
```

Once installed, you'll have access to the LocalizationGen command-line interface (CLI) tool.

```bash
dotnet localizationgen
```

You can also install the tool locally to your solution.

First add the tool manifest if not already installed:

```bash
dotnet new tool-manifest
```

Then register the tool within your solution:

```bash
dotnet tool install SoloX.BlazorJsonLocalization.Tools.Command
```

## Usage

Generating Localization Resources
To generate interface implementations and JSON localization resources for your project, use the following command:

```bash
dotnet localizationgen <path-to-your-project.csproj>
```

Replace `<path-to-your-project.csproj>` with the path to your project file.

## Example
Let's say your project is located in a directory named MyWebApp and your project file is named MyWebApp.csproj.

Write your localization interface defining all messages you want to use in your application:

```csharp
using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;

namespace MyWebApp.Localizer
{
  /// <summary>
  /// The component class.
  /// In this example it is an empty class but instead you can use directly
  /// a razor component.
  /// </summary>
  public class ServerGlobal
  {
  }

  /// <summary>
  /// Here, we use the Localizer attribute to tell the LocalizationGen tool
  /// that we need the Json file stored in the wwwroot folder and that we
  /// want the fr-FR and en-UK files in addition of the default Json file.
  /// </summary>
  [Localizer("wwwroot", ["fr-FR", "en-UK"])]
  public interface IServerGlobalStringLocalizer :
    IStringLocalizer<ServerGlobal>
  {
    /// <summary>
    /// Let's define some properties to access the localized values.
    /// The optional Translate attribute allows to define a default
    /// translation for the localized entry.
    /// </summary>

    [Translate("Home")]
    string HomePageTitle { get; }

    [Translate("Hello, world!")]
    string HelloTitle { get; }

    [Translate("Welcome to your new app.")]
    string HelloBody { get; }
  }
}
```

To generate localization resources for this project, you would run:

```bash
dotnet localizationgen MyWebApp/MyWebApp.csproj
```

## Result

LocalizationGen will generate the following files:

Interface implementation files (e.g., ServerGlobalStringLocalizer.g.cs, ServerGlobalStringLocalizerExtensions.g.cs)
JSON localization files (e.g., ServerGlobal.json, ServerGlobal.fr-FR.json, etc.)

## Help

For detailed usage information and available options, you can use the --help option with the LocalizationGen command:

```bash
dotnet localizationgen --help
```

## resources

Please find a full working example of a Blazor Web App using the tool:

https://github.com/xaviersolau/DevArticles/tree/code_first_json_localization

And the step by step guide to setup the project:

https://medium.com/younited-tech-blog/code-first-json-localization-on-blazor-web-app-a1b18352dd67