// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
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
            string configuration,
            string outputDir)
        {
            configuration = configuration ?? Constants.DefaultConfiguration;

            if (Directory.Exists(filePath))
            {
                var msbuildFiles = Directory.EnumerateFiles(filePath, "*.*proj")
                    .Where(f => !string.Equals(Path.GetExtension(f), ".xproj", StringComparison.OrdinalIgnoreCase))
                    .Take(2).ToList();
                if (msbuildFiles.Count == 2)
                {
                    throw new OperationErrorException($"Multiple projects found in the directory '{filePath}'");
                }
                if (msbuildFiles.Count == 1)
                {
                    return CreateMsBuildContext(msbuildFiles[0], framework, configuration, outputDir);
                }

                return CreateDotNetContext(filePath, framework, configuration, outputDir);
            }

            if (!Path.GetFileName(filePath).Equals("project.json", StringComparison.OrdinalIgnoreCase))
            {
                return CreateMsBuildContext(filePath, framework, configuration, outputDir);
            }

            return CreateDotNetContext(filePath, framework, configuration, outputDir);
        }

        private IProjectContext CreateDotNetContext(
            string filePath,
            NuGetFramework framework,
            string configuration,
            string outputDir)
        {
            var project = SelectCompatibleFramework(
                framework,
                ProjectContext.CreateContextForEachFramework(filePath,
                    runtimeIdentifiers: Microsoft.DotNet.Cli.Utils.RuntimeEnvironmentRidExtensions.GetAllCandidateRuntimeIdentifiers()));

            return new DotNetProjectContext(project,
                configuration,
                outputDir);
        }

        private IProjectContext CreateMsBuildContext(
            string filePath,
            NuGetFramework framework,
            string configuration,
            string outputDir)
            => new MsBuildProjectContext(filePath, framework, configuration, outputDir);

        private ProjectContext SelectCompatibleFramework(NuGetFramework target, IEnumerable<ProjectContext> contexts)
            => NuGetFrameworkUtility.GetNearest(contexts, target ?? FrameworkConstants.CommonFrameworks.NetCoreApp10, f => f.TargetFramework)
                   ?? contexts.First();
    }
}
