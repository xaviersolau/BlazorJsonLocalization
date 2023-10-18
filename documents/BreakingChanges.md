# Breaking Changes

## From version 1.0.* to 2.0.*

### Rename SoloX.BlazorJsonLocalization.Core.AExtensionOptions.Assemblies property.

Change `AssemblyNames` property to `Assemblies` in `SoloX.BlazorJsonLocalization.Core.AExtensionOptions` (base
class for `EmbeddedJsonLocalizationOptions` and `HttpHostedJsonLocalizationOptions`)

Before:
```csharp
builder.UseHttpHostedJson(options =>
    {
        options.AssemblyNames = new[]
        {
            typeof(ComponentsStaticAssetsExtensions).Assembly.GetName().Name
        };
        // ...
    });
```

After:
```csharp
builder.UseHttpHostedJson(options =>
    {
        options.Assemblies = new[]
        {
            typeof(ComponentsStaticAssetsExtensions).Assembly
        };
        // ...
    });
```

### Resource culture name suffix file separator.

The default resource file naming policy was using a `-` char a culture name suffix separator. Now the default
separator is a `.`.

Before:
```code
MyComponent-fr-FR.json
```

After:
```code
MyComponent.fr-FR.json
```

### Use of Logger.

The use of logger when loading messages is now disabled by default. The logs can be enabled using the
configuration builder method `EnableLogger`.
