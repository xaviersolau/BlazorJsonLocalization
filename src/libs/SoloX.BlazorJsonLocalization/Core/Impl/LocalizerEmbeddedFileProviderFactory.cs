using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    public class LocalizerEmbeddedFileProviderFactory : ILocalizerFileProviderFactory
    {
        public IFileProvider GetFileProvider(Assembly assembly)
        {
            return new EmbeddedFileProvider(assembly);
        }
    }
}
