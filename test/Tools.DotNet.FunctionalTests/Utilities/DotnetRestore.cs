// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Linq;
using Microsoft.DotNet.Cli.Utils;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Tools.DotNet.FunctionalTests.Utilities
{
    public class DotnetRestore : Command
    {
        public DotnetRestore(string projectPath, ITestOutputHelper output)
            : base("dotnet", output)
        {
            WorkingDirectory = Path.GetDirectoryName(projectPath);
        }

        public override CommandResult Execute(params string[] args)
            => base.Execute(new[] { "restore" }.Concat(args).ToArray());
    }
}