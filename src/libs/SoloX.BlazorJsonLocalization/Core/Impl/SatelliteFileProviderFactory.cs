// ----------------------------------------------------------------------
// <copyright file="SatelliteFileProviderFactory.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// File provider factory that handle satelite culture assembly.
    /// </summary>
    public class SatelliteFileProviderFactory
    {
        private readonly string rootNameSpace;

        /// <summary>
        /// Base assembly.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Setup factory.
        /// </summary>
        /// <param name="assembly">Base assembly to get satellite assembly from.</param>
        /// <param name="rootNameSpace">Root name space.</param>
        public SatelliteFileProviderFactory(Assembly assembly, string rootNameSpace)
        {
            Assembly = assembly;
            this.rootNameSpace = rootNameSpace;
        }

        /// <summary>
        /// Create file provider for the given culture.
        /// </summary>
        /// <param name="cultureInfo">Culture info to use.</param>
        /// <returns>The file provider to get resources from satellite assembly.</returns>
        public IFileProvider GetFileProvider(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            var providerMap = new List<KeyValuePair<string, EmbeddedFileProvider>>();

            var ci = cultureInfo;

            while (ci != ci.Parent)
            {
                var sa = TryGetSatelliteAssembly(Assembly, ci);
                if (sa != null)
                {
                    providerMap.Add(
                        new KeyValuePair<string, EmbeddedFileProvider>(
                            ci.Name,
                            new EmbeddedFileProvider(sa, this.rootNameSpace)));
                }
                ci = ci.Parent;
            }

            if (providerMap.Any())
            {
                providerMap.Add(
                    new KeyValuePair<string, EmbeddedFileProvider>(
                        string.Empty,
                        new EmbeddedFileProvider(Assembly, this.rootNameSpace)));

                return new SatelliteFileProvider(providerMap);
            }
            else
            {
                return new EmbeddedFileProvider(Assembly, this.rootNameSpace);
            }
        }

        private static Assembly? TryGetSatelliteAssembly(Assembly assembly, CultureInfo cultureInfo)
        {
            try
            {
                var satelliteAssembly = assembly.GetSatelliteAssembly(cultureInfo);

                return satelliteAssembly;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        private class SatelliteFileProvider : IFileProvider
        {
            private readonly List<KeyValuePair<string, EmbeddedFileProvider>> providerMap;

            public SatelliteFileProvider(List<KeyValuePair<string, EmbeddedFileProvider>> providerMap)
            {
                this.providerMap = providerMap;
            }

            public IDirectoryContents GetDirectoryContents(string subpath)
            {
                throw new NotImplementedException();
            }

            public IFileInfo GetFileInfo(string subpath)
            {
                foreach (var provider in this.providerMap)
                {
                    if (string.IsNullOrEmpty(provider.Key))
                    {
                        return provider.Value.GetFileInfo(subpath);
                    }
                    else if (subpath.Contains($".{provider.Key}.", StringComparison.Ordinal)
                        || subpath.EndsWith($".{provider.Key}", StringComparison.Ordinal))
                    {
                        var res = subpath.Replace($".{provider.Key}", string.Empty, StringComparison.Ordinal);
                        return provider.Value.GetFileInfo(res);
                    }
                }

                throw new FileNotFoundException(subpath);
            }

            public IChangeToken Watch(string filter)
            {
                throw new NotSupportedException();
            }
        }
    }
}