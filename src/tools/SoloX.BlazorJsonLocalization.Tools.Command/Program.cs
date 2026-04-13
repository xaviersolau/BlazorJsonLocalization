// ----------------------------------------------------------------------
// <copyright file="Program.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

namespace SoloX.BlazorJsonLocalization.Tools
{
    /// <summary>
    /// CLI Tools Generator.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Run the tools command.
        /// </summary>
        /// <returns>Error code.</returns>
        public static async Task<int> Main(string[] args)
        {
            var generatorCommand = new GeneratorCommand();

            return await generatorCommand.RunGeneratorCommandAsync(args).ConfigureAwait(false);
        }
    }
}
