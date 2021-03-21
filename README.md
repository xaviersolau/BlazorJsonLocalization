# BlazorJsonLocalization

[![Build - CI](https://github.com/xaviersolau/BlazorJsonLocalization/actions/workflows/build-ci.yml/badge.svg)](https://github.com/xaviersolau/BlazorJsonLocalization/actions/workflows/build-ci.yml)
[![Coverage Status](https://coveralls.io/repos/github/xaviersolau/BlazorJsonLocalization/badge.svg?branch=main)](https://coveralls.io/github/xaviersolau/BlazorJsonLocalization?branch=main)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![NuGet Beta](https://img.shields.io/nuget/vpre/SoloX.BlazorJsonLocalization.svg)](https://www.nuget.org/packages/SoloX.BlazorJsonLocalization)

Provides JSON-based Blazor localization support. 


Don't hesitate to post issue, pull request on the project or to fork and improve the project.

## License and credits

BlazorJsonLocalization project is written by Xavier Solau. It's licensed under the MIT license.

 * * *

## Installation

You can checkout this Github repository or you can use the NuGet package:

**Install using the command line from the Package Manager:**
```bash
Install-Package SoloX.BlazorJsonLocalization -version 1.0.0-alpha.3
```

**Install using the .Net CLI:**
```bash
dotnet add package SoloX.BlazorJsonLocalization --version 1.0.0-alpha.3
```

**Install editing your project file (csproj):**
```xml
<PackageReference Include="SoloX.BlazorJsonLocalization" Version="1.0.0-alpha.3" />
```

## How to use it

Note that you can find code examples in this repository in this location: `src/examples`.

### Setup the localizer using embedded Json resources file

#### Blazor Wasm Version

A few lines of code are actually needed to setup the BlazorJsonLocalizer.
You just need to use the name space `SoloX.BlazorJsonLocalization` to get access to
right extension methods and to setup the services in you `Program.cs` file :

```csharp
// Here we are going to store the Json files in the project 'Resources' folder.
builder.Services.AddJsonLocalization(
    builder => builder.UseEmbeddedJson(
        options => options.ResourcesPath = "Resources"));
```

Let's say that we are going to use the localization in the `Index` page. The first thing to do
is to create your Json resource files that are actually going to contain the translated text.
You will add the Json files in the project "Resources" folder (since we have defined it in the
options `ResourcesPath` property).

The file must be named with the component name (In our case `Index`) and suffixed with the
ISO2 language code:

| File name     | Description              |
|---------------|--------------------------|
| Index-fr.json | French translated text.  |
| Index-de.json | German translated text.  |
| Index-en.json | English translated text. |
| Index.json    | Since there is no language code, this file is going to be used as fallback when the language is unknown. |

For example the English Json file will look like this:

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

The Json file need to be declared as *Embedded resources* in order to be shipped in
the Assembly. You can do it this way in your csproj file:

```xml
  <ItemGroup>
    <Content Remove="Resources\Index-fr.json" />
    <!-- ... -->
    <Content Remove="Resources\Index.json" />
  </ItemGroup>

  <ItemGroup>
    <EmbeddedResource Include="Resources\Index-fr.json" />
    <!-- ... -->
    <EmbeddedResource Include="Resources\Index.json" />
  </ItemGroup>
```

Then you will need to inject the `IStringLocalizer<Index>` in the Index class:

```csharp
[Inject]
private IStringLocalizer<Index> L { get; set; }
```

or using the razor syntax:

```razor
@inject IStringLocalizer<Index> L
```

with the using declared in your `_Imorts.razor`:

```razor
@using Microsoft.Extensions.Localization
```

Once the localizer is available you can just use it like this:

```razor
@page "/"

<h1>@L["Hello"]</h1>

@L["Welcome"]

```

