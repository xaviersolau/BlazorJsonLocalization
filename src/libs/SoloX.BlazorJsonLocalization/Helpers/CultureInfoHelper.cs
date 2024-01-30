// ----------------------------------------------------------------------
// <copyright file="CultureInfoHelper.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading.Tasks;

#if !NET6_0_OR_GREATER
using ArgumentNullException = SoloX.BlazorJsonLocalization.Helpers.ArgumentNullExceptionLegacy;
#endif

namespace SoloX.BlazorJsonLocalization.Helpers
{
    /// <summary>
    /// Culture info helpers.
    /// </summary>
    public static class CultureInfoHelper
    {
        /// <summary>
        /// Walk though CultureInfo Parents and try to load data. Stop as soon as Data is not null.
        /// </summary>
        /// <typeparam name="TData">Data type to load.</typeparam>
        /// <param name="cultureInfo">The start CultureInfo to walk from.</param>
        /// <param name="loadDataAsync">Asynchronous data loader.</param>
        /// <returns>The loaded data or null if not fund/loaded.</returns>
        public static async ValueTask<TData?> WalkThoughCultureInfoParentsAsync<TData>(CultureInfo cultureInfo, Func<CultureInfo, Task<TData?>> loadDataAsync) where TData : class
        {
            ArgumentNullException.ThrowIfNull(cultureInfo, nameof(cultureInfo));
            ArgumentNullException.ThrowIfNull(loadDataAsync, nameof(loadDataAsync));

            TData? data;
            bool done;

            do
            {
                data = await loadDataAsync(cultureInfo).ConfigureAwait(false);

                done = data != null
                    || ReferenceEquals(cultureInfo.Parent, cultureInfo);

                cultureInfo = cultureInfo.Parent;
            }
            while (!done);

            return data;
        }
    }
}
