// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.IO;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.EntityFrameworkCore.Tools.DotNet.Properties;

namespace Microsoft.EntityFrameworkCore.Tools.DotNet.Internal
{
    public class DispatchOperationExecutor
    {
        private readonly EfConsoleCommandSpecFactory _commandSpecFactory;
        private readonly ProjectContextFactory _projectFactory;
        private readonly IProjectBuilder _projectBuilder;

        public DispatchOperationExecutor(
            ProjectContextFactory projectFactory,
            EfConsoleCommandSpecFactory commandSpecFactory,
            IProjectBuilder projectBuilder)
        {
            Debug.Assert(projectFactory != null, "projectFactory is null.");
            Debug.Assert(commandSpecFactory != null, "commandSpecFactory is null.");
            Debug.Assert(projectBuilder != null, "projectBuilder is null.");

            _commandSpecFactory = commandSpecFactory;
            _projectFactory = projectFactory;
            _projectBuilder = projectBuilder;
        }

        public int Execute(CommandLineOptions options)
        {
            Debug.Assert(options != null, "options is null.");

            var targetProjectPath = options.TargetProject ?? Directory.GetCurrentDirectory();

            var targetProject = _projectFactory.Create(targetProjectPath,
                    options.Framework,
                    options.Configuration,
                    options.BuildOutputPath);

            var startupProject = string.IsNullOrEmpty(options.StartupProject)
                ? targetProject
                : _projectFactory.Create(options.StartupProject,
                    targetProject.TargetFramework,
                    options.Configuration,
                    options.BuildOutputPath);

            Reporter.Verbose.WriteLine(string.Format(ToolsDotNetStrings.LogUsingTargetProject, targetProject.ProjectName));
            Reporter.Verbose.WriteLine(string.Format(ToolsDotNetStrings.LogUsingStartupProject, startupProject.ProjectName));
            Reporter.Verbose.WriteLine(string.Format(ToolsDotNetStrings.LogUsingFramework, startupProject.TargetFramework.GetShortFolderName()));
            Reporter.Verbose.WriteLine(string.Format(ToolsDotNetStrings.LogUsingConfiguration, startupProject.Configuration));

            if (!options.NoBuild)
            {
                _projectBuilder.EnsureBuild(startupProject);
            }

            Reporter.Verbose.WriteLine(string.Format(ToolsDotNetStrings.LogDataDirectory, startupProject.TargetDirectory));

            var commandSpec = _commandSpecFactory.Create(startupProject, targetProject, options.IsVerbose, options.RemainingArguments);

            Reporter.Verbose.WriteLine(string.Format(ToolsDotNetStrings.LogBeginDispatch, startupProject.ProjectName));

            return Command.Create(commandSpec)
                .Execute()
                .ExitCode;
        }
    }
}
