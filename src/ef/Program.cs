// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.DotNet.Cli.CommandLine;
using Microsoft.EntityFrameworkCore.Tools.Commands;

namespace Microsoft.EntityFrameworkCore.Tools
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                Name = "ef"
            };

            new RootCommand().Configure(app);

            try
            {
                return app.Execute(args);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                {
                    ex = ex.InnerException;
                }

                if (!(ex is CommandException || ex is CommandParsingException))
                {
                    Reporter.WriteVerbose(ex.ToString());
                }

                Reporter.WriteError(ex.Message);

                return 1;
            }
        }
    }
}
