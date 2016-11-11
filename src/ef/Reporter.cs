// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Tools;
using Microsoft.EntityFrameworkCore.Tools.Utilities;

using static Microsoft.EntityFrameworkCore.Tools.Utilities.AnsiConstants;

namespace Microsoft.EntityFrameworkCore.Tools
{
    public static class Reporter
    {
        public const string JsonPrefix = "//BEGIN";
        public const string JsonSuffix = "//END";

        private static readonly bool _ansiPassThru;

        static Reporter()
        {
            _ansiPassThru = Environment.GetEnvironmentVariable("DOTNET_CLI_CONTEXT_ANSI_PASS_THRU") == bool.TrueString;
        }

        public static bool IsVerbose { get; set; }
        public static bool NoColor { get; set; }
        public static bool PrefixOutput { get; set; }

        public static string Colorize(string value, Func<string, string> colorizeFunc)
            => NoColor ? value : colorizeFunc(value);

        public static void Error(string message)
            => WriteToError(Prefix("ERROR   : ", Colorize(message, x => Bold + Red + x + Reset)));

        public static void Warning(string message)
            => WriteToOut(Prefix("WARNING : ", Colorize(message, x => Bold + Yellow + x + Reset)));

        public static void Output(string message)
            => WriteToOut(Prefix("OUTPUT  : ", message));

        public static void Verbose(string message)
        {
            if (IsVerbose)
            {
                WriteToOut(Prefix("VERBOSE : ", Colorize(message, x => Bold + Black + x + Reset)));
            }
        }

        private static string Prefix(string prefix, string value)
            => PrefixOutput
                ? prefix + value
                : value;

        private static void WriteToOut(string value)
        {
            if (NoColor || _ansiPassThru)
            {
                Console.WriteLine(value);
            }
            else
            {
                AnsiConsole.WriteLine(value);
            }
        }

        private static void WriteToError(string value)
        {
            if (NoColor || _ansiPassThru)
            {
                Console.Error.WriteLine(value);
            }
            else
            {
                AnsiConsole.Error.WriteLine(value);
            }
        }
    }
}
