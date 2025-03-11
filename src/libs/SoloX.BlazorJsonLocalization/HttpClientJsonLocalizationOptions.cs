// ----------------------------------------------------------------------
// <copyright file="HttpClientJsonLocalizationOptions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Net.Http;

namespace SoloX.BlazorJsonLocalization
{
    /// <summary>
    /// JsonLocalizer Http Client extension options
    /// </summary>
    public class HttpClientJsonLocalizationOptions : HttpHostedJsonLocalizationOptions
    {
        /// <summary>
        /// HttpClient builder handler.
        /// </summary>
        /// <param name="serviceProvider">Service provider to get the HttpClient from.</param>
        /// <returns>HttpClient to use to get Json localizer resources.</returns>
        public delegate HttpClient HttpClientBuilderHandler(IServiceProvider serviceProvider);

        /// <summary>
        /// Optional HttpClient builder.
        /// </summary>
        public HttpClientBuilderHandler? HttpClientBuilder { get; set; }

        /// <summary>
        /// Tells if the HttpClient provided by the HttpClientBuilder needs to be disposed.
        /// </summary>
        public bool IsDisposeHttpClientFromBuilderEnabled { get; set; } = true;
    }
}
