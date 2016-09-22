// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Microsoft.EntityFrameworkCore.Tools.DotNet.FunctionalTests.Utilities;
using Microsoft.EntityFrameworkCore.Tools.DotNet.Properties;
using Microsoft.EntityFrameworkCore.Tools.DotNet.TestUtilities;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Tools.DotNet.FunctionalTests
{
    public partial class EndToEndTests
    {
        [ConditionalFact]
        [PlatformSkipCondition(TestPlatform.Linux | TestPlatform.Mac)]
        public void Unsupported_MigrationsOnDesktopClassLibrary()
        {
            var targetProject = Path.Combine(_fixture.TestProjectRoot, "DesktopClassLibrary/project.json");

            var result = new AddMigration(targetProject, "Initial", _output)
                .ExecuteWithCapturedOutput();
            AssertCommand.Fail(result);
            Assert.Contains(string.Format(ToolsDotNetStrings.ClassLibrariesNotSupported, "DesktopClassLibrary"), result.StdErr);
        }

        [Fact]
        public void Unsupported_MigrationsOnNetStandardClassLibrary()
        {
            var targetProject = Path.Combine(_fixture.TestProjectRoot, "NetStandardClassLibrary/project.json");

            var result = new AddMigration(targetProject, "Initial", _output)
                .ExecuteWithCapturedOutput();
            AssertCommand.Fail(result);
            Assert.Contains(string.Format(ToolsDotNetStrings.ClassLibrariesNotSupported, "NetStandardClassLibrary"), result.StdErr);
        }
    }
}