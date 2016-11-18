// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.EntityFrameworkCore.Tools.DotNet.FunctionalTests.Utilities;
using Microsoft.EntityFrameworkCore.Tools.DotNet.TestUtilities;

namespace Microsoft.EntityFrameworkCore.Tools.DotNet.FunctionalTests
{
    public class DotNetEfFixture : IDisposable
    {
        public string TestProjectRoot { get; } = Path.Combine(Path.GetTempPath(), "ef.tools.tests", Guid.NewGuid().ToString("N"));

        private readonly string _testProjectsSource = Path.Combine(AppContext.BaseDirectory, "TestProjects");

        public DotNetEfFixture()
        {
            Directory.CreateDirectory(TestProjectRoot);
            FileUtility.DirectoryCopy(_testProjectsSource, TestProjectRoot, recursive: true);

            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir.Parent != null)
            {
                var nugetConfig = Path.Combine(dir.FullName, "NuGet.config");
                if (File.Exists(nugetConfig))
                {
                    File.Copy(nugetConfig, Path.Combine(TestProjectRoot, "NuGet.config"));
                    break;
                }
                dir = dir.Parent;
            }

            foreach (var file in Directory.EnumerateFiles(TestProjectRoot, "project.json.ignore", SearchOption.AllDirectories))
            {
                File.Move(file, Path.Combine(Path.GetDirectoryName(file), "project.json"));
            }

            Console.WriteLine($"Fixture work dir = {TestProjectRoot}".Bold().Black());
            Console.WriteLine("Restoring test projects...".Bold().Black());
            AssertCommand.Pass(new DotnetRestore(TestProjectRoot, null).Execute());
            Console.WriteLine("Restore done.".Bold().Black());
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(TestProjectRoot, recursive: true);
            }
            catch
            {
                Console.Error.WriteLine("Failed to delete " + TestProjectRoot);
                throw;
            }
        }
    }
}
