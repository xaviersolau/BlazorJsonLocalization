# BlazorJsonLocalization

This project provides JSON-based Blazor localization support. It is a extension of the standard
**Microsoft.Extensions.Localization** package.

Json files can be embedded in the assemblies or stay as static assets on the HTTP host side.

Don't hesitate to post issues, pull requests on the project or to fork and improve the project.

## Project dashboard

[![Build - CI](https://github.com/xaviersolau/BlazorJsonLocalization/actions/workflows/build-ci.yml/badge.svg)](https://github.com/xaviersolau/BlazorJsonLocalization/actions/workflows/build-ci.yml)
[![Coverage Status](https://coveralls.io/repos/github/xaviersolau/BlazorJsonLocalization/badge.svg?branch=main)](https://coveralls.io/github/xaviersolau/BlazorJsonLocalization?branch=main)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

| Package                                    | Nuget.org | Pre-release |
|--------------------------------------------|-----------|-----------|
|**SoloX.BlazorJsonLocalization**            |[![NuGet Beta](https://img.shields.io/nuget/v/SoloX.BlazorJsonLocalization.svg)](https://www.nuget.org/packages/SoloX.BlazorJsonLocalization)|[![NuGet Beta](https://img.shields.io/nuget/vpre/SoloX.BlazorJsonLocalization.svg)](https://www.nuget.org/packages/SoloX.BlazorJsonLocalization)|
|**SoloX.BlazorJsonLocalization.WebAssembly**|[![NuGet Beta](https://img.shields.io/nuget/v/SoloX.BlazorJsonLocalization.WebAssembly.svg)](https://www.nuget.org/packages/SoloX.BlazorJsonLocalization.WebAssembly)|[![NuGet Beta](https://img.shields.io/nuget/vpre/SoloX.BlazorJsonLocalization.svg)](https://www.nuget.org/packages/SoloX.BlazorJsonLocalization)|
|**SoloX.BlazorJsonLocalization.ServerSide** |[![NuGet Beta](https://img.shields.io/nuget/v/SoloX.BlazorJsonLocalization.ServerSide.svg)](https://www.nuget.org/packages/SoloX.BlazorJsonLocalization.ServerSide)|[![NuGet Beta](https://img.shields.io/nuget/vpre/SoloX.BlazorJsonLocalization.svg)](https://www.nuget.org/packages/SoloX.BlazorJsonLocalization)|

## License and credits

BlazorJsonLocalization project is written by Xavier Solau. It's licensed under the MIT license.

 * * *

## Features

* Enable Json based localization with embedded Json files.
* Enable Json based localization with hosted Json files.
* Assembly scoped localizer support.
* Structured Json support with complex objects.
* Localizer inheritance.
* Localizer fall back.
* Both WebAssembly and Server Side support.

## Installation

You can checkout this Github repository or you can use the NuGet packages:

**Install using the command line from the Package Manager:**
```bash
Install-Package SoloX.BlazorJsonLocalization -version 2.0.0-alpha.3
Install-Package SoloX.BlazorJsonLocalization.WebAssembly -version 2.0.0-alpha.3
Install-Package SoloX.BlazorJsonLocalization.ServerSide -version 2.0.0-alpha.3
```

**Install using the .Net CLI:**
```bash
dotnet add package SoloX.BlazorJsonLocalization --version 2.0.0-alpha.3
dotnet add package SoloX.BlazorJsonLocalization.WebAssembly --version 2.0.0-alpha.3
dotnet add package SoloX.BlazorJsonLocalization.ServerSide --version 2.0.0-alpha.3
```

**Install editing your project file (csproj):**
```xml
<PackageReference Include="SoloX.BlazorJsonLocalization" Version="2.0.0-alpha.3" />
<PackageReference Include="SoloX.BlazorJsonLocalization.WebAssembly" Version="2.0.0-alpha.3" />
<PackageReference Include="SoloX.BlazorJsonLocalization.ServerSide" Version="2.0.0-alpha.3" />
```

> Find out the [Breaking changes](documents/BreakingChanges.md) from one version to another.

## How to use it

Note that you can find code examples in this repository at this location: `src/examples`.

### Set up the localizer using embedded Json resources files

If you are going to embed your Json files in your Assemblies, you just need the
**SoloX.BlazorJsonLocalization** package to enable the localization support.

#### Set up the dependency injection

A few lines of code are actually needed to setup the BlazorJsonLocalizer.
You just need to use the name space `SoloX.BlazorJsonLocalization` to get access to
the right extension methods and to add the services in your `ServiceCollection` :

* For Blazor WebAssembly:

Update your `Main` method in the `Program.cs` file:

```csharp
// Here we are going to store the Json files in the project 'Resources' folder.
builder.Services.AddJsonLocalization(
    builder => builder.UseEmbeddedJson(
        options => options.ResourcesPath = "Resources"));
```

By default the current culture is going to be taken from the browser settings. But it can
be easily set by code with this simple code:

```csharp
var cultureInfo = CultureInfo.GetCultureInfo(name);
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
```

You can find an example in the project repository in `src/examples/SoloX.BlazorJsonLocalization.Example.Wasm`.

* For Blazor Server Side:

First add the `using Microsoft.AspNetCore.Localization` and `using SoloX.BlazorJsonLocalization` directives then update your
`ConfigureServices` method in the `Startup.cs` file:

```csharp
// Here we are going to store the Json files in the project 'Resources' folder.
services.AddJsonLocalization(
    builder => builder.UseEmbeddedJson(
        options => options.ResourcesPath = "Resources"),
        ServiceLifetime.Singleton);

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
```

Note that it is also possible to provide custom `JsonSerializerOptions` in the `JsonSerializerOptions` options property.

```csharp
// Here we are going to store the Json files in the project 'Resources' folder
// and we provide custom JsonSerializerOptions.
services.AddJsonLocalization(
    builder => builder.UseEmbeddedJson(
        options =>
        {
            options.ResourcesPath = "Resources";
            options.JsonSerializerOptions = yourCustomOptions;
        }),
        ServiceLifetime.Singleton);
```


And update your `Configure` method in the Startup.cs file:

```csharp
// AspNet core standard localization setup to get culture from the
// Browser
app.UseRequestLocalization();
```

With this configuration, you will be able to get the culture from the browser. If you want to
define the culture by code you can go to the Microsoft documentation:
[Provide UI to choose the culture](https://docs.microsoft.com/fr-fr/aspnet/core/blazor/globalization-localization?view=aspnetcore-5.0#provide-ui-to-choose-the-culture).
You will find a nice solution for this purpose.
In addition you can find an example in the BlazorJsonLocalizer project repository in
`src/examples/SoloX.BlazorJsonLocalization.Example.ServerSide`.

#### Enable localization support in your component

Let's say that we are going to use the localization in the `Index` page. The first thing to do
is to create your Json resource files that are actually going to contain the translated text.
You will add the Json files in the project "Resources" folder (since we have defined it in the
options `ResourcesPath` property).
The folders hierarchy is also important to avoid name collision so if your component is in a
sub-folder your Json files must also be in the same sub-folder.

> For example if your `Index` page is in the `Pages` sub-folder, your Index Json files must
> be in the same sub-folder: `Resources/Pages` with `Resources` the resources path defined
> in the options.

##### Write your localization Json files

The file must be named with the component name (In our case `Index`) and suffixed with `CultureInfo` name or the
ISO2 language code:

| File name     | Description              |
|---------------|--------------------------|
| Index-fr.json | French translated text.  |
| Index-de.json | German translated text.  |
| Index-en.json | English translated text. |
| Index.json    | Since there is no language code, this file is going to be used as fall back when the language is unknown. |

> Note that if you want to get the localization resources for your
> component `MyComponent` and for the French `CultureInfo` (fr-FR) the
> factory will first try to load `MyComponent-fr-FR.json`. If the file
> is not found, it will try to load `MyComponent-fr.json` and finally
> if the file is still not found, it will fall back to the file `MyComponent.json`

The content of the file is using a conventional Json syntax for example
the English Json file will look like this:

```json
{
  "Hello": "Hello world!",
  "Welcome": "Welcome to your new app."
}
```

and the French file:

```json
{
  "Hello": "Bonjour tout le monde!",
  "Welcome": "Bienvenue dans votre nouvelle application."
}
```

> **Warning: the Json file should use UTF8 encoding in order to easily handle accents or specific character set.**

The Json file need to be declared as *Embedded resources* in order to be shipped in
the Assembly. You can do it this way in your csproj file specifying every json file manually:

```xml
  <ItemGroup>
    <EmbeddedResource Include="Resources\Pages\Index-fr.json" />
    <EmbeddedResource Include="Resources\Pages\Index.json" />
  </ItemGroup>
```

Or using the wildcards version:

```xml
  <ItemGroup>
    <EmbeddedResource Include="Resources\**\*.json" />
  </ItemGroup>
```


##### Use the localization in your code

You will need to inject the `IStringLocalizer<Index>` in the `Index` class:

```csharp
[Inject]
private IStringLocalizer<Index> L { get; set; }
```

or using the razor syntax in the `Index.razor` file:

```razor
@inject IStringLocalizer<Index> L
```

with the `using` declared in your `_Imorts.razor`:

```razor
@using Microsoft.Extensions.Localization
```

Once the localizer is available you can just use it like this in the `Index.razor` file:

```razor
@page "/"

<h1>@L["Hello"]</h1>

@L["Welcome"]
```

##### Structured Json support

As we have seen before, we can store key / value peer in the Json file but you also can use structured Json. For instance
you can associate to a given key a sub-json structure like this:

```json
{
  "Notification": {
    "Message": "Some notification!",
    "Description": "My notification description!"
  }
}
```

To access a complex object, you can directly use a key path (separated with ':'). In this case it is possible to use the
IStringLocalizer with the key "Notification:Message" to get the corresponding value: "Some notification!".

```csharp
// Import extension methods (Get, GetSubLocalizer)
using SoloX.BlazorJsonLocalization;

[Inject]
IStringLocalizer<Index> Localizer { get; set; }

// Access with the key path:
var txt = Localizer["Notification:Message"];

// Or preferably using the Key.Path method.
var txt = Localizer[Key.Path("Notification", "Message")];
```

That said, we can use an other way to access structured object: the GetSubLocalizer extension method:

```csharp

// Access using a sub-localizer
var subLocalizer = Localizer.GetSubLocalizer("Notification");
var txt = subLocalizer["Message"];
```

The sub-localizer can be be used as if it would contain only the structured object:
```json
{
  "Message": "Some notification!",
  "Description": "My notification description!"
}
```

### Set up the localizer using Json static assets on the HTTP host side

Basically you need to do almost the same as above except for the following points.

#### Json resource files as static assets

Your resource files are not embedded any more and they are located in the `wwwroot` folder
of your project with the same naming rules.

#### Dependency injection in Blazor WebAssembly

In addition of the **SoloX.BlazorJsonLocalization** package you will need the
**SoloX.BlazorJsonLocalization.WebAssembly** extension package.

The registration of your services will look like this in the `Program.cs` file:

```csharp
// Here we are going to store the Json files in the project 'wwwroot/Resources' folder.
builder.Services.AddWebAssemblyJsonLocalization(
    builder => builder.UseHttpHostedJson(
        options =>
        {
            options.ApplicationAssembly = typeof(Program).Assembly;
            options.ResourcesPath = "Resources";
        }));
```

> Note that naming your static asset file, you can change the expected file name by using
> the `NamingPolicy` delegate. So instead of naming your file `MyComponent-fr.json` you can customize it
> like this `MyComponent.fr.json?v=1.0.0.1` just by setting the `NamingPolicy` delegate to something like:
> ```csharp
> // Here we are going to change the culture separator to a dot `.` instead of a `-` 
> // and add a version querystring to help with cache busting
> builder.Services.AddWebAssemblyJsonLocalization(
>     builder => builder.UseHttpHostedJson(
>         options =>
>         {
>             options.ApplicationAssembly = typeof(Program).Assembly;
>             options.ResourcesPath = "Resources";
>             options.NamingPolicy =  
>                 (basePath, cultureName) => string.IsNullOrEmpty(cultureName)
>                     ? new Uri($"{basePath}.json?v=1.0.0.1", UriKind.Relative)
>                     : new Uri($"{basePath}.{cultureName}.json?v=1.0.0.1", UriKind.Relative);
>         }));
> ```

#### Dependency injection in Blazor Server Side

In addition of the **SoloX.BlazorJsonLocalization** package you will need the
**SoloX.BlazorJsonLocalization.ServerSide** extension package.

The registration of your services will look like this in your `ConfigureServices` method in the
`Startup.cs` file:

```csharp
// Here we are going to store the Json files in the project 'wwwroot/Resources' folder.
services.AddServerSideJsonLocalization(
    builder => builder.UseHttpHostedJson(
        options =>
        {
            options.ApplicationAssembly = typeof(Program).Assembly;
            options.ResourcesPath = "Resources";
        }));
```

#### Make sure your Json resources are actually loaded

Since the Json resources are static assets, they need to be loaded through HTTP to be used in your
components. Thanks to a cache system, this will occur only once for each of your components but it
may be useful to override the `OnInitializedAsync` method with a `L.LoadAsync()` like this to make
sure your refresh your localized text once the localization data are available:

```csharp
[Inject]
private IStringLocalizer<Index> L { get; set; }

protected override async Task OnInitializedAsync()
{
    await L.LoadAsync();
    await base.OnInitializedAsync();
}
```

> Note that as long as the translation resources are not loaded the IStringLocalizer will always return ``...``
> unless you use the ``EnableDisplayKeysWhileLoadingAsync`` method in the ``JsonLocalizationOptionsBuilder``.


### Inheritance and fall back

Let's say that we have a localizer `Global` with some basic and reusable string, we have
several options to use it.

The Global class:
```csharp
public class Global
{
}
```

With its Json resource file:
```json
{
  "GlobalKey": "This is global message..."
}
```

#### Injecting the Global localizer

The first option is to inject the localizer in your component
and to use it directly alongside of the component specific localizer.

```csharp
[Inject]
private IStringLocalizer<Global> Localizer { get; set; }
```

#### Using inheritance

A second option is to use inheritance. Your component specific localizer can inherit `Global` in
order to access the Global localization strings directly using you component localizer.

Here is an example with a `Specific` localizer:

```csharp
public class Specific : Global
{
}

[Inject]
private IStringLocalizer<Specific> Localizer { get; set; }

// Use of the GlobalKey will return the message from the Global localizer.
var txt = Localizer["GlobalKey"]
```

#### Using a fall back

Finally you can setup a fall back localizer in the localization options to use the fall back
localizer any time where a message key is not found in a specific Localizer.

Here is an example that set up a fall back to the Fallback Json files defined in a given assembly.

```csharp
builder.Services.AddWebAssemblyJsonLocalization(
    optionBuilder =>
    {
        optionBuilder
            // Add a localization fallback.
            .AddFallback("Fallback", assemblyWhereFallbackJsonFilesAreDefined);
    });
```

### Scope Localizer to a specific Assembly

You may need to set up a localizer differently depending on the assembly. For example you may
be using embedded Json resources in an assembly that contains some components and Http hosted
Json resources in an other assembly.

In this case, you can set up the localization options for each assembly using the `AssemblyNames`
property in the localizer options object.

```csharp
builder.Services.AddWebAssemblyJsonLocalization(
    builder =>
    {
        builder
            .UseEmbeddedJson(options =>
            {
                options.AssemblyNames = new[] { AssemblyWithEmbeddedJson.GetName().Name };
                //...
            })
            .UseHttpHostedJson(options =>
            {
                options.AssemblyNames = new[] { AssemblyWithHttpHostedJson.GetName().Name };
                //...
            });
    });
```
