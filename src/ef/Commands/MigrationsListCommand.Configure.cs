// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.DotNet.Cli.CommandLine;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    internal partial class MigrationsListCommand : ContextCommandBase
    {
        private CommandOption _json;

        public override void Configure(CommandLineApplication command)
        {
            command.Description = "Lists available migrations";

            _json = command.JsonOption();

            base.Configure(command);
        }
    }
}
