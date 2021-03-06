﻿// ----------------------------------------------------------------------
// <copyright file="MyExtensionService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.UTests.Samples.Extension
{
    public class MyExtensionService : IJsonLocalizationExtensionService<MyOptions>
    {
        public ValueTask<IReadOnlyDictionary<string, string>> TryLoadAsync(
            MyOptions options,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
