// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.DotNet.Cli.CommandLine;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    internal partial class MigrationsAddCommand : ContextCommandBase
    {
        private CommandArgument _name;
        private CommandOption _outputDir;
        private CommandOption _json;

        public override void Configure(CommandLineApplication command)
        {
            command.Description = "Add a new migration.";

            _name = command.Argument("<NAME>", "The name of the migration.");

            _outputDir = command.Option(
                "-o|--output-dir <PATH>",
                "The directory (and sub-namespace) to use. Paths are relative to the project directory. Defaults to \"Migrations\".");
            _json = Json.ConfigureOption(command);

            base.Configure(command);
        }
    }
}
