// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.DotNet.Cli.CommandLine;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    internal partial class DbContextScaffoldCommand : ProjectCommandBase
    {
        private CommandArgument _connection;
        private CommandArgument _provider;
        private CommandOption _dataAnnotations;
        private CommandOption _context;
        private CommandOption _force;
        private CommandOption _outputDir;
        private CommandOption _schemas;
        private CommandOption _tables;
        private CommandOption _json;

        public override void Configure(CommandLineApplication command)
        {
            command.Description = "Scaffolds a DbContext and entity types for a database.";

            _connection = command.Argument(
                "<CONNECTION>",
                "The connection string to the database.");
            _provider = command.Argument(
                "<PROVIDER>",
                "The provider to use. (E.g. Microsoft.EntityFrameworkCore.SqlServer)");

            _dataAnnotations = command.Option(
                "-d|--data-annotations",
                "Use attributes to configure the model (where possible). If omitted, only the fluent API is used.");
            _context = command.Option(
                "-c|--context <NAME>",
                "The name of the DbContext to generate.");
            _force = command.Option(
                "-f|--force",
                "Overwrite existing files.");
            _outputDir = command.Option(
                "-o|--output-dir <PATH>",
                "The directory to put files in. Paths are relaive to the project directory.");
            _schemas = command.Option(
                "--schema <SCHEMA_NAME>...",
                "The schemas of tables to generate entity types for.");
            _tables = command.Option(
                "-t|--table <TABLE_NAME>...",
                "The tables to generate entity types for.");
            _json = Json.ConfigureOption(command);

            base.Configure(command);
        }
    }
}
