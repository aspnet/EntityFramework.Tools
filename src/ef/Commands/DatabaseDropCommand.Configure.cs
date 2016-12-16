// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.DotNet.Cli.CommandLine;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    internal partial class DatabaseDropCommand : ContextCommandBase
    {
        private CommandOption _force;
        private CommandOption _dryRun;
        private CommandOption _json;

        public override void Configure(CommandLineApplication command)
        {
            command.Description = "Drops the database";

            _force = command.Option(
                "-f|--force",
                "Don't confirm.");
            _dryRun = command.Option(
                "--dry-run",
                "Show which database would be dropped, but don't dorp it.");
            _json = Json.ConfigureOption(command);

            base.Configure(command);
        }
    }
}
