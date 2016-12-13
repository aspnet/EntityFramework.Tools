// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.DotNet.Cli.CommandLine;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    internal partial class MigrationsScriptCommand : ContextCommandBase
    {
        private CommandArgument _from;
        private CommandArgument _to;
        private CommandOption _output;
        private CommandOption _idempotent;

        public override void Configure(CommandLineApplication command)
        {
            command.Description = "Generates a SQL script from migrations.";

            _from = command.Argument(
                "<FROM>",
                "The starting migration. Defaults to '0' (the initial database).");
            _to = command.Argument(
                "<TO>",
                "The ending migration. Defaults to the last migration.");

            _output = command.Option(
                "-o|--output <FILE>",
                "The file to write the script to.");
            _idempotent = command.Option(
                "-i|--idempotent",
                "Generate a script that can be used on a database at any migration.");

            base.Configure(command);
        }
    }
}
