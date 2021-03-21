// ----------------------------------------------------------------------
// <copyright file="ConstStringLocalizer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// Constant string localizer.
    /// </summary>
    public class ConstStringLocalizer : IStringLocalizer
    {
        private readonly string constValue;

        /// <summary>
        /// Setup the ConstStringLocalizer with the constant value
        /// </summary>
        /// <param name="constValue">Constant value to use.</param>
        public ConstStringLocalizer(string constValue)
        {
            this.constValue = constValue;
        }

        ///<inheritdoc/>
        public LocalizedString this[string name] => new(name, this.constValue);

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments] => new(name, this.constValue);

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Array.Empty<LocalizedString>();
        }
    }
}
