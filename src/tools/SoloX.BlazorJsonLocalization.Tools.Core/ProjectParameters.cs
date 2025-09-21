// ----------------------------------------------------------------------
// <copyright file="ProjectParameters.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

namespace SoloX.BlazorJsonLocalization.Tools.Core
{
    /// <summary>
    /// Project parameters class.
    /// </summary>
    public class ProjectParameters
    {
        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="projectPath">Project path.</param>
        /// <param name="rootNamespace">Project root namespace.</param>
        public ProjectParameters(string projectPath, string rootNamespace)
        {
            ProjectPath = projectPath;
            RootNamespace = rootNamespace;
        }

        /// <summary>
        /// Get project path.
        /// </summary>
        public string ProjectPath { get; private set; }

        /// <summary>
        /// Get project root namespace.
        /// </summary>
        public string RootNamespace { get; private set; }
    }
}
