// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.DotNet.Cli.Utils;
using NuGet.Frameworks;

namespace Microsoft.EntityFrameworkCore.Tools.DotNet.Internal
{
    public class MsBuildProjectContext : IProjectContext
    {
        public MsBuildProjectContext(string filePath, NuGetFramework framework, string configuration, string outputDir)
        {
            var metadata = GetProjectMetadata(filePath, framework, configuration, outputDir);

            Configuration = configuration;
            ProjectName = Path.GetFileNameWithoutExtension(filePath);
            ProjectFullPath = metadata["ProjectPath"];
            RootNamespace = metadata["RootNamespace"] ?? ProjectName;
            TargetFramework = NuGetFramework.Parse(metadata["NuGetTargetMoniker"]);
            IsClassLibrary = string.Equals(metadata["OutputType"], "Library", StringComparison.OrdinalIgnoreCase);
            TargetDirectory = metadata["TargetDir"];
            Platform = metadata["Platform"];
            AssemblyFullPath = metadata["TargetPath"];
            PackagesDirectory = metadata["NuGetPackageRoot"];

            // TODO get from actual properties according to TFM
            RuntimeConfigJson = Path.Combine(TargetDirectory, Path.GetFileNameWithoutExtension(AssemblyFullPath) + ".runtimeconfig.json");
            DepsJson = Path.Combine(TargetDirectory, Path.GetFileNameWithoutExtension(AssemblyFullPath) + ".deps.json");
        }

        public NuGetFramework TargetFramework { get; }
        public bool IsClassLibrary { get; }
        public string DepsJson { get; }
        public string RuntimeConfigJson { get; }
        public string PackagesDirectory { get; }
        public string AssemblyFullPath { get; }
        public string ProjectName { get; }
        public string Configuration { get; }
        public string Platform { get; }
        public string ProjectFullPath { get; }
        public string RootNamespace { get; }
        public string TargetDirectory { get; }

        private IReadOnlyDictionary<string, string> GetProjectMetadata(string filePath, NuGetFramework framework, string configuration, string outputDir)
        {
            var metadataFile = Path.GetTempFileName();
            try
            {
                var args = new List<string>
                {
                    "/t:_EFGetProjectMetadata",
                    "/p:Configuration=" + configuration,
                    "/p:_EFProjectMetadataFile=" + metadataFile
                };

                if (outputDir != null)
                {
                    args.Add("/p:OutputPath=" + outputDir);
                }

                if (framework != null)
                {
                    args.Add("/p:TargetFramework=" + framework.GetShortFolderName());
                }

                var command = Command.CreateDotNet("msbuild", args)
                    .CaptureStdOut()
                    .CaptureStdErr();
                var result = command.Execute();
                if (result.ExitCode != 0)
                {
                    throw new OperationErrorException(
                        $"Couldn't read metadata for project '{filePath}'. Ensure the package 'Microsoft.EntityFrameworkCore.Tools' is installed.");
                }

                return File.ReadLines(metadataFile).Select(l => l.Split(new[] { ':' }, 2))
                    .ToDictionary(s => s[0], s => s[1].TrimStart());
            }
            finally
            {
                File.Delete(metadataFile);
            }
        }
    }
}