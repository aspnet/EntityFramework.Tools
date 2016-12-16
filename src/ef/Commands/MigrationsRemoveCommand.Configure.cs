// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.DotNet.Cli.CommandLine;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    internal partial class MigrationsRemoveCommand : ContextCommandBase
    {
        private CommandOption _force;
        private CommandOption _json;

        public override void Configure(CommandLineApplication command)
        {
            command.Description = "Removes the last migration.";

            _force = command.Option(
                "-f|--force",
                "Don't check to see if the migration has been applied to the database.");
            _json = Json.ConfigureOption(command);

            base.Configure(command);
        }
    }
}
