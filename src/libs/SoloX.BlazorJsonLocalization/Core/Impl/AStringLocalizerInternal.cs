// ----------------------------------------------------------------------
// <copyright file="AStringLocalizerInternal.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// String localizer internal class.
    /// </summary>
    public abstract class AStringLocalizerInternal : IStringLocalizerInternal
    {
        private static readonly Dictionary<string, IStringLocalizerInternal> IdentityMap = new Dictionary<string, IStringLocalizerInternal>();

        internal static IStringLocalizerInternal? TryGetStringLocalizerInternal(IStringLocalizer stringLocalizer)
        {
            var entry = stringLocalizer[StringLocalizerIdentityKey];
            if (entry != null)
            {
                lock (IdentityMap)
                {
                    return IdentityMap[entry.Value];
                }
            }

            return null;
        }

        internal static async ValueTask<bool> LoadAsync(IStringLocalizer localizer, bool loadParentCulture)
        {
            var localizerInternal = TryGetStringLocalizerInternal(localizer);

            if (localizerInternal == null)
            {
                // Unknown localizer implementation.
                return false;
            }

            await localizerInternal.LocalizerFactoryInternal.LoadDataThroughStringLocalizerHierarchyAsync(
                localizerInternal,
                localizerInternal.CultureInfo,
                loadParentCulture).ConfigureAwait(false);

            return true;
        }

        internal const string StringLocalizerIdentityKey = nameof(StringLocalizerIdentityKey);

        /// <summary>
        /// String localizer identity.
        /// </summary>
        protected string StringLocalizerGuid { get; }

        /// <summary>
        /// Return the default localizer.
        /// </summary>
        protected IStringLocalizerInternal DefaultLocalizer
        {
            get
            {
                if (this.defaultLocalizer == null)
                {
                    this.defaultLocalizer = LocalizerFactoryInternal.CreateDefaultStringLocalizer(ResourceSource, CultureInfo, StringLocalizerGuid);
                }
                return this.defaultLocalizer;
            }
        }

        private IStringLocalizerInternal? defaultLocalizer;

        ///<inheritdoc/>
        public abstract IStringLocalizer AsStringLocalizer { get; }

        ///<inheritdoc/>
        public StringLocalizerResourceSource ResourceSource { get; }

        ///<inheritdoc/>
        public CultureInfo CultureInfo { get; }

        ///<inheritdoc/>
        public IJsonStringLocalizerFactoryInternal LocalizerFactoryInternal { get; }

        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="resourceSource"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="localizerFactoryInternal"></param>
        /// <param name="stringLocalizerGuid">String localizer identity Id.</param>
        protected AStringLocalizerInternal(StringLocalizerResourceSource resourceSource, CultureInfo cultureInfo, IJsonStringLocalizerFactoryInternal localizerFactoryInternal, string? stringLocalizerGuid)
        {
            ResourceSource = resourceSource;
            CultureInfo = cultureInfo;
            LocalizerFactoryInternal = localizerFactoryInternal ?? throw new ArgumentNullException(nameof(localizerFactoryInternal));

            if (stringLocalizerGuid == null)
            {
                StringLocalizerGuid = Guid.NewGuid().ToString();

                lock (IdentityMap)
                {
                    IdentityMap.Add(StringLocalizerGuid, this);
                }
            }
            else
            {
                StringLocalizerGuid = stringLocalizerGuid;
            }
        }

        ///<inheritdoc/>
        public virtual Task<bool> LoadDataAsync()
        {
            return Task.FromResult(true);
        }

        ///<inheritdoc/>
        public LocalizedString? TryGet(string name)
        {
            if (StringLocalizerIdentityKey == name)
            {
                return new LocalizedString(StringLocalizerIdentityKey, StringLocalizerGuid, false);
            }
            else
            {
                return TryGetInternal(name);
            }
        }

        /// <summary>
        /// Try get the LocalizedString.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The localized string or null</returns>
        protected abstract LocalizedString? TryGetInternal(string name);

        ///<inheritdoc/>
        public abstract LocalizedString? TryGet(string name, object[] arguments, CultureInfo requestedCultureInfo);
    }
}
