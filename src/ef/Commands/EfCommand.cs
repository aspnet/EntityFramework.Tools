// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.CommandLineUtils;

using static Microsoft.EntityFrameworkCore.Tools.Utilities.AnsiConstants;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    public static class EfCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            app.Command("database", c => DatabaseCommand.Configure(c, options));

            app.Command("dbcontext", c => DbContextCommand.Configure(c, options));

            app.Command("migrations", c => MigrationsCommand.Configure(c, options));

            app.OnExecute(() =>
                {
                    WriteLogo();
                    app.ShowHelp();
                });
        }

        public static void WriteLogo()
        {
            var lines = new[]
            {
                string.Empty,
                Reporter.Colorize(@"                     _/\__       ", s => s.Insert(21, Bold + Gray)),
                Reporter.Colorize(@"               ---==/    \\      ", s => s.Insert(20, Bold + Gray)),
                Reporter.Colorize(@"         ___  ___   |.    \|\    ", s => s.Insert(26, Bold).Insert(21, Dark).Insert(20, Bold + Gray).Insert(9, Dark + Magenta)),
                Reporter.Colorize(@"        | __|| __|  |  )   \\\   ", s => s.Insert(20, Bold + Gray).Insert(8, Dark + Magenta)),
                Reporter.Colorize(@"        | _| | _|   \_/ |  //|\\ ", s => s.Insert(20, Bold + Gray).Insert(8, Dark + Magenta)),
                Reporter.Colorize(@"        |___||_|       /   \\\/\\", s => s.Insert(33, Reset).Insert(23, Bold + Gray).Insert(8, Dark + Magenta)),
                string.Empty
            };

            Reporter.Output(string.Join(Environment.NewLine, lines));
        }
    }
}
