# Breaking Changes

## From version 1.0.* to 2.0.*

Change `AssemblyNames` property to `Assemblies` in `SoloX.BlazorJsonLocalization.Core.AExtensionOptions` (base class for
`EmbeddedJsonLocalizationOptions` and `HttpHostedJsonLocalizationOptions`)

Before:
```csharp
builder.UseHttpHostedJson(options =>
    {
        options.AssemblyNames = new[] { typeof(ComponentsStaticAssetsExtensions).Assembly.GetName().Name };
        // ...
    });
```

After:
```csharp
builder.UseHttpHostedJson(options =>
    {
        options.Assemblies = new[] { typeof(ComponentsStaticAssetsExtensions).Assembly };
        // ...
    });
```
