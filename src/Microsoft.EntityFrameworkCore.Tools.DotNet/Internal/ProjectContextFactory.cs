// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.DotNet.InternalAbstractions;
using Microsoft.DotNet.ProjectModel;
using NuGet.Frameworks;

namespace Microsoft.EntityFrameworkCore.Tools.DotNet.Internal
{
    public class ProjectContextFactory
    {
        public IProjectContext Create(string filePath,
            NuGetFramework framework,
            string configuration = null,
            string outputDir = null)
        {
            var project = SelectCompatibleFramework(
                framework,
                ProjectContext.CreateContextForEachFramework(filePath,
                    runtimeIdentifiers: Microsoft.DotNet.Cli.Utils.RuntimeEnvironmentRidExtensions.GetAllCandidateRuntimeIdentifiers()));

            return new DotNetProjectContext(project,
                configuration ?? Constants.DefaultConfiguration,
                outputDir);
        }

        private ProjectContext SelectCompatibleFramework(NuGetFramework target, IEnumerable<ProjectContext> contexts)
            => NuGetFrameworkUtility.GetNearest(contexts, target ?? FrameworkConstants.CommonFrameworks.NetCoreApp10, f => f.TargetFramework)
                   ?? contexts.First();
    }
}
