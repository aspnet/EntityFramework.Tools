// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.DotNet.Cli.CommandLine;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    internal partial class DatabaseUpdateCommand : ContextCommandBase
    {
        private CommandArgument _migration;

        public override void Configure(CommandLineApplication command)
        {
            command.Description = "Updates the database to a specified migration.";

            _migration = command.Argument(
                "<MIGRATION>",
                "The target migration. If '0', all migrations will be reverted. Defaults to the last migration.");

            base.Configure(command);
        }
    }
}
