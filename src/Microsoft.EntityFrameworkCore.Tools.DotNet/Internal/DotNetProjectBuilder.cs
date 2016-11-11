// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.EntityFrameworkCore.Tools.DotNet.Properties;

namespace Microsoft.EntityFrameworkCore.Tools.DotNet.Internal
{
    public class DotNetProjectBuilder : IProjectBuilder
    {
        public void EnsureBuild(IProjectContext project)
        {
            ICommand command;
            if (project is DotNetProjectContext)
            {
                command = CreateDotNetBuildCommand(project);
            }
            else
            {
                Debug.Assert(project is MsBuildProjectContext, "Unexpected project type.");
                command = CreateMsBuildBuildCommand(project);
            }

            var buildExitCode = command
                .CaptureStdOut()
                .CaptureStdErr()
                .Execute()
                .ExitCode;

            if (buildExitCode != 0)
            {
                throw new OperationErrorException(string.Format(ToolsDotNetStrings.BuildFailed, project.ProjectName));
            }
        }

        private static ICommand CreateDotNetBuildCommand(IProjectContext projectContext)
        {
            var args = new List<string>
            {
                projectContext.ProjectFullPath,
                "--configuration", projectContext.Configuration,
                "--framework", projectContext.TargetFramework.GetShortFolderName()
            };

            if (projectContext.TargetDirectory != null)
            {
                args.Add("--output");
                args.Add(projectContext.TargetDirectory);
            }

            return Command.CreateDotNet(
                "build",
                args,
                projectContext.TargetFramework,
                projectContext.Configuration);
        }

        private static ICommand CreateMsBuildBuildCommand(IProjectContext projectContext)
        {
            var args = new List<string>
            {
                projectContext.ProjectFullPath,
                "--configuration", projectContext.Configuration,
                "--framework", projectContext.TargetFramework.GetShortFolderName()
            };

            if (projectContext.TargetDirectory != null)
            {
                args.Add("--output");
                args.Add(projectContext.TargetDirectory);
            }

            // Force deps.json file to be generated
            args.Add("/p:GenerateDependencyFile=true");

            return Command.CreateDotNet("build", args);
        }
    }
}
