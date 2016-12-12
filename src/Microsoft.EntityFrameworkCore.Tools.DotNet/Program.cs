// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.EntityFrameworkCore.Tools.Properties;
using Microsoft.Extensions.CommandLineUtils;

namespace Microsoft.EntityFrameworkCore.Tools
{
    internal static class Program
    {
        private static CommandOption _project;
        private static CommandOption _startupProject;
        private static CommandOption _framework;
        private static CommandOption _configuration;
        private static CommandOption _msbuildprojectextensionspath;
        private static CommandOption _verbose;
        private static IList<string> _args;

        private static int Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                Name = "dotnet ef",
                FullName = "Entity Framework Core .NET Command Line Tools",
                AllowArgumentSeparator = true
            };

            _project = app.Option(
                "-p|--project <PROJECT>",
                "The project to use. Defaults to the current directory.");
            _startupProject = app.Option(
                "-s|--startup-project <PROJECT>",
                "The startup project to use. Defaults to the target project.");
            _framework = app.Option(
                "-f|--framework <FRAMEWORK>",
                "The framework to target.");
            _configuration = app.Option(
                "-c|--configuration <CONFIGURATION>",
                "The configuration to use.");
            _msbuildprojectextensionspath = app.Option(
                "--msbuildprojectextensionspath <PATH>",
                "The MSBuild project extensions path. Defaults to 'obj'.");
            _verbose = app.Option(
                "-v|--verbose",
                "Show verbose output.");

            _args = app.RemainingArguments;
            app.ArgumentSeparatorHelpText = "Any extra options that should be passed to ef.";

            app.OnExecute(() => Execute());

            try
            {
                return app.Execute(args);
            }
            catch (Exception ex)
            {
                if (!(ex is CommandException || ex is CommandParsingException))
                {
                    Reporter.WriteVerbose(ex.ToString());
                }

                Reporter.WriteError(ex.Message);

                return 1;
            }
        }

        private static int Execute()
        {
            Reporter.IsVerbose = _verbose.HasValue();

            var projectFiles = FindProjects(_project.Value());
            if (projectFiles.Count == 0)
            {
                throw new CommandException(
                    _project.HasValue()
                        ? string.Format(Resources.NoProjectInDirectory, _project.Value())
                        : Resources.NoProject);
            }
            if (projectFiles.Count != 1)
            {
                throw new CommandException(
                    _project.HasValue()
                        ? string.Format(Resources.MultipleProjectsInDirectory, _project.Value())
                        : Resources.MultipleProjects);
            }
            var projectFile = projectFiles[0];

            Project project;
            Project startupProject;
            if (_startupProject.HasValue())
            {
                var starupProjectFiles = FindProjects(_startupProject.Value());
                if (starupProjectFiles.Count == 0)
                {
                    throw new CommandException(
                        string.Format(Resources.NoStartupProject, _startupProject.Value()));
                }
                if (starupProjectFiles.Count != 1)
                {
                    throw new CommandException(string.Format(Resources.MultipleStartupProjects, _project.Value()));
                }

                project = Project.FromFile(projectFile, _msbuildprojectextensionspath.Value());
                startupProject = Project.FromFile(
                    starupProjectFiles[0],
                    _msbuildprojectextensionspath.Value(),
                    _framework.Value(),
                    _configuration.Value());
            }
            else
            {
                project = Project.FromFile(
                    projectFile,
                    _msbuildprojectextensionspath.Value(),
                    _framework.Value(),
                    _configuration.Value());
                startupProject = project;
            }

            startupProject.Build();

            string executable;
            var args = new List<string>();

            var toolsPath = Path.GetFullPath(
                Path.Combine(
                    Path.GetDirectoryName(typeof(Program).GetTypeInfo().Assembly.Location),
                    "..",
                    "..",
                    "tools"));

            var targetDir = Path.Combine(startupProject.ProjectDir, startupProject.OutputPath);
            var targetPath = Path.Combine(targetDir, project.TargetFileName);
            var startupTargetPath = Path.Combine(targetDir, startupProject.TargetFileName);
            var depsFile = Path.Combine(
                targetDir,
                startupProject.AssemblyName + ".deps.json");
            var runtimeConfig = Path.Combine(
                targetDir,
                startupProject.AssemblyName + ".runtimeconfig.json");

            var targetFramework = new FrameworkName(startupProject.TargetFrameworkMoniker);
            if (targetFramework.Identifier == ".NETFramework")
            {
                executable = Path.Combine(
                    toolsPath,
                    "net451",
                    startupProject.PlatformTarget == "x86"
                        ? "ef.x86.exe"
                        : "ef.exe");
            }
            else if (targetFramework.Identifier == ".NETCoreApp"
                || targetFramework.Identifier == ".NETStandard")
            {
                if (targetFramework.Identifier == ".NETStandard")
                {
                    Reporter.WriteWarning(
                        string.Format(Resources.NETStandardStartupProject, startupProject.ProjectName));
                }

                executable = "dotnet";
                args.Add("exec");
                args.Add("--depsfile");
                args.Add(depsFile);
                args.Add("--additionalprobingpath");
                args.Add(startupProject.NuGetPackageRoot.TrimEnd(Path.DirectorySeparatorChar));

                if (File.Exists(runtimeConfig))
                {
                    args.Add("--runtimeconfig");
                    args.Add(runtimeConfig);
                }

                args.Add(Path.Combine(toolsPath, "netcoreapp1.0", "ef.dll"));
            }
            else
            {
                throw new CommandException(
                    string.Format(
                        Resources.UnsupportedFramework,
                        startupProject.ProjectName,
                        targetFramework.Identifier));
            }

            if (_verbose.HasValue())
            {
                args.Add("--verbose");
            }

            args.Add("--assembly");
            args.Add(targetPath);
            args.Add("--startup-assembly");
            args.Add(startupTargetPath);
            args.Add("--project-dir");
            args.Add(project.ProjectDir);
            args.Add("--content-root-path");
            args.Add(startupProject.ProjectDir);
            args.Add("--data-dir");
            args.Add(targetDir);

            if (project.RootNamespace.Length != 0)
            {
                args.Add("--root-namespace");
                args.Add(project.RootNamespace);
            }

            args.AddRange(_args);

            return Exe.Run(executable, args);
        }

        private static IReadOnlyList<string> FindProjects(string path)
        {
            if (path == null)
            {
                path = Directory.GetCurrentDirectory();
            }

            if (Directory.Exists(path))
            {
                return Directory.EnumerateFiles(path, "*.*proj", SearchOption.TopDirectoryOnly)
                    .Where(f => !string.Equals(Path.GetExtension(f), ".xproj", StringComparison.OrdinalIgnoreCase))
                    .Take(2).ToList();
            }

            return new[] { path };
        }
    }
}
