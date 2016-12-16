// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.DotNet.Cli.CommandLine;

namespace Microsoft.EntityFrameworkCore.Tools
{
    internal static class Json
    {
        public static CommandOption ConfigureOption(CommandLineApplication command)
            => command.Option("--json", "Show JSON output.");

        public static string Escape(string text)
            => text.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
