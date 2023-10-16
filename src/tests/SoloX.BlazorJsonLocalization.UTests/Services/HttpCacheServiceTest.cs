// ----------------------------------------------------------------------
// <copyright file="HttpCacheServiceTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Threading.Tasks;
using Xunit;
using SoloX.BlazorJsonLocalization.Services.Impl;
using System;
using System.Collections.Generic;
using FluentAssertions;

namespace SoloX.BlazorJsonLocalization.UTests.Services
{
    public class HttpCacheServiceTest
    {
        [Fact]
        public async Task ItShouldLoadDataMapOnlyOnce()
        {
            var map = new Dictionary<string, string>();
            var httpCacheService = new HttpCacheService();

            var uri1 = new Uri("http://host/test1");
            var nbCall = 0;

            var handler = () =>
            {
                nbCall++;
                return Task.FromResult<IReadOnlyDictionary<string, string>?>(map);
            };

            var loadedMap = await httpCacheService.ProcessLoadingTask(
                uri1,
                handler).ConfigureAwait(false);

            loadedMap.Should().NotBeNull().And.BeSameAs(map);

            var loadedMap2 = await httpCacheService.ProcessLoadingTask(
                uri1,
                handler).ConfigureAwait(false);

            loadedMap2.Should().NotBeNull().And.BeSameAs(map);
            nbCall.Should().Be(1);
        }

        [Fact]
        public async Task ItShouldReLoadIfFailed()
        {
            var map = new Dictionary<string, string>();
            var httpCacheService = new HttpCacheService();

            var uri1 = new Uri("http://host/test1");
            var nbCall = 0;

            Func<Task<IReadOnlyDictionary<string, string>?>> handler = async () =>
            {
                await Task.Yield();

                nbCall++;

                throw new NotSupportedException("Load failed...");
            };

            var action1 = async () => await httpCacheService.ProcessLoadingTask(
                    uri1,
                    handler).ConfigureAwait(false);

            await action1.Should().ThrowAsync<NotSupportedException>().ConfigureAwait(false);

            var action2 = async () => await httpCacheService.ProcessLoadingTask(
                    uri1,
                    handler).ConfigureAwait(false);

            await action2.Should().ThrowAsync<NotSupportedException>().ConfigureAwait(false);

            nbCall.Should().Be(2);
        }
    }
}
