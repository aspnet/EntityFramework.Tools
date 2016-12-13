// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.DotNet.Cli.CommandLine;

namespace Microsoft.EntityFrameworkCore.Tools
{
    internal class ProjectOptions
    {
        private CommandOption _project;
        private CommandOption _startupProject;
        private CommandOption _framework;
        private CommandOption _configuration;
        private CommandOption _msbuildprojectextensionspath;

        public CommandOption Project
            => _project;

        public CommandOption StartupProject
            => _startupProject;

        public CommandOption Framework
            => _framework;

        public CommandOption Configuration
            => _configuration;

        public CommandOption MSBuildProjectExtensionsPath
            => _msbuildprojectextensionspath;

        public void Configure(CommandLineApplication command)
        {
            _project = command.Option(
                "-p|--project <PROJECT>",
                "The project to use.");
            _startupProject = command.Option(
                "-s|--startup-project <PROJECT>",
                "The startup project to use. Defaults to the target project.");
            _framework = command.Option(
                "--framework <FRAMEWORK>",
                "The target framework.");
            _configuration = command.Option(
                "--configuration <CONFIGURATION>",
                "The configuration to use.");
            _msbuildprojectextensionspath = command.Option(
                "--msbuildprojectextensionspath <PATH>",
                "The MSBuild project extensions path. Defaults to 'obj'.");
        }
    }
}
